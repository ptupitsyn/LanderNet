namespace LanderNet.Game
{
    /// <summary>
    /// Describes game mechanics settings (speeds, health, etc).
    /// </summary>
    public interface ILanderGameSettings
    {
        double BulletSpeed { get; }
        double RocketAcceleration { get; }
    }
}
