using System;
using UnityEngine;

namespace Devdog.Rucksack
{
    [Serializable]
    public struct SerializedGuid
    {
        [SerializeField]
        private byte[] _guidBytes;
        public System.Guid guid
        {
            get
            {
                if (_guidBytes == null || _guidBytes.Length != 16)
                {
                    return System.Guid.Empty;
                }
                
                return new System.Guid(_guidBytes);
            }
            set { _guidBytes = value.ToByteArray(); }
        }

        public override string ToString()
        {
            return guid.ToString();
        }
    }
}