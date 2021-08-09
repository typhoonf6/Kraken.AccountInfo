namespace Kraken.AccountInfo
{
    /// <summary>
    /// Represents a single asset for display on UI
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// Abbreviated name of the asset
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Amount of the asset held
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Value in chosen currency
        /// </summary>
        public double Value { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="amount"></param>
        public Asset(string name, double amount)
        {
            Name = name;
            Amount = amount;
        }
    }
}
