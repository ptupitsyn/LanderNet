using System;
using System.Windows.Input;

namespace LanderNet.Game.Components
{
    public class KeyboardControlledMovementComponent : ITimedComponent
    {
        private readonly LinearMovementComponent _movementComponent;
        private double _accelerationPerSecond;

        public KeyboardControlledMovementComponent(LinearMovementComponent movementComponent, double accelerationPerSecond)
        {
            if (movementComponent == null) throw new ArgumentNullException();

            _movementComponent = movementComponent;
            _accelerationPerSecond = accelerationPerSecond;
        }


        public bool IsAccelerating =>
            Keyboard.IsKeyDown(Key.Up) ||
            Keyboard.IsKeyDown(Key.Down) || 
            Keyboard.IsKeyDown(Key.Left) || 
            Keyboard.IsKeyDown(Key.Right);

        #region ITimedComponent Members

        public void UpdateOnTime(double seconds)
        {
            var acceleration = _accelerationPerSecond * seconds;
            if (Keyboard.IsKeyDown(Key.Up)) _movementComponent.YSpeed += acceleration;
            if (Keyboard.IsKeyDown(Key.Down)) _movementComponent.YSpeed -= acceleration;
            if (Keyboard.IsKeyDown(Key.Right)) _movementComponent.XSpeed += acceleration;
            if (Keyboard.IsKeyDown(Key.Left)) _movementComponent.XSpeed -= acceleration;
        }

        #endregion
    }
}