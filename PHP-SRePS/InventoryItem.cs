using System;
using System.Collections.Generic;

namespace PHP_SRePS
{
    public class InventoryItem
    {
        public string Name { get; set; }

        // The dictionary will track quantity changes more accurately than what the get properties will return
        // We can use this for graphs
        readonly Dictionary<DateTime, int> inventoryDateQuantityPair = new Dictionary<DateTime, int>();

        public int QuantityCurrent
        {
            get
            {
                return inventoryDateQuantityPair[DateTime.Today];
            }
            set
            {
                inventoryDateQuantityPair.Add(DateTime.Today, value);
            }
        }
        public int QuantityLastWeek
        {
            get
            {
                return inventoryDateQuantityPair[DateTime.Today.AddDays(-7)];
            }
        }
        public int QuantityLastMonth
        {
            get
            {
                return inventoryDateQuantityPair[DateTime.Today.AddDays(-28)];
            }
        }
    }
}
