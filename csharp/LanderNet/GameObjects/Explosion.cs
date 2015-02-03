using System;
using LanderNet.Game.Components;
using LanderNet.Game.GameObject;
using LanderNet.UI.Components;
using TpsGraphNet;

namespace LanderNet.UI.GameObjects
{
    public class Explosion : GameObjectBase
    {
        public Explosion(IGameObject explodedObject, double scale = 1, bool quick = false)
        {
            // Size
            AddComponent(new SizeComponent {Width = 100*scale, Height = 100*scale});

            // We need same position and speed
            AddComponent(explodedObject.GetComponent<PositionComponent>());
            AddComponent(explodedObject.GetComponent<LinearMovementComponent>());

            // Remove after animation completes
            AddComponent(new ExpirationComponent(DateTime.Now.AddSeconds(0.6)));

            // Add animation
            var animationStart = DateTime.UtcNow;
            var animationDuration = quick ? 0.2 : 0.4;
            AddComponent(new AnimatedSpriteComponent("X", 9, fps: 20, frameIncrement: 10, initialFrame: 10,
                reverseAnimation: false, decodePixelWidth: (int) (100*scale))
            {
                BlendMode = TpsGraphWrapper.BlendMode.Additive,
                VisibilityFunc = () => (DateTime.UtcNow - animationStart) < TimeSpan.FromSeconds(animationDuration)
            });

            if (!quick)
            {
                AddComponent(new AnimatedSpriteComponent("X", 9, fps: 20, frameIncrement: 10, initialFrame: 10,
                    reverseAnimation: true, decodePixelWidth: (int) (100*scale))
                {
                    BlendMode = TpsGraphWrapper.BlendMode.Additive,
                    VisibilityFunc = () => (DateTime.UtcNow - animationStart) > TimeSpan.FromSeconds(animationDuration)
                });
            }
        }
    }
}