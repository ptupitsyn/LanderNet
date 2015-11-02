using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace TpsGraphNet
{
    public sealed class Sprite : IDisposable
    {
        #region private static properties and indexers

        private static uint SizeofSpriteHeader
        {
            get { return (uint) (_sizeofSpriteHeader ?? (_sizeofSpriteHeader = TpsGraphWrapper.GetSizeOfSpriteHeader())); }
        }

        #endregion

        #region private constants

        private const uint TransparentColor = 0xFF000000; // need to use bgra format with alpha set to 255

        #endregion

        #region public constructors

        public Sprite(uint width, uint height)
        {
            _width = width;
            _height = height;
            _sizeInBytes = _width*_height*4;
            _sizeInBytesForMmx = (_sizeInBytes/8)*8;
                
            _dataPtr = TpsGraphWrapper.InitSprite(width, height, 4);
       }

        public Sprite(BitmapSource source) : this((uint) source.PixelWidth, (uint) source.PixelHeight)
        {
            var bmp = new FormatConvertedBitmap(source, PixelFormats.Bgr32, null, 0);
            var pixels = new uint[_width*_height];
            bmp.CopyPixels(pixels, (int) (_width*4), 0);
            SetRawData(pixels);
        }

        #endregion

        #region public methods

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void SetPixel(uint x, uint y, uint color)
        {
            if (x >= Width || y >= Height) return;

            SetLayer();
                
            TpsGraphWrapper.SetPixel(x, y, color);
        }

        public uint GetPixel(uint x, uint y)
        {
            SetLayer();

            return TpsGraphWrapper.GetPixel(x, y);
        }

        public void Fill(uint color)
        {
            SetLayer();

            TpsGraphWrapper.ClearScreen(color);
            //TpsGraphWrapper.MemSet(RawDataPtr, 0, _sizeInBytes);
        }

        public void PutSprite(uint x, uint y, Sprite sprite, BlendMode? blendMode)
        {
            SetLayer();
            
            if (blendMode == null)
            {
                TpsGraphWrapper.PutSprite(x, y, TransparentColor, sprite._dataPtr);
            }
            else
            {
                TpsGraphWrapper.PutSpriteBlend(x, y, sprite._dataPtr, (byte) blendMode);
            }
        }

        /// <summary>
        /// Gets the raw image data (first bytes are sprite signature).
        /// </summary>
        public unsafe uint[] GetRawData()
        {
            var result = new uint[_width*_height];
            fixed (uint* dstPtr = &result[0]) CopyPixelsTo((uint) dstPtr);
            return result;
        }

        public unsafe byte[] GetRawByteData()
        {
            var result = new byte[_sizeInBytes];
            fixed (byte* dstPtr = &result[0]) CopyPixelsTo((uint) dstPtr);
            return result;
        }

        public unsafe void SetRawData(uint[] rawData)
        {
            if (rawData.Length != _width*_height)
                throw new ArgumentException("rawData size mismatch; should be width*height");

            fixed (uint* srcPtr = &rawData[0])
                TpsGraphWrapper.CopyMemory(_dataPtr + SizeofSpriteHeader, (uint) srcPtr, _sizeInBytes);
        }

        public BitmapSource GetBitmapSource2()
        {
            if (_writeableBitmap == null)
            {
                _writeableBitmap = new WriteableBitmap((int) Width, (int) Height, 96, 96, PixelFormats.Bgr32, null);
            }

            _writeableBitmap.Lock();
            var dstPtr = (uint) _writeableBitmap.BackBuffer;
            CopyPixelsTo(dstPtr);
            _writeableBitmap.AddDirtyRect(new Int32Rect(0, 0, _writeableBitmap.PixelWidth, _writeableBitmap.PixelHeight));
            _writeableBitmap.Unlock();

            return _writeableBitmap;
        }

        public unsafe BitmapSource GetOrUpdateBitmapSource()
        {
            if (_bitmapSourcePtr == null)
            {
                var stride = Width*4; // Size of "horizontal row"
                var sectionSize = _sizeInBytes;
                var section = NativeMethods.CreateFileMapping(NativeMethods.INVALID_HANDLE_VALUE, IntPtr.Zero, (int)NativeMethods.PAGE_READWRITE, 0, (int)sectionSize, null);
                _bitmapSource = Imaging.CreateBitmapSourceFromMemorySection(section, (int)Width, (int)Height, PixelFormats.Bgr32, (int)stride, 0);
                _bitmapSourcePtr = (uint)NativeMethods.MapViewOfFile(section, NativeMethods.FILE_MAP_ALL_ACCESS, 0, 0, sectionSize).ToPointer();
                NativeMethods.CloseHandle(section);
                NativeMethods.UnmapViewOfFile(section);

                // We can use mapped memory directly to draw, but I could not find a way to avoid flickering in this case. So we sprite memory as a double buffer.
                //var sectionSize = _sizeInBytes + SizeofSpriteHeader;
                //var section = NativeMethods.CreateFileMapping(NativeMethods.INVALID_HANDLE_VALUE, IntPtr.Zero, (int) NativeMethods.PAGE_READWRITE, 0, (int) sectionSize, null);
                //_bitmapSource = Imaging.CreateBitmapSourceFromMemorySection(section, (int)Width, (int)Height, PixelFormats.Bgr32, (int)stride, (int) SizeofSpriteHeader);
                //_bitmapSourcePtr = (uint)NativeMethods.MapViewOfFile(section, NativeMethods.FILE_MAP_ALL_ACCESS, 0, 0, sectionSize).ToPointer();
                //NativeMethods.CloseHandle(section);
                //NativeMethods.UnmapViewOfFile(section);
                //TpsGraphWrapper.CopyMemory((uint) _bitmapSourcePtr, _dataPtr, _sizeInBytes + SizeofSpriteHeader);
                //TpsGraphWrapper.FreeSprite(_dataPtr);
                //_shouldFreeSprite = false;
                //_dataPtr = (uint)_bitmapSourcePtr;

            }

            CopyPixelsTo((uint) _bitmapSourcePtr);
            return _bitmapSource;
        }

        public void CopyPixelsTo(uint targetPtr)
        {
            //TpsGraphWrapper.MemCopyMmx(_dataPtr + SizeofSpriteHeader, targetPtr, _sizeInBytesForMmx);
            //TpsGraphWrapper.MemCopy(_dataPtr + SizeofSpriteHeader, targetPtr, _sizeInBytesForMmx);
            TpsGraphWrapper.CopyMemory(targetPtr, _dataPtr + SizeofSpriteHeader, _sizeInBytes);

            // http://stackoverflow.com/questions/14834108/speed-copy-bitmap-data-into-array-or-work-with-it-directly
            // http://stackoverflow.com/questions/13511661/create-bitmap-from-double-two-dimentional-array
            // http://stackoverflow.com/questions/8104461/pixelformat-format32bppargb-seems-to-have-wrong-byte-order
            // http://code.google.com/p/renderterrain/source/browse/trunk/Utilities/FastBitmap.cs?r=18
            // Yeah! http://msdn.microsoft.com/en-us/library/system.windows.interop.imaging.createbitmapsourcefrommemorysection.aspx
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return string.Format("{0}x{1}, {2} bytes", Width, Height,
                _sizeInBytes + TpsGraphWrapper.GetSizeOfSpriteHeader());
        }

        #endregion

        //public void CopyFrom(Sprite other)
        //{
        //    if (other.Width != Width || other.Height != Height || other == this) throw new ArgumentException();

        //    TpsGraphWrapper.MemCopyMmx(other._dataPtr + SizeofSpriteHeader, _dataPtr + SizeofSpriteHeader, _sizeInBytesForMmx);
        //}

        #region public properties and indexers

        public uint Width
        {
            get { return _width; }
        }

        public uint Height
        {
            get { return _height; }
        }

        public uint RawDataPtr
        {
            get { return _dataPtr + TpsGraphWrapper.GetSizeOfSpriteHeader(); }
        }

        #endregion

        #region protected methods

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                // free managed resources
            }

            // free native resources if there are any.
            if (!_disposed)
            {
                _disposed = true;
                if (_shouldFreeSprite)
                {
                    TpsGraphWrapper.FreeSprite(_dataPtr);
                }
            }
        }

        #endregion

        #region private methods

        private void SetLayer()
        {
            TpsGraphWrapper.SetLayer(_dataPtr);
        }

        #endregion

        #region private fields

        private readonly uint _dataPtr;
        private bool _shouldFreeSprite = true;
        private readonly uint _height;
        private readonly uint _width;
        private bool _disposed;
        private uint? _bitmapSourcePtr;
        private BitmapSource _bitmapSource;
        private WriteableBitmap _writeableBitmap;
        private readonly uint _sizeInBytes;
        private readonly uint _sizeInBytesForMmx;
        private static uint? _sizeofSpriteHeader;

        #endregion

        /// <summary>
        /// Finalizes an instance of the <see cref="Sprite"/> class.
        /// </summary>
        ~Sprite()
        {
            // Finalizer calls Dispose(false)
            Dispose(false);
        }
    }
}