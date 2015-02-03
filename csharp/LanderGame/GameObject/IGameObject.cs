using System;
using System.Collections.Generic;

namespace LanderNet.Game.GameObject
{
    public interface IGameObject
    {
        /// <summary>
        /// Updates object in game loop according to time that has passed.
        /// </summary>
        /// <param name="secondsPassed">The time passed, in seconds.</param>
        void UpdateOnTime(double secondsPassed);
        
        /// <summary>
        /// Gets the game object components (such as position, rotation, size, etc).
        /// </summary>
        IEnumerable<object> Components { get; }

        /// <summary>
        /// Adds the component to the GameObject.
        /// </summary>
        /// <param name="component">The component to add.</param>
        void AddComponent(object component);

        object GetComponentByType(Type type);
    }
}
