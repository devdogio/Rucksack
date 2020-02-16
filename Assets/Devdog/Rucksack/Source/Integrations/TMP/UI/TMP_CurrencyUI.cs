using Devdog.Rucksack.Currencies;
using Devdog.Rucksack.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Devdog.Rucksack.Integrations.TMP
{
    
    /// <summary>
    /// Use this component instead of CurrencyUI if you want to use
    /// TextMesh Pro instead of Unity text.
    /// </summary>
    public class TMP_CurrencyUI : CurrencyUIBase<TMP_CurrencyUI>
    {
        [SerializeField]
        private Image _icon;

        [SerializeField]
        private TMP_Text _amount;

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
            
            this.Set(_amount, string.Format(_format, amount, currency.maxAmount, currency.name, currency.tokenName));
            var unityCurrency = currency as IUnityCurrency;
            if (unityCurrency != null)
            {
                Set(_icon, unityCurrency.icon);
            }
        }

        public override string ToString()
        {
            return currency?.ToString() ?? "<NULL>";
        }
    }
}