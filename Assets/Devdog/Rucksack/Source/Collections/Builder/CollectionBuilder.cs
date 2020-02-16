using System;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.Rucksack.Collections
{
    public sealed class CollectionBuilder<TElementType>
        where TElementType : class, IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable
    {
        private bool _isReadOnly;
        private Type _slotType;
        private int _size;
        private ILogger _logger;
        private List<ICollectionRestriction<TElementType>> _restrictions;
        private string _collectionName;

        public CollectionBuilder()
        {
            Reset();
        }
        
        public CollectionBuilder<TElementType> SetLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }
        
        public CollectionBuilder<TElementType> SetIsReadOnly(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
            return this;
        }

        public CollectionBuilder<TElementType> SetSlotType<TSlotType>()
        {
            _slotType = typeof(TSlotType);
            return this;
        }

        public CollectionBuilder<TElementType> SetSlotType(Type slotType)
        {
            _slotType = slotType;
            return this;
        }

        public CollectionBuilder<TElementType> SetSize(int size)
        {
            _size = size;
            return this;
        }
        
        public CollectionBuilder<TElementType> SetRestrictions(params ICollectionRestriction<TElementType>[] restrictions)
        {
            _restrictions = restrictions.ToList();
            return this;
        }
        
//        public CollectionBuilder<TElementType> AddRestriction<TRestrictionType>()
//        {
////            _restrictions.Add(new TRestrictionType());
//            return this;
//        }

        public CollectionBuilder<TElementType> SetName(string name)
        {
            _collectionName = name;
            return this;
        }
        
        public CollectionBuilder<TElementType> AddRestriction(ICollectionRestriction<TElementType> instance)
        {
            _restrictions.Add(instance);
            return this;
        }
        
//        public CollectionBuilder<TElementType> SetOwner(IIdentifiable owner)
//        {
//            throw new NotImplementedException();
//        }

        public Collection<TElementType> Build()
        {
            if (_size <= 0)
            {
                throw new ArgumentException("Can't build collection because size is 0 or lower.");
            }
            
            var col = new Collection<TElementType>(_size, _logger);
            col.GenerateSlots(_slotType);
            col.isReadOnly = _isReadOnly;
            col.collectionName = _collectionName;
            col.restrictions.AddRange(_restrictions);

            return col;
        }

        public CollectionBuilder<TElementType> Reset()
        {
            _logger = new NullLogger();
            _isReadOnly = false;
            _slotType = typeof(CollectionSlot<TElementType>);
            _size = 0;
            _restrictions = new List<ICollectionRestriction<TElementType>>();
            return this;
        }
    }
}