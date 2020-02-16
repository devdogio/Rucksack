using Photon.Pun;

namespace Devdog.Rucksack.Collections
{
    public interface IPUN2Collection : INetworkCollection<PhotonView>
    {
        void Register();
        void Server_Register();
        void UnRegister();
        void Server_UnRegister();
    }
}