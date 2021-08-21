namespace Kraken.AccountInfo
{
    /// <summary>
    /// Represents a single asset for display on UI
    /// </summary>
    public class Asset
    {
        /// <summary>
        /// Alternative name of the asset
        /// </summary>
        public string altname { get; set; }

        /// <summary>
        /// Amount of the asset held
        /// </summary>
        public double Amount { get; set; }

        /// <summary>
        /// Value of the asset in app configured currency
        /// </summary>
        public double Value { get; set; }
    }
}
