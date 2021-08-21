namespace Kraken.AccountInfo
{
    public class Ticker
    {
        /// <summary>
        /// Asking price [price, whole lot volume, lot volume]
        /// </summary>
        public string[] a { get; set; }

        /// <summary>
        /// Bid price [price, whole lot volume, lot volume]
        /// </summary>
        public string[] b { get; set; }

        /// <summary>
        /// Volume weighted average price [today, 24hrs]
        /// </summary>
        public string[] p { get; set; }
    }
}