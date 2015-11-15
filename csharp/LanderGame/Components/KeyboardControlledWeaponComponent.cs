using System;
using System.Windows.Input;

namespace LanderNet.Game.Components
{
    public class KeyboardControlledWeaponComponent : ITimedComponent
    {
        public KeyboardControlledWeaponComponent(Action onWeaponFire, Key key, double ammoRegenerationSpeed = 5, double maxAmmo = 50)
        {
            WeaponFire += onWeaponFire;
            _key = key;
            _ammo.RegenerationSpeed = ammoRegenerationSpeed;
            _ammo.Maximum = _ammo.Health = maxAmmo;
        }

        public event Action WeaponFire;

        public double Ammo => _ammo.Health;

        public double ReloadTime { get; set; }

        public void RestoreAmmo()
        {
            _ammo.Health = _ammo.MaxHealth;
        }

        public void UpdateOnTime(double secondsPassed)
        {
            _reloadSecondsPassed += secondsPassed;
            var isKeyDown = Keyboard.IsKeyDown(_key);
            if (_reloadSecondsPassed >= ReloadTime && isKeyDown && _ammo.Health > 0)
            {
                _reloadSecondsPassed = 0;
                _ammo.Health -= 1;
                OnWeaponFire();
            }

            if (!isKeyDown)
            {
                _ammo.UpdateOnTime(secondsPassed);
            }
        }

        private void OnWeaponFire()
        {
            WeaponFire?.Invoke();
        }

        private readonly HealthComponent _ammo = new HealthComponent{Health = 10, Maximum = 50};
        private readonly Key _key;
        private double _reloadSecondsPassed = double.MaxValue;
    }
}