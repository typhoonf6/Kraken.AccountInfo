using System;
using System.Globalization;
using Xamarin.CommunityToolkit.ObjectModel;
using Xamarin.Forms;

namespace Kraken.AccountInfo
{
    public class TotalValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var assets = value as ObservableRangeCollection<Asset>;
            double totalValue = 0.0;
            foreach (var asset in assets)
                totalValue += asset.Value;
            // TODO Allow conversion to given currency
            return totalValue.ToString("C");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
