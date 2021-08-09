using System.Collections.Generic;

namespace Kraken.AccountInfo
{
    public class KrakenData<T>
    {
        public Dictionary<string, T> result { get; set; }
        public IEnumerable<string> error { get; set; }
    }
}
