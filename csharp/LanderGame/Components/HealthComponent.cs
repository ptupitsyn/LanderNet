namespace LanderNet.Game.Components
{
    /// <summary>
    /// Represents health or damage.
    /// </summary>
    public class HealthComponent : ITimedComponent
    {
        public HealthComponent()
        {
            Health = Maximum = 10;
        }

        public double Health
        {
            get { return _health; }
            set
            {
                _health = value;

                if (_health > MaxHealth)
                {
                    MaxHealth = _health;
                }
            }
        }

        public double MaxHealth
        {
            get; private set;
        }

        public double Maximum { get; set; }

        public double RegenerationSpeed { get; set; }

        public void UpdateOnTime(double seconds)
        {
            if (Health < 0) Health = 0;
            Health += RegenerationSpeed*seconds;
            if (Health > Maximum) Health = Maximum;
        }

        private double _health;
    }
}