using Devdog.Rucksack.Currencies;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.UI
{
    public class CurrencyUI : CurrencyUIBase<CurrencyUI>
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private Text _amount;

        /// <summary>
        /// The format for displaying the currency amount. 
        /// {0} = Currency amount
        /// {1} = Max currency amount
        /// {2} = Currency name
        /// {3} = Currency token name
        /// </summary>
        [SerializeField]
        private string _format = "{0}/{1}";

        public override void Repaint(double amount, ICurrency repaintCurrency)
        {
            this.currency = repaintCurrency;
            
            Set(_amount, string.Format(_format, amount, currency.maxAmount, currency.name, currency.tokenName));
            var unityCurrency = currency as IUnityCurrency;
            if (unityCurrency != null)
            {
                Set(_icon, unityCurrency.icon);
            }
        }
    }
}