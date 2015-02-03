using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    public class Asteroid : GameObjectBase
    {
        public Asteroid()
        {
            AddComponent(new PositionComponent());
            AddComponent(new SizeComponent { Width = 100, Height = 100 });
            AddComponent(new HealthComponent());
        }
    }
}