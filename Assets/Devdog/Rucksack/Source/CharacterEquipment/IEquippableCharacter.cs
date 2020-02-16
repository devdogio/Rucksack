
using System;

namespace Devdog.Rucksack.CharacterEquipment
{
    public interface IEquippableCharacter<TEquippableItemType>
        where TEquippableItemType : IEquatable<TEquippableItemType>
    {
        IMountPoint<TEquippableItemType>[] mountPoints { get; }
        
        Result<EquipmentResult<TEquippableItemType>[]> Equip(TEquippableItemType item, int amount = 1);
        Result<EquipmentResult<TEquippableItemType>> EquipAt(int index, TEquippableItemType item, int amount = 1);
        Result<UnEquipmentResult> UnEquip(TEquippableItemType item, int amount = 1);
        Result<UnEquipmentResult> UnEquipAt(int index, int amount = 1);

        Result<bool> SwapOrMerge(int from, int to);
    }
}