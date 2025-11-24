using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using Zenject;

namespace Santelmo.Rinsurv
{
    public class WorldObjectInstaller : MonoInstaller
    {
        [SerializeField] private UnitPlayer _unitPlayer;
        [SerializeField] private GameObject _gameplaySystems;
        [SerializeField] private bool _isDebugMode;

        private readonly List<Type> _componentTypes = new();

        public override void InstallBindings()
        {
            var components = _gameplaySystems.GetComponentsInChildren<MonoBehaviour>()
                                             .Where(x => x != null)
                                             .Select(x => x.GetType())
                                             .ToArray();

            // Store dependency types here for binding and unbinding.
            _componentTypes.AddRange(new[] { typeof(UnitPlayer), typeof(PickupField) });
            _componentTypes.AddRange(components);

            Container.Bind(typeof(UnitPlayer), typeof(PickupField))
                     .FromComponentInNewPrefab(_unitPlayer)
                     .AsCached()
                     .NonLazy()
                     .IfNotBound();

            if (!_isDebugMode)
            {
                Container.Bind(components)
                         .FromComponentInNewPrefab(_gameplaySystems)
                         .AsCached()
                         .NonLazy()
                         .IfNotBound();
            }

            BindComponentsToAllContainers();
        }

        /// <summary>
        ///     Install all components to all containers.
        ///     This is to make dependencies accessible to UI scene context.
        /// </summary>
        private void BindComponentsToAllContainers()
        {
            foreach (var componentType in _componentTypes)
            {
                var instance = Container.TryResolve(componentType);
                Container.AncestorContainers.ForEach(x =>
                {
                    x.Bind(componentType)
                     .FromInstance(instance)
                     .AsCached()
                     .IfNotBound();
                });
            }
        }

        private void OnDestroy()
        {
            foreach (var componentType in _componentTypes)
            {
                Container.Unbind(componentType);
                Container.AncestorContainers.ForEach(x =>
                {
                    x.Unbind(componentType);
                });
            }
        }
    }
}
