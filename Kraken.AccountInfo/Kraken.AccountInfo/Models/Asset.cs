using QuickGraph;
using System.Collections.Generic;

namespace Kraken.AccountInfo
{
    /// <summary>
    /// Represents a single asset for display on UI
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// Alternative name of the asset
        /// Note: Naming violation due to JSON deserialization.
        /// </summary>
        private string _altname;
        public string altname 
        {
            get => _altname;
            set
            {
                _altname = value;
                if (value.EndsWith(".S"))
                {
                    value = value.TrimEnd('2', '.', 'S');
                    Type = "Staked";
                }
                    
                IconSource = $"resource://Kraken.AccountInfo.Resources.Icons.{value.ToLower()}.svg";
            }
        }

        /// <summary>
        /// Source address for the svg icon
        /// </summary>
        public string IconSource { get; set; }

        /// <summary>
        /// Type of asset. Staked/Spot
        /// </summary>
        public string Type { get; set; } = "Spot";

        /// <summary>
        /// Amount of the asset held
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Value of the asset in app configured currency
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// The rate to convert the ammount to the Value
        /// Note: This could be just in a converter, but just hard coding for now.
        /// </summary>
        public double ConversionRate { get; set; }

        /// <summary>
        /// The route to convert from amount to the value
        /// based on the application selected currency
        /// </summary>
        public IEnumerable<Edge<string>> ConversionRoute { get; set; }
    }
}
