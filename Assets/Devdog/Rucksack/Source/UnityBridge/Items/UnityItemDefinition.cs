using System;
using Devdog.General2.Localization;
using Devdog.Rucksack.Currencies;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    // [CreateAssetMenu(menuName = RucksackConstants.AddPath + "Unity Item Definition")]
    public partial class UnityItemDefinition : ScriptableObject, IUnityItemDefinition, IWeight, IIdentifiable, ISerializationCallbackReceiver
    {
        public virtual bool isPersistent
        {
            get { return true; }
        }

        [SerializeField]
        private UnityItemDefinition _parent;
        public IItemDefinition parent
        {
            get { return _parent; }
            protected set
            {
                if (value != null && value?.GetType() != GetType())
                {
                    throw new ArgumentException($"Given value is of type {value?.GetType().Name} this item definition is of type {GetType().Name}. Parent item definitions have to be of the same type as the children. Use C# class inheritance instaed.");
                }
                
                _parent = value as UnityItemDefinition;
            }
        }

        [SerializeField]
        private SerializedGuid _guid;
        public Guid ID
        {
            get { return _guid.guid; }
            protected set { _guid.guid = value; }
        }

        [SerializeField]
        private LocalizedString _name = new LocalizedString();
        public new virtual string name
        {
            get { return this.GetValue(t => t._name.ToString()); }
            set { _name.message = value; }
        }

        [SerializeField]
        private LocalizedString _description = new LocalizedString();
        public virtual string description
        {
            get { return this.GetValue(t => t._description.ToString()); }
            set { _description.message = value; }
        }
        
        [SerializeField]
        private int _maxStackSize = 1;
        public virtual int maxStackSize
        {
            get { return this.GetValue(t => t._maxStackSize, 1); }
            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), $"{nameof(maxStackSize)} can not be 0 or negative.");
                }
                
                _maxStackSize = value;
            }
        }

        [SerializeField]
        private UnityShape2D _layoutShape = new UnityShape2D(new bool[,]{ { true } });
        public IShape2D layoutShape
        {
            get { return this.GetValue(t => t._layoutShape, new UnityShape2D(new bool[,]{ { true } })); }
//            set { _layoutShape = value; }
        }

        [SerializeField]
        private UnityCurrencyDecorator[] _buyPrice = new UnityCurrencyDecorator[0];
        private CurrencyDecorator<double>[] _buyPriceDec;
        public CurrencyDecorator<double>[] buyPrice
        {
            get { return this.GetValue(t => t._buyPriceDec); }
            set { _buyPriceDec = value; }
        }

        [SerializeField]
        private UnityCurrencyDecorator[] _sellPrice = new UnityCurrencyDecorator[0];
        private CurrencyDecorator<double>[] _sellPriceDec;
        public CurrencyDecorator<double>[] sellPrice
        {
            get { return this.GetValue(t => t._sellPriceDec); }
            set { _sellPriceDec = value; }
        }

        [SerializeField]
        private float _weight;
        public virtual float weight
        {
            get { return this.GetValue(t => t._weight); }
            set { _weight = value; }
        }

        [SerializeField]
        private Sprite _icon;
        public Sprite icon
        {
            get { return this.GetUnityObjectValue(t => t._icon); }
            set { _icon = value; }
        }

        /// <summary>
        /// The model used for dropping / placing it in the world.
        /// </summary>
        [SerializeField]
        private GameObject _worldModel;
        public virtual GameObject worldModel
        {
            get { return this.GetUnityObjectValue(t => t._worldModel); }
            set { _worldModel = value; }
        }


        public static bool operator ==(UnityItemDefinition left, UnityItemDefinition right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(UnityItemDefinition left, UnityItemDefinition right)
        {
            return !Equals(left, right);
        }


//        public static UnityItemDefinition CreateInstance(Guid itemID, UnityItemDefinition parent = null)
//        {
//            var inst = CreateInstance<UnityItemDefinition>();
//            inst.ID = itemID;
//            inst.parent = parent;
//            inst.isPersistent = false;
//            
//            return inst;
//        }
        
        public virtual void OnBeforeSerialize()
        { }

        public virtual void OnAfterDeserialize()
        {
            // MoveAutoBoxed the UnityCurrencyDecorators into the interface required decs...
            if (_buyPrice.Length > 0)
            {
                _buyPriceDec = new CurrencyDecorator<double>[_buyPrice.Length];
                for (var i = 0; i < _buyPrice.Length; i++)
                {
                    _buyPriceDec[i] = _buyPrice[i].ToNativeDecorator();
                }
            }
            
            if (_sellPrice.Length > 0)
            {
                _sellPriceDec = new CurrencyDecorator<double>[_sellPrice.Length];
                for (var i = 0; i < _sellPrice.Length; i++)
                {
                    _sellPriceDec[i] = _sellPrice[i].ToNativeDecorator();
                }
            }
        }
        
        public bool Equals(IItemDefinition other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return ID.Equals(other.ID);
        }
        
        public bool Equals(IUnityItemDefinition other)
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
            return Equals((IItemDefinition) obj);
        }

        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }
        
        public virtual object Clone()
        {
            var clone = (UnityItemDefinition) MemberwiseClone();
            clone.ID = Guid.NewGuid();
//            clone.isPersistent = false;

            return clone;
        }

        /// <summary>
        /// Reset the ID of this item definition. 
        /// <remarks>This can cause ID's from getting desynced! Use with caution!</remarks>
        /// </summary>
        /// <param name="guid"></param>
        public void ResetID(System.Guid guid)
        {
            ID = guid;
        }

        public void ResetLocalizedStrings()
        {
            var tmpMessage = this._name.message;
            this._name = new LocalizedString();
            this._name.message = tmpMessage;

            tmpMessage = this._description.message;
            this._description = new LocalizedString();
            this._description.message = tmpMessage;
        }
    }
}