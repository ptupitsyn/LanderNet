using System;
using System.Windows.Input;
using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    public class Spaceship : GameObjectBase
    {
        public Spaceship(Action onMachinegunFire, Action onRocketFire)
        {
            AddComponent(new PositionComponent());
            AddComponent(new LinearMovementComponent(this.GetComponent<PositionComponent>()) {YSpeedMin = 200});
            AddComponent(new KeyboardControlledMovementComponent(this.GetComponent<LinearMovementComponent>(), 1000));
            AddComponent(new SizeComponent {Width = 100, Height = 100});
            AddComponent(new HealthComponent{RegenerationSpeed = 1});
            AddComponent(_machinegun = new KeyboardControlledWeaponComponent(onMachinegunFire, Key.Space) { ReloadTime = 0.1 });
            AddComponent(_rockets = new KeyboardControlledWeaponComponent(onRocketFire, Key.LeftCtrl, 0.5, 5) {ReloadTime = 0.3});

            //Level = 10; // Cheat
        }

        public int Level { get; private set; }

        public int MachinegunLevel
        {
            get
            {
                if (Level > 2)
                    return 3;

                return Level > 0 ? 2 : 1;
            }
        }
        public double MachinegunAmmo => _machinegun.Ammo;

        public int RocketLevel
        {
            get
            {
                if (Level > 3)
                    return 2;

                return Level > 1 ? 1 : 0;
            }
        }
        
        public double RocketAmmo => _rockets.Ammo;

        public void ProcessPowerup(IGameObject powerup)
        {
            var health = this.GetComponent<HealthComponent>();
            if (health.Health < health.Maximum)
            {
                // Heal
                health.Health = health.Maximum;
            }
            else
            {
                // Upgrade level
                Level++;
            }
            
            // Fully restore ammo levels
            _machinegun.RestoreAmmo();
            _rockets.RestoreAmmo();
        }

        private readonly KeyboardControlledWeaponComponent _machinegun;
        private readonly KeyboardControlledWeaponComponent _rockets;
    }
}