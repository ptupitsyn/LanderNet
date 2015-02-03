namespace LanderNet.Game.Components
{
    public interface ITimedComponent
    {
        #region public methods

        /// <summary>
        /// Updates object in game loop according to time that has passed.
        /// </summary>
        /// <param name="seconds">The time passed, in seconds.</param>
        void UpdateOnTime(double seconds);

        #endregion
    }
}