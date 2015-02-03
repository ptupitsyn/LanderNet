using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    public class Bullet : GameObjectBase
    {
        public Bullet()
        {
            AddComponent(new SizeComponent {Width = 4, Height = 10});
            AddComponent(new HealthComponent {Health = 30});
        }
    }
}