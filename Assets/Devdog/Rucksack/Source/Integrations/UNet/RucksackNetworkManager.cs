using UnityEngine.Networking;

namespace Devdog.Rucksack
{
    public class RucksackNetworkManager : NetworkManager
    {
        
        
        public override void OnServerConnect(NetworkConnection conn)
        {
            base.OnServerConnect(conn);
            conn.SetChannelOption(Channels.DefaultReliable, ChannelOption.MaxPendingBuffers, 500);
        }
    }
}