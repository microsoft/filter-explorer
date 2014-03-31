/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
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
