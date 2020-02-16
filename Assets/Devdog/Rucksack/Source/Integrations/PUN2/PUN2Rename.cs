using UnityEngine;

using Photon.Pun;

namespace Devdog.Rucksack
{
    public sealed class PUN2Rename : MonoBehaviourPun
    {
        [SerializeField]
        private string _localPlayer = "LocalPlayer:{0}";

        [SerializeField]
        private string _networkPlayer = "NetworkPlayer:{0}";

        private void Start()
        {
            gameObject.name = string.Format(this.photonView.IsMine ? _localPlayer : _networkPlayer, this.photonView.OwnerActorNr);
        }
    }
}