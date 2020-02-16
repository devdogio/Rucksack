using UnityEngine;
using UnityEngine.Networking;

namespace Devdog.Rucksack
{
    public sealed class UNetRename : MonoBehaviour
    {
        [SerializeField]
        private string _localPlayer = "LocalPlayer:{0}";
        
        [SerializeField]
        private string _networkPlayer = "NetworkPlayer:{0}";
        
        private void Start()
        {
            var identity = GetComponent<NetworkIdentity>();
            if (identity.isLocalPlayer)
            {
                gameObject.name = string.Format(_localPlayer, identity.netId);
            }
            else
            {
                gameObject.name = string.Format(_networkPlayer, identity.netId);
            }
        }
    }
}