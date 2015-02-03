using System;
using System.Collections.Generic;
using System.Linq;
using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    /// <summary>
    /// Extension methods for IGameObject.
    /// </summary>
    public static class GameObjectExtensions
    {
        #region public static methods

        public static IEnumerable<T> GetComponents<T>(this IGameObject gameObject)
        {
            if (gameObject == null) throw new ArgumentNullException();
            return gameObject.Components.OfType<T>();
        }

        public static T GetComponent<T>(this IGameObject gameObject)
        {
            if (gameObject == null) throw new ArgumentNullException();
            return (T) gameObject.GetComponentByType(typeof (T));
        }

        /// <summary>
        /// Adds the component when no components of this type exist.
        /// </summary>
        /// <param name="gameObject">The game object.</param>
        /// <param name="component">The component.</param>
        /// <returns>Returns existing component, if any. Otherwise, returns provided component..</returns>
        public static T AddComponentUniqueType<T>(this IGameObject gameObject, T component) where T : class
        {
            if (gameObject == null || component == null) throw new ArgumentNullException();

            var existingComponent = gameObject.GetComponent<T>();
            if (existingComponent != null) return existingComponent;

            gameObject.AddComponent(component);
            return component;
        }

        public static bool IsExpired(this IGameObject gameObject)
        {
            var expiration = gameObject.GetComponent<ExpirationComponent>();
            return expiration != null && expiration.IsExpired;
        }

        public static T SetPosition<T>(this T gameObject, double x, double y) where T : IGameObject
        {
            var position = gameObject.AddComponentUniqueType(new PositionComponent());
            position.X = x;
            position.Y = y;
            return gameObject;
        }

        public static T SetSpeed<T>(this T gameObject, double xSpeed, double ySpeed, double? xAcceleration = null, double? yAcceleration = null, double? xSpeedMax = null, double? ySpeedMax = null) where T : IGameObject
        {
            var movement = gameObject.AddComponentUniqueType(new LinearMovementComponent(gameObject.AddComponentUniqueType(new PositionComponent())));
            movement.XSpeed = xSpeed;
            movement.YSpeed = ySpeed;
            movement.XAcceleration = xAcceleration ?? movement.XAcceleration;
            movement.YAcceleration = yAcceleration ?? movement.YAcceleration;
            movement.XSpeedMax = xSpeedMax ?? movement.XSpeedMax;
            movement.YSpeedMax = ySpeedMax ?? movement.YSpeedMax;
            return gameObject;
        }

        public static double GetHealth<T>(this T gameObject) where T : IGameObject
        {
            var health = gameObject.GetComponent<HealthComponent>();
            return health == null ? 0 : health.Health;
        }

        public static void AddHealth<T>(this T gameObject, double health) where T : IGameObject
        {
            var healthComponent = gameObject.GetComponent<HealthComponent>();
            if (healthComponent != null)
            {
                healthComponent.Health += health;
            }
        }

        #endregion
    }
}