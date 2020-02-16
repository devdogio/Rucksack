using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestUseItemMessage : MessageBase
    {
//        /// <summary>
//        /// The name of the collection this item resides in.
//        /// NOTE: An item HAS to reside in a collection to be usable to the player. World objects are triggers (different system)
//        /// </summary>
//        public byte[] collectionGuidBytes;
        
        /// <summary>
        /// The Unique GUID of the item we want to use
        /// </summary>
        public GuidMessage itemGuid;

        /// <summary>
        /// The amount of items we want to use
        /// </summary>
        public ushort useAmount = 1;

        /// <summary>
        /// The target index for this use operation.
        /// This is only relevant when an item is moved during it's use process (like equippable items).
        /// </summary>
        public short targetIndex = -1;
    }
}