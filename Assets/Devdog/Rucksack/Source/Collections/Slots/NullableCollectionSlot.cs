//
//using System;
//
//namespace Devdog.Rucksack.Collections
//{
        // NOTE: Proof of concept for nullable collection slots; Useful for data types.
//    public sealed class NullableCollectionSlot<T> : ICollectionSlot<T>
//        where T : struct, IEquatable<T>
//    {
//        private T? _item;
//        public T item
//        {
//            get { return _item.GetValueOrDefault(); }
//            private set
//            {
//                _item = value;
//                Repaint();
//            }
//        }
//    
//        // TODO: Consider where to place item amount. On item or in CollectionSlot.
//        // Collection slot amount could relay to the item instance, however, setting the value directly
//        // on the item instance would be possible, circumventing the collection updates completely.
//        private int _amount;
//        public int amount
//        {
//            get { return _amount; }
//            set
//            {
//                _amount = value;
//                Repaint();
//            }
//        }
// 
//        public bool isNotOccupied
//        {
//            get
//            {
//                // TODO: Also check if this slot is occupied by another slot.
//                return _item.HasValue == false;
//            }
//        }
//    
//        public bool CanSet(T item)
//        {
//            return true;
//        }
//    
//        public void Set(T item)
//        {
//            this.item = item;
//            this.amount = 0; // TODO: Considered weird behavior??
//        }
//    
//        public void Clear()
//        {
//            this.item = default(T);
//            this.amount = 0;
//        }
//    
//        public void Repaint()
//        {
//            // TODO: Repaint UI elements
//        }
//    }
//}