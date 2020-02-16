using System;

namespace Devdog.Rucksack.Items
{
    public static class ItemFactory
    {
        public static IItemFactoryBinder binder = new DefaultItemFactoryBinder();
        public static void Bind<TDefinition, TInstance>()
            where TDefinition: IItemDefinition
            where TInstance: IItemInstance
        {
            binder.Bind<TDefinition, TInstance>();
        }

        public static bool ContainsBinding<TDefinition>()
            where TDefinition: IItemDefinition
        {
            return binder.ContainsBinding<TDefinition>();
        }

        public static void RemoveBinding<TDefinition>()
            where TDefinition: IItemDefinition
        {
            binder.RemoveBinding<TDefinition>();
        }

        public static void RemoveAllBindings()
        {
            binder.RemoveAllBindings();
        }

        public static IItemInstance CreateInstance<TDefinition>(TDefinition itemDefinition, System.Guid guid)
            where TDefinition: IItemDefinition
        {
            return binder.CreateInstance<TDefinition>(itemDefinition, guid);
        }
    }
}