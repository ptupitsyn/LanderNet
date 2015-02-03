using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    public class Rocket : GameObjectBase
    {
        public Rocket()
        {
            AddComponent(new SizeComponent {Width = 20, Height = 50});
            AddComponent(new HealthComponent { Health = 100 });
        }
    }
}