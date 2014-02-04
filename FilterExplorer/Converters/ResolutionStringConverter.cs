using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml.Data;

namespace FilterExplorer.Converters
{
    public class ResolutionStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var size = (Size?)value;
            var text = string.Empty;

            if (size.HasValue)
            {
                text = String.Format("{0} x {1}", (int)size.Value.Width, (int)size.Value.Height);
            }
            else
            {
                text = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("PhotoPageUnknownResolutionText");
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
