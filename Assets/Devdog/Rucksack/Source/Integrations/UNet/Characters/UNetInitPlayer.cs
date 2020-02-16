using Devdog.General2;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Characters
{
    public class UNetInitPlayer : NetworkBehaviour
    {
//        public override void OnStartClient()
//        {
//            base.OnStartClient();
//            GetComponent<Player>()?.RegisterPlayerAsCurrentPlayer();
//        }

        public override void OnStartAuthority()
        {
            base.OnStartAuthority();
            GetComponent<Player>()?.RegisterPlayerAsCurrentPlayer();
        }
    }
}