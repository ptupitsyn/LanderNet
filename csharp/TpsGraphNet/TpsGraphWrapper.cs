using System.Runtime.InteropServices;
using System.Security;

namespace TpsGraphNet
{
    /// <summary>
    ///     Provides access to native TMT Pascal DLL functions.
    /// </summary>
    public static class TpsGraphWrapper
    {
        #region BlendMode enum

        public enum BlendMode
        {
            Additive = 1,
            Substractive = 2,
            HalfAdditive = 3
        }

        #endregion

        #region public static methods

        /// <summary>
        /// Sets the active layer (sprite) which other procedures will manipulate.
        /// </summary>
        /// <param name="spritePtr">Reference to the beginning of sprite struct.</param>
        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern void SetLayer(uint spritePtr);

        /// <summary>
        /// Set pixel.
        /// </summary>
        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern void PSET32(uint x, uint y, uint c);

        /// <summary>
        /// Get pixel.
        /// </summary>
        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern uint PGET32(uint x, uint y);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern uint InitSprite(uint width, uint height, uint bytesPerPixel);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern void FreeSprite(uint spritePtr);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern uint GetSizeOfSpriteHeader();

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern uint tPut32(uint x, uint y, uint transparentColor, uint spritePtr);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern uint btPut32(uint x, uint y, uint spritePtr, byte blendMode);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern void _CLS(uint color);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern void MemCopyMmx(uint srcPtr, uint dstPtr, uint length);

        [DllImport("TPSGRAPH", CallingConvention = CallingConvention.StdCall)]
        public static extern void MemCopy(uint srcPtr, uint dstPtr, uint length);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false), SuppressUnmanagedCodeSecurity]
        public static extern uint CopyMemory(uint dest, uint src, ulong count);

        [DllImport("msvcrt.dll", EntryPoint = "memset", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern uint MemSet(uint dest, uint color, uint count);

        #endregion
    }
}