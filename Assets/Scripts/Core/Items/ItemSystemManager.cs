using System;
using System.Collections.Generic;
using System.Linq;
using VContainer;

namespace Anomalus.Items
{
    public sealed class ItemSystemManager
    {
        private readonly IObjectResolver _objectResolver;
        private readonly Dictionary<Type, ItemSystem> _systems = new();

        public ItemSystemManager(IObjectResolver objectResolver)
        {
            _objectResolver = objectResolver;

            var baseType = typeof(ItemSystem);
            var types = GetType().Assembly.GetTypes().Where(x => baseType.IsAssignableFrom(x)).ToList();
            foreach (var systemType in GetType().Assembly.GetTypes().Where(x => baseType.IsAssignableFrom(x) && !x.IsAbstract))
            {
                var system = Activator.CreateInstance(systemType) as ItemSystem;
                _objectResolver.Inject(system);
                _systems.Add(system.ItemType, system);
            }
        }

        public ItemSystem<T> Get<T>() where T : ItemConfig
        {
            return Get(typeof(T)) as ItemSystem<T>;
        }

        public ItemSystem Get(Type itemType)
        {
            return _systems.GetValueOrDefault(itemType);
        }
        public ItemSystem Get(ItemConfig config)
        {
            return Get(config.GetType());
        }

        public ItemSystem Get(ItemStack stack)
        {
            return Get(stack.ItemType);
        }
    }
}
