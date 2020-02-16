using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class RequestDropItemMessage : MessageBase
    {
        /// <summary>
        /// The Unique GUID of the item we want to sell
        /// </summary>
        public GuidMessage itemGuid;

        /// <summary>
        /// The position the client requested to drop the item at. 
        /// The server will verify the position and choose it's own if the given position is not valid.
        /// </summary>
        public Vector3 worldPosition;
    }
}