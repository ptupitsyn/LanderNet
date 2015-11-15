using System;
using System.Runtime.InteropServices;

// ReSharper disable InconsistentNaming
namespace TpsGraphNet
{
    public static class NativeMethods
    {
        #region public static methods

        [DllImport("kernel32", EntryPoint = "CreateFileMapping", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr CreateFileMapping(IntPtr hFile, IntPtr lpAttributes, int flProtect, int dwMaximumSizeLow, int dwMaximumSizeHigh, string lpName);

        [DllImport("kernel32", EntryPoint = "CloseHandle", SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32", SetLastError = true)]
        public static extern IntPtr MapViewOfFile(IntPtr hFileMappingObject, uint dwDesiredAccess, uint dwFileOffsetHigh, uint dwFileOffsetLow, uint dwNumberOfBytesToMap);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern bool UnmapViewOfFile(IntPtr lpBaseAddress);
        #endregion


        #region private fields

        public const uint FILE_MAP_ALL_ACCESS = 0xF001F;

        public const uint PAGE_READWRITE = 0x04;

        public static  readonly IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        #endregion
    }
}