using UnityEngine.Networking;

namespace Devdog.Rucksack.Collections
{
    public sealed class SetCollectionContentsMessage : MessageBase
    {
        public string collectionName;
        public GuidMessage collectionGuid;
        public ItemAmountMessage[] items;
        
//        public ReadWritePermission permission;
//        public ushort collectionCount;

//        public override void Serialize(NetworkWriter writer)
//        {
//            writer.Write((byte)permission);
//            writer.Write((ushort)items.Length);
//            foreach (var item in items)
//            {
//                writer.Write(item);
//            }
//        }
//
//        public override void Deserialize(NetworkReader reader)
//        {
//            permission = (ReadWritePermission) reader.ReadByte();
//            var length = reader.ReadUInt16();
//            var result = new ItemAmountMessage[length];
//            for (int i = 0; i < length; i++)
//            {
//                result[i] = reader.ReadMessage<ItemAmountMessage>();
//            }
//
//            items = result;
//        }
    }
}