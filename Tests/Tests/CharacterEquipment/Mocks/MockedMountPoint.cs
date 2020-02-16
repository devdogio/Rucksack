using Devdog.Rucksack.CharacterEquipment;
using Devdog.Rucksack.CharacterEquipment.Items;

namespace Devdog.Rucksack.Tests
{
    public class MockedMountPoint :  IMountPoint<IEquippableItemInstance>
    {
        public string name { get; set; }

        public IEquippableItemInstance mountedItem { get; private set; }
        public bool hasMountedItem
        {
            get { return mountedItem != null; }
        }

        public MockedMountPoint(string name)
        {
            this.name = name;
        }

        public void Mount(IEquippableItemInstance item)
        {
            mountedItem = item;
        }

        public void Clear()
        {
            mountedItem = null;
        }
    }
}