using System;

namespace LanderNet.Game.Components
{
    public class ExpirationComponent
    {
        #region public constructors

        public ExpirationComponent(DateTime expirationTime)
        {
            ExpirationTime = expirationTime;
        }

        #endregion

        #region public properties and indexers

        public bool IsExpired => ExpirationTime < DateTime.Now;

        public DateTime ExpirationTime { get; }

        #endregion

        #region private fields

        #endregion
    }
}