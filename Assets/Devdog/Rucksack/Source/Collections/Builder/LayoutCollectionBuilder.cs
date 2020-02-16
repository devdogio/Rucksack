using System;
using System.Collections.Generic;
using System.Linq;

namespace Devdog.Rucksack.Collections
{
    public sealed class LayoutCollectionBuilder<TElementType>
        where TElementType : class, IEquatable<TElementType>, IStackable, IIdentifiable, ICloneable, IShapeOwner2D
    {
        private bool _isReadOnly;
        private Type _slotType;
        private int _size;
        private int _columnCount = 4;
        private ILogger _logger;
        private List<ICollectionRestriction<TElementType>> _restrictions;
        private string _collectionName;

        public LayoutCollectionBuilder()
        {
            Reset();
        }
        
        public LayoutCollectionBuilder<TElementType> SetLogger(ILogger logger)
        {
            _logger = logger;
            return this;
        }
        
        public LayoutCollectionBuilder<TElementType> SetIsReadOnly(bool isReadOnly)
        {
            _isReadOnly = isReadOnly;
            return this;
        }

        public LayoutCollectionBuilder<TElementType> SetColumnCount(int count)
        {
            _columnCount = count;
            return this;
        }
        
        public LayoutCollectionBuilder<TElementType> SetSlotType<TSlotType>()
        {
            _slotType = typeof(TSlotType);
            return this;
        }

        public LayoutCollectionBuilder<TElementType> SetSlotType(Type slotType)
        {
            _slotType = slotType;
            return this;
        }

        public LayoutCollectionBuilder<TElementType> SetSize(int size)
        {
            _size = size;
            return this;
        }
        
        public LayoutCollectionBuilder<TElementType> SetRestrictions(params ICollectionRestriction<TElementType>[] restrictions)
        {
            _restrictions = restrictions.ToList();
            return this;
        }
        
//        public CollectionBuilder<TElementType> AddRestriction<TRestrictionType>()
//        {
////            _restrictions.Add(new TRestrictionType());
//            return this;
//        }

        public LayoutCollectionBuilder<TElementType> SetName(string name)
        {
            _collectionName = name;
            return this;
        }
        
        public LayoutCollectionBuilder<TElementType> AddRestriction(ICollectionRestriction<TElementType> instance)
        {
            _restrictions.Add(instance);
            return this;
        }
        
//        public CollectionBuilder<TElementType> SetOwner(IIdentifiable owner)
//        {
//            throw new NotImplementedException();
//        }

        public LayoutCollection<TElementType> Build()
        {
            if (_size <= 0)
            {
                throw new ArgumentException("Can't build collection because size is 0 or lower.");
            }
            
            var col = new LayoutCollection<TElementType>(_size, _columnCount, _logger);
            col.GenerateSlots(_slotType);
            col.isReadOnly = _isReadOnly;
            col.collectionName = _collectionName;
            col.restrictions = _restrictions;

            return col;
        }

        public LayoutCollectionBuilder<TElementType> Reset()
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