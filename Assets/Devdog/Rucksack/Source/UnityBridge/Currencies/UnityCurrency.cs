using System;
using Devdog.General2.Localization;
using UnityEngine;

namespace Devdog.Rucksack.Currencies
{
    [CreateAssetMenu(menuName = RucksackConstants.AddPath + "Currency")]
    public class UnityCurrency : ScriptableObject, IUnityCurrency, ICloneable
    {
        [SerializeField]
        private SerializedGuid _guid;
        public Guid ID
        {
            get { return _guid.guid; }
        }
        
        [SerializeField]
        private LocalizedString _name = new LocalizedString();
        public new string name
        {
            get { return _name.message; }
        }

        [SerializeField]
        private LocalizedString _tokenName = new LocalizedString();
        public string tokenName
        {
            get { return _tokenName.message; }
        }

        [SerializeField]
        private Sprite _icon;
        public Sprite icon
        {
            get { return _icon; }
        }

        [SerializeField]
        private int _decimals = 0;
        public int decimals
        {
            get { return _decimals; }
        }

        [SerializeField]
        private double _maxAmount = 999d;
        public double maxAmount
        {
            get { return _maxAmount; }
        }

        public ConversionTable<ICurrency, double> conversionTable { get; set; }

        public UnityCurrency()
        {
            conversionTable = new ConversionTable<ICurrency, double>();
        }
        
        public bool Equals(ICurrency other)
        {
            return ID.Equals(other?.ID);
        }

        public void ResetID(System.Guid id)
        {
            _guid = new SerializedGuid()
            {
                guid = id
            };
        }

        public object Clone()
        {
            var clone = (UnityCurrency) MemberwiseClone();
            clone.ResetID(System.Guid.NewGuid());
            return clone;
        }
        
        public override string ToString()
        {
            return name;
        }
    }
}