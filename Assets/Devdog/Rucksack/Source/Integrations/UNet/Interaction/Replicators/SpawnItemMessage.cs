using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Items
{
    public sealed class SpawnItemMessage : MessageBase
    {
        public NetworkHash128 assetID;
        public NetworkInstanceId networkInstanceID;
        public Vector3 position;
        public Quaternion rotation;
    }
}