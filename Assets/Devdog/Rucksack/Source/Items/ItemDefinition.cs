using System;
using Devdog.Rucksack.Currencies;

namespace Devdog.Rucksack.Items
{
    [System.Serializable]
    public class ItemDefinition : IIdentifiable, IStackable, IEquatable<ItemDefinition>, IItemDefinition
    {
        public virtual bool isPersistent
        {
            get { return false; }
        }

        private IItemDefinition _parent;
        public IItemDefinition parent
        {
            get { return _parent; }
            protected set
            {
                if (value != null && value.GetType() != GetType())
                {
                    throw new ArgumentException($"Given value is of type {value.GetType().Name} this item definition is of type {GetType().Name}. Parent item definitions have to be of the same type as the children. Use C# class inheritance instaed.");
                }
                
                _parent = value;
            }
        }

        public Guid ID { get; protected set; }

        private string _name;
        public virtual string name
        {
            get { return this.GetValue(t => t._name); }
            set { _name = value; }
        }
		

        private string _description;
        public virtual string description
        {
            get { return this.GetValue(t => t._description); }
            set { _description = value; }
        }

        private int _maxStackSize = 1;
        public virtual int maxStackSize
        {
            get { return this.GetValue(t => t._maxStackSize, 1); }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(maxStackSize)} can not be 0 or negative.");
                }
                
                _maxStackSize = value;
            }
        }

        private IShape2D _layoutShape = new SimpleShape2D(1, 1);
        public IShape2D layoutShape
        {
            get { return this.GetValue(t => t._layoutShape, new SimpleShape2D(1, 1)); }
            set { _layoutShape = value; }
        }

        private CurrencyDecorator<double>[] _buyPrice;
        public CurrencyDecorator<double>[] buyPrice
        {
            get { return this.GetValue(t => t._buyPrice); }
            set { _buyPrice = value; }
        }

        private CurrencyDecorator<double>[] _sellPrice;
        public CurrencyDecorator<double>[] sellPrice
        {
            get { return this.GetValue(t => t._sellPrice); }
            set { _sellPrice = value; }
        }


        public ItemDefinition()
            : this(Guid.Empty, null)
        { }

        public ItemDefinition(Guid ID)
            : this(ID, null)
        { }
        
        public ItemDefinition(Guid ID, ItemDefinition parent)
        {
            this.ID = ID;
            this.parent = parent;
        }
        
        public static bool operator ==(ItemDefinition left, ItemDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(ItemDefinition left, ItemDefinition right)
        {
            return !Equals(left, right);
        }
        
        
        
        
        public IItemInstance CreateInstance(Guid instanceGuid)
        {
            return null;
        }
        
        public bool Equals(IItemDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }
        
        public bool Equals(ItemDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ItemDefinition) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        
        public virtual object Clone()
        {
            var clone = (ItemDefinition) MemberwiseClone();
            clone.ID = Guid.NewGuid();

            return clone;
        }
    }
}
