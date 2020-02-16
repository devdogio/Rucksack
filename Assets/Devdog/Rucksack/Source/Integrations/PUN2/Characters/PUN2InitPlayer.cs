using System;
using Devdog.General2;
using Photon.Pun;
using UnityEngine;

namespace Devdog.Rucksack.Characters
{
    [RequireComponent(typeof(Player))]
    public class PUN2InitPlayer : MonoBehaviourPun
    {
        void Awake()
        {
            var player = GetComponent<Player>();
            if (player != null)
            {
                if (this.photonView.IsMine)
                {
                    player.RegisterPlayerAsCurrentPlayer();
                    new UnityLogger("[PUN2][PUN2InitPlayer] ")
                        .Log($"[Server][ViewId: {this.photonView.ViewID}] RegisterPlayerAsCurrentPlayer()", this);
                }
                else
                {
                    // disable trigger selector, otherwise triggers would be fired for every players on the scene.
                    player.triggerSelector = null;
                }
            }
        }
    }
}
