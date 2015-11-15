using System;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using LanderNet.UI.Components;
using LanderNet.UI.Util;

namespace LanderNet.UI.GameObjects
{
    /// <summary>
    /// Represents debris (parts of exploded object).
    /// </summary>
    internal class Debris : GameObjectBase
    {
        #region public static methods

        public static Debris Create(IGameObject gameObject, double minScale = 0.1, double maxScale = 0.3)
        {
            if (maxScale < minScale) throw new ArgumentException("maxScale must be greater then or equal to minScale");
            if (minScale <= 0) throw new ArgumentException("Scale cannot be negative");

            var result = new Debris();
            var currentTime = DateTime.Now;
            result.AddComponent(new ExpirationComponent(currentTime.AddSeconds(3)));

            var speed = 200*RandomHelper.Instance.NextDouble() + 200;
            var angle = RandomHelper.Instance.NextDouble()*2*Math.PI;
            var sx = speed*Math.Cos(angle);
            var sy = speed*Math.Sin(angle);
            result.SetSpeed(sx, sy);

            // Calculate scale
            const int scaleStepCount = 3;
            var scaleStep = (maxScale - minScale) / scaleStepCount;
            var scale = minScale + scaleStep * RandomHelper.Instance.Next(scaleStepCount);

            // Centered position
            var pos = gameObject.GetComponent<PositionComponent>();
            var size = gameObject.GetComponent<SizeComponent>();
            if (pos != null && size != null)
            {
                result.SetPosition(pos.X + size.Width/2*(1 - scale), pos.Y + size.Height/2*(1 - scale));
            }

            foreach (var sprite in gameObject.GetComponents<AnimatedSpriteComponent>())
            {
                var clone = sprite.Clone(scale);
                result.AddComponent(clone);
            }

            return result;
        }

        #endregion

    }
}