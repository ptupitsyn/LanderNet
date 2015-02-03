using System;

namespace LanderNet.Game.Components
{
    public class LinearMovementComponent : ITimedComponent
    {
        #region public constructors

        public LinearMovementComponent(PositionComponent position)
        {
            if (position == null) throw new ArgumentNullException();
            _position = position;
        }

        #endregion

        #region public methods

        /// <summary>
        /// Updates object in game loop according to time that has passed.
        /// </summary>
        /// <param name="seconds">The time passed, in seconds.</param>
        public void UpdateOnTime(double seconds)
        {
            if (Position == null) return;

            XSpeed += XAcceleration*seconds;
            YSpeed += YAcceleration*seconds;
            Position.X += XSpeed*seconds;
            Position.Y += YSpeed*seconds;
        }

        #endregion

        #region public properties and indexers

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        public PositionComponent Position
        {
            get { return _position; }
        }

        /// <summary>
        /// Gets or sets the X speed, in units per second. 
        /// </summary>
        public double XSpeed
        {
            get { return _xSpeed; }
            set
            {
                _xSpeed = value;
                ValidateXSpeed();
            }
        }

        /// <summary>
        /// Gets or sets the Y speed, in units per second. 
        /// </summary>
        public double YSpeed
        {
            get { return _ySpeed; }
            set
            {
                _ySpeed = value;
                ValidateYSpeed();
            }
        }

        /// <summary>
        /// Gets or sets the minimum Y speed.
        /// </summary>
        public double YSpeedMin
        {
            get { return _ySpeedMin; }
            set
            {
                _ySpeedMin = value;
                ValidateYSpeed();
            }
        }

        public double YSpeedMax
        {
            get { return _ySpeedMax; }
            set
            {
                _ySpeedMax = value;
                ValidateYSpeed();
            }
        }

        public double XSpeedMax
        {
            get { return _xSpeedMax; }
            set
            {
                _xSpeedMax = value;
                ValidateXSpeed();
            }
        }

        public double XAcceleration { get; set; }
        public double YAcceleration { get; set; }

        #endregion

        #region private methods

        private void ValidateXSpeed()
        {
            if (XSpeed > XSpeedMax) XSpeed = XSpeedMax;
        }

        private void ValidateYSpeed()
        {
            if (YSpeed < YSpeedMin) YSpeed = YSpeedMin;
            if (YSpeed > YSpeedMax) YSpeed = YSpeedMax;
        }

        #endregion

        #region private fields

        private readonly PositionComponent _position;
        private double _ySpeedMin = double.MinValue;
        private double _ySpeed;
        private double _xSpeed;
        private double _ySpeedMax = double.MaxValue;
        private double _xSpeedMax = double.MaxValue;

        #endregion
    }
}