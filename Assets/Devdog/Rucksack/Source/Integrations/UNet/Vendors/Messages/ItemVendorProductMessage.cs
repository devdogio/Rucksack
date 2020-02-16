using System.Linq;
using Devdog.Rucksack.Collections;
using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.Database;
using Devdog.Rucksack.Items;
using UnityEngine.Networking;

namespace Devdog.Rucksack.Vendors
{
    [System.Serializable]
    public class ItemVendorProductMessage : MessageBase
    {
        public ItemAmountMessage itemAmount;

        public CurrencyDecoratorMessage[] buyPrice;
        public CurrencyDecoratorMessage[] sellPrice;


        public ItemVendorProductMessage()
        {
            this.itemAmount = new ItemAmountMessage()
            {
                itemInstance = new RegisterItemInstanceMessage(),
                amount = 0
            };
            
            buyPrice = new CurrencyDecoratorMessage[0];
            sellPrice = new CurrencyDecoratorMessage[0];
        }

        public ItemVendorProductMessage(IVendorProduct<IItemInstance> product, int amount)
        {
            this.itemAmount = new ItemAmountMessage()
            {
                itemInstance = new RegisterItemInstanceMessage(product.item),
                amount = (ushort)amount
            };
            
            buyPrice = new CurrencyDecoratorMessage[product.buyPrice.Length];
            for (int i = 0; i < product.buyPrice.Length; i++)
            {
                buyPrice[i] = new CurrencyDecoratorMessage(product.buyPrice[i]);
            }
            
            sellPrice = new CurrencyDecoratorMessage[product.sellPrice.Length];
            for (int i = 0; i < product.sellPrice.Length; i++)
            {
                sellPrice[i] = new CurrencyDecoratorMessage(product.sellPrice[i]);
            }
        }
        
        
        public IVendorProduct<IItemInstance> TryCreateVendorProductInstance(IDatabase<UnityItemDefinition> itemDatabase, IDatabase<UnityCurrency> currencyDatabase, out int amount)
        {
            var itemInstance = itemAmount.itemInstance.TryCreateItemInstance(itemDatabase);
            amount = itemAmount.amount;
            
            return new VendorProduct<IItemInstance>(itemInstance, buyPrice.Select(o => o.ToDecorator(currencyDatabase)).ToArray(), sellPrice.Select(o => o.ToDecorator(currencyDatabase)).ToArray());
        }
    }
}