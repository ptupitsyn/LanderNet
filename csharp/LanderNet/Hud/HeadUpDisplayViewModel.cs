using System;
using System.Linq;
using System.Windows.Media;
using LanderNet.Game;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using LanderNet.UI.Util;

namespace LanderNet.UI.Hud
{
    internal class HeadUpDisplayViewModel : NotifierBase
    {
        public HeadUpDisplayViewModel(LanderGame game)
        {
            if (game == null) throw new ArgumentNullException();
            _game = game;

            CompositionTarget.Rendering += CompositionTargetOnRendering;
        }

        public bool HasRockets { get; private set; }
        public double Health { get; private set; }
        public bool IsLevelChanged { get; private set; }
        public double MachinegunAmmo { get; private set; }
        public int MachinegunLevel { get; private set; }
        public double MaxHealth { get; private set; }
        public double RocketAmmo{ get; private set; }
        public int RocketLevel { get; private set; }
        public int Score { get; private set; }
        public bool IsGameOver { get; private set; }

        private void CompositionTargetOnRendering(object sender, EventArgs eventArgs)
        {
            // Do not redraw each frame
            _frameCount ++;
            if (_frameCount < 2)
                return;

            _frameCount = 0;

            var ship = _game.GameObjects.OfType<Spaceship>().First();
            if (ship == null)
                return;

            MachinegunLevel = ship.MachinegunLevel;
            MachinegunAmmo = ship.MachinegunAmmo;
            RocketLevel = ship.RocketLevel;
            RocketAmmo = ship.RocketAmmo;
            HasRockets = RocketLevel > 0;
            
            IsLevelChanged = _level != ship.Level;
            _level = ship.Level;

            var health = ship.GetComponent<HealthComponent>();
            if (health != null)
            {
                Health = health.Health;
                MaxHealth = health.Maximum;
            }

            // Score
            Score = _game.Score;

            // Game over
            IsGameOver = _game.IsGameOver;

            OnPropertyChanged(string.Empty);
        }

        private readonly LanderGame _game;
        private int _level = -1;
        private int _frameCount;
    }
}