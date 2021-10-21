using QuikGraph;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace Kraken.AccountInfo
{
    public class StringCollectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var collection = value as IEnumerable<Edge<string>>;
            var sb = new StringBuilder();
            foreach (var item in collection)
            {
                if (sb.Length == 0)
                    sb.Append(item.Source);
                sb.Append($" -> {item.Target}");
            }
            return sb.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
