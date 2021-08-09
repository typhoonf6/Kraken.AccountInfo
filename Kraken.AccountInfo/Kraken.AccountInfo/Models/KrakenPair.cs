namespace Kraken.AccountInfo
{
    public class KrakenPair
    {

        /// <summary>
        /// Conversion pair thats split
        /// </summary>
        private string _wsname;
        public string wsname
        {
            get => _wsname;
            set
            {
                _wsname = value;

                var split = value.Split('/');

                AltBase = split[0];
                AltQuote = split[1];
            }
        }

        /// <summary>
        /// Alternate name for base
        /// </summary>
        public string AltBase { get; set; }

        /// <summary>
        /// Alternate name for quote
        /// </summary>
        public string AltQuote { get; set; }

        /// <summary>
        /// Source currency
        /// </summary>
        public string Base { get; set; }
        public string @base
        {
            set
            {
                Base = value;
            }
        }

        /// <summary>
        /// Target currnecy
        /// </summary>
        public string Quote { get; set; }
        public string quote {
            set
            {
                Quote = value;
            }
        }

        /// <summary>
        /// Rate of conversion from source to target
        /// </summary>
        public double ConversationRate { get; set;  }
    }
}