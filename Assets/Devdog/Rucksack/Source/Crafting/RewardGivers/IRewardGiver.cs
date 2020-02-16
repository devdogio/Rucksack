namespace Devdog.Rucksack.Crafting
{
    public interface IRewardGiver<T>
        where T : IIdentifiable
    {
        Result<bool> CanGiveRewards(IBlueprint<T> blueprint, CraftContext context);
        void GiveRewards(IBlueprint<T> blueprint, CraftContext context);
    }
}