using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MapTime
{
    public partial class Form1 : Form
    {
        float timeoffset = 0;
        float scale = 0.75f;
        long startOffset;
        readonly Font DEFAULT_FONT = new Font("Consolas", 9);
        readonly Image MAP = Image.FromFile("Graphics\\map2.JPG");
        readonly SolidBrush brush = new SolidBrush(Color.FromArgb(0x7F, 255, 0, 0));
        readonly Pen linePen = new Pen(Color.FromArgb(0x7F, 0xFF, 0xFF, 0xFF));
        readonly SolidBrush hoursBarBrush = new SolidBrush(Color.FromArgb(0x7F, Color.Black));



        public Form1()
        {
            InitializeComponent();
            this.MouseWheel += Form1_MouseWheel;

            this.Height = (int)(MAP.Height / (1 / scale));
            this.Width = (int)(MAP.Width / (1 / scale));

            startOffset = this.Width / 2;
            this.DoubleBuffered = true;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics gfx = e.Graphics;
            
            gfx.DrawImage(MAP, 0, 0, MAP.Width / (1/scale), MAP.Height / (1 / scale));

            float lat = 35.6693863f;
            float lng = 139.6012978f;

            float ly = mapRange(lat+90, 0, 180, 0, this.Height);
            float lx = mapRange(lng+180, 0, 360, 0, this.Width);

            gfx.DrawLine(Pens.White, lx, this.Height - ly - 5, lx, this.Height - ly + 5);
            gfx.DrawLine(Pens.White, lx - 5, this.Height - ly, lx + 5, this.Height - ly);

            float timezone = mapRange(timeoffset, 60*8, 60 * 24, 0, this.Width);
            gfx.FillRectangle(brush, startOffset - timezone / 2, 0, timezone, this.Height);
            if((startOffset + timezone/2) > this.Width)
            {
                float len = startOffset + timezone / 2;
                while(len > this.Width)
                {
                    len -= this.Width;
                }

                gfx.FillRectangle(brush, 0, 0, len, this.Height);
            }
            else if((startOffset - timezone/2) < 0)
            {
                float len = startOffset - timezone / 2;
                while(len < 0)
                {
                    len += this.Width;
                }

                gfx.FillRectangle(brush, len, 0, this.Width - len, this.Height);
            }

            
            gfx.DrawLine(linePen, startOffset, 0, startOffset, this.Height);



            float step = this.Width / 24;
            float offset = startOffset;
            gfx.FillRectangle(hoursBarBrush, 0, this.Height - 22 - 15, this.Width, 22 + 15);
            for(int i = -11; i <= 12; i++)
            {
                float x = startOffset + (i * step);
                while(x > this.Width)
                {
                    x -= this.Width;
                }
                while (x < 0)
                {
                    x += this.Width;
                }

                gfx.DrawString(i.ToString(), DEFAULT_FONT, Brushes.White, x - TextRenderer.MeasureText(i.ToString(), DEFAULT_FONT).Width / 2, this.Height - 22 - 15);
                gfx.DrawLine(Pens.White, x, this.Height, x, this.Height - 22);

                //string text = i + " ½";
                //gfx.DrawString(text, DEFAULT_FONT, Brushes.White, x + (step / 2) - TextRenderer.MeasureText(text, DEFAULT_FONT).Width / 2, this.Height - 11 - 15);
                gfx.DrawLine(Pens.White, x + (step / 2), this.Height, x + (step / 2), this.Height - 11);
            }
        }

        private void Form1_ResizeEnd(object sender, EventArgs e)
        {
            this.Refresh();
        }

        private float mapRange(float value, float rmin0, float rmax0, float rmin1, float rmax1)
        {
            float percent0 = (value + rmin0) / rmax0;
            float newval = (rmax1 * percent0) + (rmin1 * percent0);

            return newval;
        }

        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            float timezone = mapRange(timeoffset, 60 * 8, 60 * 24, 0, this.Width);
            if (e.Button != MouseButtons.Left)
            {
                return;
            }

            if(ModifierKeys == Keys.Control)
            {
                Point mouse = this.PointToScreen(e.Location);
                this.Left = mouse.X - this.Width / 2;
                this.Top = mouse.Y - this.Height / 2;

                return;
            }

            int x = e.X;
            if(e.X > this.Width)
            {
                x = this.Width;
            }
            else if(e.X < 0)
            {
                x = 0;
            }

            startOffset = x;
            this.Refresh();
        }

        private void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (e.Delta > 0)
            {
                float toAdd = 0.01f;
                if (ModifierKeys == Keys.LShiftKey)
                {
                    toAdd = 0.1f;
                }

                this.scale += toAdd;
                this.Height = (int)(MAP.Height / (1 / scale));
                this.Width = (int)(MAP.Width / (1 / scale));
                this.Refresh();
            }
            else if (e.Delta < 0)
            {
                float toSub = 0.01f;
                if (ModifierKeys == Keys.LShiftKey)
                {
                    toSub = 0.1f;
                }

                this.scale -= toSub;
                this.Height = (int)(MAP.Height / (1 / scale));
                this.Width = (int)(MAP.Width / (1 / scale));
                this.Refresh();
            }
        }
    }
}
