namespace Devdog.Rucksack
{
    public struct GuidMessage
    {
        public const int GuidByteCount = 16;

        public byte[] bytes;
        public System.Guid guid
        {
            get
            {
                if (bytes == null || bytes.Length != GuidByteCount)
                {
                    return System.Guid.Empty;
                }
                
                return new System.Guid(bytes);
            }
        }

        public static implicit operator GuidMessage(System.Guid guid)
        {
            return new GuidMessage()
            {
                bytes = guid.ToByteArray()
            };
        }
        
        public static implicit operator System.Guid(GuidMessage msg)
        {
            return msg.guid;
        }

//        public override void Serialize(NetworkWriter writer)
//        {
//            writer.Write(bytes, GuidByteCount);
//        }
//
//        public override void Deserialize(NetworkReader reader)
//        {
//            bytes = reader.ReadBytes(GuidByteCount);
//        }

        public override string ToString()
        {
            return guid.ToString();
        }
    }
}