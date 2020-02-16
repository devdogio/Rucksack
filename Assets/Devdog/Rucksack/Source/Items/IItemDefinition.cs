using System;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Items
{
    public interface IItemDefinition : IEquatable<IItemDefinition>, ICloneable, IShapeOwner2D
    {
        /// <summary>
        /// Is this item definition stored on disk / in a database somwewhere?
        /// <remarks>Note that item definitions that are not persistent have to be synced over the network in case of a multiplayer game.</remarks>
        /// </summary>
        bool isPersistent { get; }
        
        IItemDefinition parent { get; }
        Guid ID { get; }
       
        string name { get; }
        string description { get; }
        int maxStackSize { get; }

//        IShape2D layoutShape { get; }

        CurrencyDecorator<double>[] buyPrice { get; }
        CurrencyDecorator<double>[] sellPrice { get; }
    }
}