using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    public class ItemVendorCustomerWrapper : ScriptableObject
    {
        public ICustomer<IItemInstance> customer;
    }
}