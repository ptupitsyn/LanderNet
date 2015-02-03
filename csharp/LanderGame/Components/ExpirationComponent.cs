using System;

namespace LanderNet.Game.Components
{
    public class ExpirationComponent
    {
        #region public constructors

        public ExpirationComponent(DateTime expirationTime)
        {
            _expirationTime = expirationTime;
        }

        #endregion

        #region public properties and indexers

        public bool IsExpired
        {
            get { return ExpirationTime < DateTime.Now; }
        }

        public DateTime ExpirationTime
        {
            get { return _expirationTime; }
        }

        #endregion

        #region private fields

        private readonly DateTime _expirationTime;

        #endregion
    }
}