using LanderNet.Game.GameObject;
using LanderNet.UI.Components;

namespace LanderNet.UI.GameObjects
{
    public static class GameObjectExtensions
    {
        public static double GetZIndex(this IGameObject gameObject)
        {
            var zIndex = gameObject.GetComponent<ZIndexComponent>();
            return zIndex == null ? 0 : zIndex.ZIndex;
        }

        public static T SetZIndex<T>(this T gameObject, int zIndex) where T : IGameObject
        {
            gameObject.AddComponentUniqueType(new ZIndexComponent()).ZIndex = zIndex;
            return gameObject;
        }
    }
}