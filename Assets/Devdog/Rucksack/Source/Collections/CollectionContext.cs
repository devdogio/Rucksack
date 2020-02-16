using System;

namespace Devdog.Rucksack.Collections
{
    public partial class CollectionContext
    {
        [Flags]
        public enum Events : byte
        {
            Add = 1,
            Remove = 2,
            SlotChanged = 4,
            SizeChanged = 8
        }

        [Flags]
        public enum Validations : byte
        {
            Restrictions = 1,
            SpecificInstance = 2,
        }
        
        public int originalIndex = -1;
        public Events fireEventFlags;
        public Validations validationFlags;
        public bool allowAABBCollisionSelf = true;
//        public bool preferEmptySlot = false;

        public CollectionContext()
        {
            // Fire everything by default
            fireEventFlags |= (Events.Add | Events.Remove | Events.SizeChanged | Events.SlotChanged);
            validationFlags |= (Validations.Restrictions | Validations.SpecificInstance);
        }


        public bool HasFlag(Validations flags)
        {
            return (validationFlags & flags) != 0;
        }
        
        public bool HasFlag(Events flags)
        {
            return (fireEventFlags & flags) != 0;
        }

        public CollectionContext Clone()
        {
            return (CollectionContext) MemberwiseClone();
        }
    }
}