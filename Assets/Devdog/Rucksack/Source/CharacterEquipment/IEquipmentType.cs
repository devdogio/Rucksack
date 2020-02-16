using System;

namespace Devdog.Rucksack.CharacterEquipment
{
    public interface IEquipmentType : IEquatable<IEquipmentType>, IIdentifiable
    {    
        string name { get; }

        /// <summary>
        /// Check to see if this equipment type is valid with another equipment type.
        /// <remarks>Note that this currently doesn't affect any behavior. It only exists for developers to extend upon.</remarks>
        /// </summary>
        /// <returns></returns>
        bool IsCompatible(IEquipmentType type);
    }
}