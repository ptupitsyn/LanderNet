using System;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;

namespace LanderNet.Game.Stages
{
    internal class SurvivalStage : IStage
    {
        public SurvivalStage()
        {
            AsteroidsPerSecond = 2;
            CratesPerSecond = 0.7;
        }

        public double AsteroidsPerSecond
        {
            get; private set;
        }

        public double CratesPerSecond
        {
            get; private set;
        }

        public string Description
        {
            get { return "Survive as long as you can, score points by shooting asteroids and collecting bonuses."; }
        }

        public string Name
        {
            get { return "Survival"; }
        }

        public int GetAsteroidScore(IGameObject asteroid)
        {
            if (asteroid == null)
                throw new ArgumentNullException();

            var health = asteroid.GetComponent<HealthComponent>();

            return (int) (health != null ? health.MaxHealth : 0);
        }

        public int GetCrateScore(IGameObject crate)
        {
            if (crate == null)
                throw new ArgumentNullException();

            return 500;
        }

        public void Update(double gameTime)
        {
            AsteroidsPerSecond += gameTime / 5;
        }
    }
}