using System;

using Photon.Pun;

namespace Devdog.Rucksack
{
    public static class PUN2Extensions
    {
        public static bool IsServer(this PhotonView photonView)
        {
            return photonView.OwnerActorNr == PhotonNetwork.MasterClient.ActorNumber;
        }
    }
}
