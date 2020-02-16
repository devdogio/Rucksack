using Devdog.General2;
using UnityEngine;

namespace Devdog.Rucksack.Items
{
    public interface INetworkItemInstance : IUnityItemInstance
    {
        /// <summary>
        /// Force use this item without any form of validation.
        /// </summary>
        Result<ItemUsedResult> Server_Use(Character character, ItemContext context);

        /// <summary>
        /// Replicate the (visual) behavior on the client.
        /// </summary>
        void Client_NotifyUsed(Character character, ItemContext context);
        
        /// <summary>
        /// DropDraggingItem this item into the world. Only to be called by the server.
        /// </summary>
        Result<GameObject> Server_Drop(Character character, Vector3 worldPosition);

//        /// <summary>
//        /// A callback from the server, notifying the client that the item was used.
//        /// <remarks>This method can be used for visual effects and audio, however, should not be used for game logic!</remarks>
//        /// </summary>
//        void Client_NotifyUsed(Character character, ItemContext context);
    }
}