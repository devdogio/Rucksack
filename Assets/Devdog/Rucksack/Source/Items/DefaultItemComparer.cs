using System;
using System.Collections.Generic;
using System.Globalization;

namespace Devdog.Rucksack.Items
{
    public sealed class DefaultItemComparer : IComparer<IItemInstance>
    {
        private StringComparer _stringComparer;
        public DefaultItemComparer()
        {
            _stringComparer = StringComparer.Create(CultureInfo.InvariantCulture, true);
        }
        
        public int Compare(IItemInstance x, IItemInstance y)
        {
            var a = x?.itemDefinition.name;
            var b = y?.itemDefinition.name;
            return _stringComparer.Compare(a, b);
        }
    }
}