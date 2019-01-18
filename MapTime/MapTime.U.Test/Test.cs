using NUnit.Framework;
using System;
namespace MapTime.U.Test
{
    [TestFixture()]
    public class Test
    {
        [Test()]
        public void MapRange_DefaultTest()
        {
            float[] expected = { 50f, 500f, 50f, 360f };
            float[] actual = {
                Utils.MapRange(25, 0, 50, 0, 100),
                Utils.MapRange(5, 0,5, 0, 500),
                Utils.MapRange(0, -50, 50, 0, 100),
                Utils.MapRange(180, -180, 180, 0, 360)
            };

            for(int i = 0; i < expected.Length; i++)
            {
                Assert.AreEqual(expected[i], actual[i]);
            }
        }
    }
}
