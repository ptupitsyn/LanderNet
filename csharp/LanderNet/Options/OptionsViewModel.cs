using LanderNet.UI.Util;

namespace LanderNet.UI.Options
{
    internal class OptionsViewModel : NotifierBase
    {
        /// <summary>
        /// Gets or sets the asteroids count.
        /// </summary>
        public int AsteroidsCount
        {
            get { return _asteroidsCount; }
            set
            {
                _asteroidsCount = value;
                OnPropertyChanged("AsteroidsCount");
            }
        }

        /// <summary>
        /// Gets or sets the limit for debris per explosion.
        /// </summary>
        public int DebrisLimit
        {
            get { return _debrisLimit; }
            set
            {
                _debrisLimit = value;
                OnPropertyChanged("DebrisLimit");
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether [enable sound].
        /// </summary>
        public bool EnableSound
        {
            get { return _enableSound; }
            set
            {
                _enableSound = value;
                OnPropertyChanged("EnableSound");
            }
        }

        private int _asteroidsCount;
        private int _debrisLimit;
        private bool _enableSound;
    }
}