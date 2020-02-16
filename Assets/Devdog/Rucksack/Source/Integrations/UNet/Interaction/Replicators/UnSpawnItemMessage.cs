using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Items
{
    public sealed class UnSpawnItemMessage : MessageBase
    {
        public NetworkInstanceId networkInstanceID;
    }
}