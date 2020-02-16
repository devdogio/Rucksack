using Devdog.General2;
using Devdog.Rucksack.Characters;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Items;
using Devdog.Rucksack.Vendors;
using HutongGames.PlayMaker;
using UnityEngine;

namespace Devdog.Rucksack.Integrations.PlayMaker
{
    [ActionCategory(RucksackConstants.ProductName + "/Vendor")]
    public class CreateItemVendorCustomer : FsmStateAction
    {
        public FsmString customerGuid;

        [RequiredField]
        [ObjectType(typeof(Character))]
        public FsmObject characterObject;
        
        [RequiredField]
        [ObjectType(typeof(ItemVendorCustomerWrapper))]
        public FsmObject customerResult;


        public override string ErrorCheck()
        {
            if (customerResult.IsNone || customerResult.UseVariable == false)
            {
                return nameof(customerResult) + " variable is not set";
            }

            return "";
        }

        public override void OnEnter()
        {
            var customerWrapper = ScriptableObject.CreateInstance<ItemVendorCustomerWrapper>();
            var character = (Character)characterObject.Value;
            var inventories = character.GetComponent<IInventoryCollectionOwner>()?.itemCollectionGroup;
            var currencyCollections = character.GetComponent<ICurrencyCollectionOwner>()?.currencyCollectionGroup;

            if (inventories == null)
            {
                inventories = new CollectionGroup<IItemInstance>();
                LogWarning(nameof(IInventoryCollectionOwner) + " not found on character. Using empty group.");
            }

            if (currencyCollections == null)
            {
                currencyCollections = new CurrencyCollectionGroup<ICurrency>();
                LogWarning(nameof(ICurrencyCollectionOwner) + " not found on character. Using empty group.");
            }
            
            customerWrapper.customer = new Customer<IItemInstance>(System.Guid.Parse(customerGuid.Value), character, inventories, currencyCollections);
            customerResult = customerWrapper;
            Finish();
        }
    }
}