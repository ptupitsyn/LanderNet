using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TpsGraphNet.Tests
{
    [TestClass]
    public class TpsGraphWrapperTests
    {
        #region public methods

        [TestMethod]
        public unsafe void MemCopyTest()
        {
            var src = new uint[] {1, 2, 3, 4};
            var dst = new uint[] {0, 0, 0, 0};

            fixed (uint* srcPtr = &src[0])
            fixed (uint* dstPtr = &dst[0])
            {
                TpsGraphWrapper.CopyMemoryMmx((uint) srcPtr, (uint) dstPtr, 16);
            }
        }

        #endregion
    }
}