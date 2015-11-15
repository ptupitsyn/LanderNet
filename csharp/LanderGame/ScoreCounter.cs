using System;
using LanderNet.Game.GameObject;

namespace LanderNet.Game
{
    /// <summary>
    /// Counts the game score based on player actions
    /// </summary>
    public class ScoreCounter
    {
        public ScoreCounter(LanderGame game)
        {
            if (game == null)
                throw new ArgumentNullException();

            _game = game;
            game.AsteroidShot += OnAsteroidShot;
            game.CrateCollected += OnCrateCollected;
        }

        public int Score => _score;

        public void Reset()
        {
            _score = 0;
        }

        void OnAsteroidShot(IGameObject asteroid)
        {
            // TODO: Multipliers/bonuses
            _score += _game.Stage.GetAsteroidScore(asteroid);
        }

        void OnCrateCollected(IGameObject crate)
        {
            _score += _game.Stage.GetCrateScore(crate);
        }

        private int _score;
        private readonly LanderGame _game;
    }
}
