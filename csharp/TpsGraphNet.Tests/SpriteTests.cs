using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TpsGraphNet.Tests
{
    [TestClass]
    [DeploymentItem("TPSGRAPH.dll")]
    [DeploymentItem("msvcr70.dll")]
    public class SpriteTests
    {
        [TestMethod]
        public void GetSetPixelTest()
        {
            var spr = new Sprite(10, 10);
            const uint color = 256*256 + 13;
            spr.SetPixel(3, 4, color);
            Assert.AreEqual(color, spr.GetPixel(3, 4));
        }

        [TestMethod]
        public void GetRawDataTest()
        {
            var spr = new Sprite(10, 10);
            const uint color = 256*256 + 13;

            spr.SetPixel(0, 0, color);
            uint[] rawData = spr.GetRawData();
            Assert.AreEqual(color, rawData[0]);
        }
    }
}