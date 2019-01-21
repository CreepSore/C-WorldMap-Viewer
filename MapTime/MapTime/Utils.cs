using System;
using System.Data;
using System.Drawing;
using System.Windows.Media.Imaging;

namespace MapTime
{
    public static class Utils
    {

        public static float MapRange(float value, float rmin0, float rmax0, float rmin1, float rmax1)
        {
            float slope = (rmax1 - rmin1) / (rmax0 - rmin0);
            float newval = rmin1 + slope * (value - rmin0);

            return newval;
        }

        public static float EvaluateString(string expression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), expression);
            DataRow row = table.NewRow();
            table.Rows.Add(row);
            return float.Parse((string)row["expression"]);
        }

        public static BitmapSource CreateBitmapSourceFromBitmap(Bitmap bitmap)
        {
            if (bitmap == null)
                throw new ArgumentNullException("Failed BitmapSource Creation: Source is null!");

            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                System.Windows.Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());
        }
    }
}
