using System;
using System.Collections.Generic;
using System.Linq;
using LanderNet.Game.Components;

namespace LanderNet.Game.GameObject
{
    public class GameObjectBase : IGameObject
    {
        #region public methods

        public void UpdateOnTime(double secondsPassed)
        {
            foreach (var component in Components.OfType<ITimedComponent>())
            {
                component.UpdateOnTime(secondsPassed);
            }
        }

        public void AddComponent(object component)
        {
            if (component == null) return;
            _components.Add(component);
            _componentsTypeMap[component.GetType()] = component;
        }

        public object GetComponentByType(Type type)
        {
            object result;
            return _componentsTypeMap.TryGetValue(type, out result) ? result : null;
        }

        #endregion

        #region public properties and indexers

        public IEnumerable<object> Components
        {
            get { return _components; }
        }

        #endregion

        #region private fields

        private readonly List<object> _components = new List<object>();
        private readonly Dictionary<Type, object> _componentsTypeMap = new Dictionary<Type, object>();

        #endregion
    }
}