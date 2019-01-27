using MapTime.Handlers;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace MapTime
{
    public partial class MainForm : Form
    {
        // MISC
        float timeoffset = 0;
        long startOffset;

        // TIMING
        long latestRefresh = 0;

        // CONFIG VARS
        Font DEFAULT_FONT = new Font("Consolas", 9);
        float DisplayHours;
        Image MAP;
        bool DrawPositions;
        bool DrawNames;
        float mapOffsetX, mapOffsetY;
        float scale;

        // BRUSHES
        readonly SolidBrush rectangleBrush = new SolidBrush(Color.FromArgb(0x7F, 255, 0, 0));
        readonly SolidBrush hoursBarBrush = new SolidBrush(Color.FromArgb(0x7F, Color.Black));
        readonly Pen linePen = new Pen(Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF));


        public MainForm()
        {
            // Importing Config
            bool configLoaded = ConfigHandler.InitConfig();
            if (!configLoaded)
            {
                throw new NullReferenceException("Config couldn't be loaded!");
            }

            scale = float.Parse(ConfigHandler.ReadKey("StartScale"), NumberStyles.Any, CultureInfo.InvariantCulture);
            MAP = Image.FromFile(ConfigHandler.ReadKey("MapImg"));
            DisplayHours = float.Parse(ConfigHandler.ReadKey("SelectedHours"), NumberStyles.Any, CultureInfo.InvariantCulture);
            DEFAULT_FONT = new Font(ConfigHandler.ReadKey("FontFamily"), 9);
            DrawPositions = Boolean.Parse(ConfigHandler.ReadKey("DrawPositions"));
            DrawNames = Boolean.Parse(ConfigHandler.ReadKey("DrawNamesBelow"));

            foreach (Location loc in ConfigHandler.ReadLocationList())
            {
                LocationHandler.AddLocation(loc);
            }

            // Intialize Form
            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;
            this.DoubleBuffered = true;
            this.Height = (int)(MAP.Height / (1 / scale));
            this.Width = (int)(MAP.Width / (1 / scale));

            CalcNullPos();

            startOffset = this.Width / 2;
        }

        private void CalcNullPos()
        {
            // Handle NullPosition
            Dictionary<string, string> positions = ConfigHandler.ReadAllKeyAttributes("MapOffset");
            string zX = String.Empty, zY = String.Empty;
            positions.TryGetValue("x", out zX);
            positions.TryGetValue("y", out zY);

            mapOffsetX = Utils.EvaluateString(zX);
            mapOffsetY = Utils.EvaluateString(zY);
        }

        private void RenderHourScale(Graphics gfx)
        {
            gfx.FillRectangle(hoursBarBrush, 0, this.Height - 22 - 15, this.Width, 22 + 15);

            float step = this.Width / 24f;
            float halfStep = step / 2f;
            for (int i = -11; i <= 12; i++)
            {
                float x = startOffset + (i * step);
                while (x > this.Width)
                {
                    x -= this.Width;
                }
                while (x < 0)
                {
                    x += this.Width;
                }

                Size textSize = TextRenderer.MeasureText(i.ToString(), DEFAULT_FONT);
                gfx.DrawString(i.ToString(), DEFAULT_FONT, Brushes.White, x - (textSize.Width / 2f), this.Height - 22 - textSize.Height);
                gfx.DrawLine(Pens.White, x, this.Height, x, this.Height - 22);
                gfx.DrawLine(Pens.White, x + halfStep, this.Height, x + halfStep, this.Height - 11);
            }
        }

        private void RenderRectangle(Graphics gfx)
        {
            float timezone = (this.Width / 24f) * DisplayHours;

            gfx.FillRectangle(rectangleBrush, startOffset - timezone / 2, 0, timezone, this.Height);

            // Handle Rectangle Over-/ Underflow
            if ((startOffset + timezone / 2) > this.Width)
            {
                float len = startOffset + timezone / 2;
                while (len > this.Width)
                {
                    len -= this.Width;
                }

                gfx.FillRectangle(rectangleBrush, 0, 0, len, this.Height);
            }
            else if ((startOffset - timezone / 2) < 0)
            {
                float len = startOffset - timezone / 2;
                while (len < 0)
                {
                    len += this.Width;
                }

                gfx.FillRectangle(rectangleBrush, len, 0, this.Width - len, this.Height);
            }

            // White Line
            gfx.DrawLine(linePen, startOffset, 0, startOffset, this.Height);
        }

        private void RenderPositions(Graphics gfx)
        {
            Size textScales;
            foreach (Location loc in LocationHandler.SavedLocations)
            {
                float x = Utils.MapRange(loc.Longitude, -180, 180, 0, this.Width);
                float y = Utils.MapRange(-loc.Latitude, -90, 90, 0, this.Height);

                float toDrawX = x + mapOffsetX;
                float toDrawY = y + mapOffsetY;

                while (toDrawX < 0)
                {
                    toDrawX += this.Width;
                }

                while(toDrawX >= this.Width)
                {
                    toDrawX -= this.Width;
                }

                while (toDrawY < 0)
                {
                    toDrawY += this.Height;
                }

                while (toDrawY >= this.Height)
                {
                    toDrawY -= this.Height;
                }

                gfx.FillEllipse(Brushes.Red, toDrawX - 3, toDrawY - 3, 6, 6);
                gfx.DrawEllipse(Pens.White, toDrawX - 3, toDrawY - 3, 6, 6);

                if(DrawNames)
                {
                    textScales = TextRenderer.MeasureText(loc.Name, DEFAULT_FONT);
                    gfx.DrawString(loc.Name, DEFAULT_FONT, Brushes.Black, toDrawX - textScales.Width / 2 + 1, toDrawY + textScales.Height / 2 + 1);
                    gfx.DrawString(loc.Name, DEFAULT_FONT, Brushes.White, toDrawX - textScales.Width / 2, toDrawY + textScales.Height/2);
                }
            }
        }

        #region ---- EVENTS ----
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;

            if (ConfigHandler.InitConfig())
            {
                DisplayHours = float.Parse(ConfigHandler.ReadKey("SelectedHours"), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture);
                DrawPositions = Boolean.Parse(ConfigHandler.ReadKey("DrawPositions"));
                DrawNames = Boolean.Parse(ConfigHandler.ReadKey("DrawNamesBelow"));
                CalcNullPos();
            }

            // Drawing Image
            gfx.DrawImage(MAP, 0, 0, MAP.Width / (1 / scale), MAP.Height / (1 / scale));
            // Rectangle Rendering
            this.RenderRectangle(gfx);

            // Hours Rendering
            this.RenderHourScale(gfx);

            // Render Positions
            if (DrawPositions)
            {
                this.RenderPositions(gfx);
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.Refresh();
        }


        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if(Environment.TickCount - latestRefresh < 17)
            {
                return;
            }

            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if (ModifierKeys == Keys.Control)
            {
                Point mouse = this.PointToScreen(e.Location);
                this.Left = mouse.X - this.Width / 2;
                this.Top = mouse.Y - this.Height / 2;

                return;
            }

            int x = e.X;
            if (e.X > this.Width)
            {
                x = this.Width;
            }
            else if (e.X < 0)
            {
                x = 0;
            }

            startOffset = x;

            this.Refresh();
            latestRefresh = Environment.TickCount;
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            float toAdd = 0.001f;
            if (ModifierKeys == Keys.Shift)
            {
                toAdd = 0.01f;
            }

            if (e.Delta < 0)
            {
                toAdd = -toAdd;
            }

            this.scale += toAdd;
            this.Height = (int)(MAP.Height / (1 / scale));
            this.Width = (int)(MAP.Width / (1 / scale));

            CalcNullPos();

            this.Refresh();
        }
        #endregion ---- EVENTS ----
    }
}
