namespace Devdog.Rucksack.Items
{
    public interface IItemFactoryBinder
    {
        void Bind<TDefinition, TInstance>()
            where TDefinition: IItemDefinition
            where TInstance: IItemInstance;

        bool ContainsBinding<TDefinition>()
            where TDefinition : IItemDefinition;

        void RemoveBinding<TDefinition>()
            where TDefinition : IItemDefinition;

        void RemoveAllBindings();

        IItemInstance CreateInstance<TDefinition>(TDefinition itemDefinition, System.Guid guid)
            where TDefinition : IItemDefinition;
    }
}