using System;
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

    }
}
