using System;

namespace TpsGraphNet
{
    public struct DisposableHelper : IDisposable
    {
        #region Fields and Constants

        private readonly Action _end;

        #endregion

        #region Constructors

        public DisposableHelper(Action end)
        {
            _end = end;
        }


        public DisposableHelper(Action begin, Action end)
        {
            _end = end;
            begin();
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _end();
        }

        #endregion
    }
}