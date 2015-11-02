using System.Runtime.InteropServices;
using System.Security;

namespace TpsGraphNet
{
    /// <summary>
    /// Provides access to native TMT Pascal DLL functions.
    /// </summary>
    public static class TpsGraphWrapper
    {
        public static void ClearScreen(uint color)
        {
            Native._CLS(color);

            FpUtil.ResetFpuRegisters();
        }

        public static uint GetSizeOfSpriteHeader()
        {
            return Native.GetSizeOfSpriteHeader();
        }

        public static void MemCopyMmx(uint srcPtr, uint dstPtr, uint length)
        {
            Native.MemCopyMmx(srcPtr, dstPtr, length);

            FpUtil.ResetFpuRegisters();
        }

        public static uint InitSprite(uint width, uint height, uint bytesPerPixel)
        {
            var res = Native.InitSprite(width, height, bytesPerPixel);

            FpUtil.ResetFpuRegisters();

            return res;
        }

        public static void SetPixel(uint x, uint y, uint color)
        {
            Native.PSET32(x, y, color);

            FpUtil.ResetFpuRegisters();
        }

        public static uint GetPixel(uint x, uint y)
        {
            var res = Native.PGET32(x, y);

            FpUtil.ResetFpuRegisters();

            return res;
        }

        public static void PutSprite(uint x, uint y, uint transparentColor, uint dataPtr)
        {
            Native.tPut32(x, y, transparentColor, dataPtr);

            FpUtil.ResetFpuRegisters();
        }

        public static void PutSpriteBlend(uint x, uint y, uint dataPtr, byte blendMode)
        {
            Native.btPut32(x, y, dataPtr, blendMode);

            FpUtil.ResetFpuRegisters();
        }

        public static void CopyMemory(uint targetPtr, uint srcPtr, uint sizeInBytes)
        {
            Native.CopyMemory(targetPtr, srcPtr, sizeInBytes);

            FpUtil.ResetFpuRegisters();
        }

        public static void FreeSprite(uint dataPtr)
        {
            Native.FreeSprite(dataPtr);
        }

        public static void SetLayer(uint dataPtr)
        {
            Native.SetLayer(dataPtr);
        }
        
        private static class Native
        {
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
            
        }
    }
}