using FFImageLoading.Forms;
using FFImageLoading.Svg.Forms;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace Kraken.AccountInfo
{
    public class AssetIconSourceConverter : FFImageLoading.Forms.ImageSourceConverter, IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var source = "btc.svg";
            var test = new EmbeddedResourceImageSource(new Uri(source));
            return test;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
