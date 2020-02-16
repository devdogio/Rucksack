using System;
using System.Collections.Generic;
using System.Reflection;

namespace Devdog.Rucksack.Items
{
    public sealed class DefaultItemFactoryBinder : IItemFactoryBinder
    {
        private readonly Dictionary<Type, ConstructorInfo> _instanceLookupDictionary = new Dictionary<Type, ConstructorInfo>();
        private ILogger _logger;
        
        public DefaultItemFactoryBinder(ILogger logger = null)
        {
            _logger = logger ?? new Logger();
        }
        
        public void Bind<TDefinition, TInstance>() where TDefinition : IItemDefinition where TInstance : IItemInstance
        {
            foreach (var ci in typeof(TInstance).GetConstructors(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
            {
                var parameters = ci.GetParameters();
                if (parameters.Length == 2)
                {
                    if (parameters[0].ParameterType == typeof(System.Guid) && 
                        parameters[1].ParameterType.IsAssignableFrom(typeof(TDefinition))
                    )
                    {
                        _logger.Log($"Registering factory binding for type {typeof(TDefinition).Name}");
                        _instanceLookupDictionary[typeof(TDefinition)] = ci;
                        return;
                    }
                }
            }

            throw new ArgumentException($"Given item instance type ({typeof(TInstance).Name}) does not contain a constructor that takes (System.Guid guid, IItemDefinition itemDefinition)");
        }
        
        public bool ContainsBinding<TDefinition>()
            where TDefinition: IItemDefinition
        {
            return _instanceLookupDictionary.ContainsKey(typeof(TDefinition));
        }

        public void RemoveBinding<TDefinition>()
            where TDefinition: IItemDefinition
        {
            _logger.Log($"Removing factoring binding for type {typeof(TDefinition).Name}");
            _instanceLookupDictionary.Remove(typeof(TDefinition));
        }

        public void RemoveAllBindings()
        {
            _logger.Log($"Removing all factory bindings ({_instanceLookupDictionary.Count} count)");
            _instanceLookupDictionary.Clear();
        }

        public IItemInstance CreateInstance<TDefinition>(TDefinition itemDefinition, System.Guid guid)
            where TDefinition: IItemDefinition
        {
            ConstructorInfo info;
            if (_instanceLookupDictionary.TryGetValue(itemDefinition.GetType(), out info))
            {
                var inst = (IItemInstance) info.Invoke(new object[] { guid, itemDefinition });
                ItemRegistry.Register(inst.ID, inst);
                
                return inst;
            }
            
            throw new ArgumentException("No instance type found for definition " + itemDefinition.GetType());
        }
    }
}