using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    public class Crate : GameObjectBase
    {
        public Crate()
        {
            AddComponent(new PositionComponent());
            AddComponent(new SizeComponent { Width = 100, Height = 100 });
        }
    }
}