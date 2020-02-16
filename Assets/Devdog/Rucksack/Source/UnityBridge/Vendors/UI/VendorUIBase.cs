using System;
using Devdog.Rucksack.Vendors;
using UnityEngine;

namespace Devdog.Rucksack.UI
{
    public abstract class VendorUIBase<T> : MonoBehaviour
        where T : class, IEquatable<T>, IIdentifiable, IStackable, ICloneable
    {
//        public abstract ICollection<IVendorProduct<IItemInstance>> collection { get; }
        
        public abstract IVendor<T> vendor { get; set; }
    }
}