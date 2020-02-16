using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public interface IUNetCollection : INetworkCollection<NetworkIdentity>
    {
        void Register();
        void Server_Register();
        void UnRegister();
        void Server_UnRegister();
    }
}