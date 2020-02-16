using System;
using UnityEngine;

namespace Devdog.Rucksack
{
    [Serializable]
    public struct SerializedType
    {
        [SerializeField]
        private string _assemblyQualifiedName;
        public Type type
        {
            get
            {
                try
                {
                    return Type.GetType(_assemblyQualifiedName);
                }
                catch (Exception)
                {
                    return null;
                }
            }
            set { _assemblyQualifiedName = value.AssemblyQualifiedName; }
        }

        public override string ToString()
        {
            return _assemblyQualifiedName;
        }
    }
}