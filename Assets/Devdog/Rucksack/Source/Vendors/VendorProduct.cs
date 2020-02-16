using System;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Vendors
{  
    public sealed class VendorProduct<T> : IVendorProduct<T>, IEquatable<VendorProduct<T>>
        where T: IIdentifiable, IStackable, IEquatable<T>, ICloneable
    {
        public ICollectionEntry collectionEntry
        {
            get
            {
                return (item as ICollectionSlotEntry)?.collectionEntry;
            }
            set
            {
                var entry = item as ICollectionSlotEntry;
                if (entry != null)
                {
                    entry.collectionEntry = value;
                }
            }
        }

        public Guid ID
        {
            get { return item.ID; }
        }
        
        public T item { get; }
        public CurrencyDecorator<double>[] buyPrice { get; }
        public CurrencyDecorator<double>[] sellPrice { get; }

        public int maxStackSize
        {
            get { return item.maxStackSize; }
        }
        
        public VendorProduct(T item, CurrencyDecorator<double>[] buyPrice, CurrencyDecorator<double>[] sellPrice)
        {
            this.item = item;
            this.buyPrice = buyPrice ?? new CurrencyDecorator<double>[0];
            this.sellPrice = sellPrice ?? new CurrencyDecorator<double>[0];
        }

        public static implicit operator T(VendorProduct<T> product)
        {
            return product.item;
        }

        public bool Equals(IVendorProduct<T> other)
        {
            if (Equals(null, other)) return false;
            if (item.Equals(other.item) == false) return false;

            if (buyPrice.Length != other.buyPrice.Length) return false;
            if (sellPrice.Length != other.sellPrice.Length) return false;

            for (int i = 0; i < buyPrice.Length; i++)
            {
                if (buyPrice[i].Equals(other.buyPrice[i]) == false)
                {
                    return false;
                }
            }
            
            for (int i = 0; i < sellPrice.Length; i++)
            {
                if (sellPrice[i].Equals(other.sellPrice[i]) == false)
                {
                    return false;
                }
            }

            return true;
        }

        public bool Equals(VendorProduct<T> other)
        {
            return Equals((IVendorProduct<T>) other);
        }
        
        public override string ToString()
        {
            return $"{item} Buy: {buyPrice} sellPrice {sellPrice}";
        }
        
        public object Clone()
        {
            var prod = new VendorProduct<T>((T) item.Clone(), buyPrice, sellPrice);
//            prod.collectionEntry = collectionEntry;

            return prod;
//            return MemberwiseClone();
        }
    }
}