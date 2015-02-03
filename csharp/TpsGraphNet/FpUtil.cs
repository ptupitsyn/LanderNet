using System.Runtime.InteropServices;

namespace TpsGraphNet
{
    public static class FpUtil
    {
        // Constants from float.h

        #region public static methods

        /// <summary>
        ///     Resets the Floating-Point Control Registers to their default state.
        ///     This is needed after each call to unmanaged code due to known issue: http://support.microsoft.com/kb/326219
        /// </summary>
        public static void ResetFpuRegisters()
        {
            _controlfp(_CW_DEFAULT, 0xfffff);
        }

        #endregion

        #region private static methods

        [DllImport("msvcr70.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int _controlfp(int n, int mask);

        #endregion

        #region private constants

        private const int _RC_NEAR = 0x00000000; /*   near */
        private const int _PC_53 = 0x00010000; /*    53 bits */
        private const int _EM_INEXACT = 0x00000001; /*   inexact (precision) */
        private const int _EM_UNDERFLOW = 0x00000002; /*   underflow */
        private const int _EM_OVERFLOW = 0x00000004; /*   overflow */
        private const int _EM_ZERODIVIDE = 0x00000008; /*   zero divide */
        private const int _EM_INVALID = 0x00000010; /*   invalid */
        private const int _EM_DENORMAL = 0x00080000; /* denormal exception mask (_control87 only) */
        private const int _CW_DEFAULT = (_RC_NEAR + _PC_53 + _EM_INVALID + _EM_ZERODIVIDE + _EM_OVERFLOW + _EM_UNDERFLOW + _EM_INEXACT + _EM_DENORMAL); /* initial Control Word value */

        #endregion
    }
}