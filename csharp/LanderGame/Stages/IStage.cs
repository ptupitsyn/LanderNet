using LanderNet.Game.GameObject;

namespace LanderNet.Game.Stages
{
    public interface IStage
    {
        string Description { get; }
        string Name { get; }
        double AsteroidsPerSecond { get; }
        double CratesPerSecond { get; }
        void Update(double gameTime);
        int GetCrateScore(IGameObject crate);
        int GetAsteroidScore(IGameObject asteroid);
    }
}