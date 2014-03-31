/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Filters;
using System;
using System.Collections.Generic;
using Windows.UI.Xaml.Data;

namespace FilterExplorer.Converters
{
    public class FilterListStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var list = (IList<Filter>)value;
            var text = string.Empty;
            var separator = parameter != null ? parameter.ToString() : " ";

            if (list != null && list.Count > 0)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    text += list[i].ToString();

                    if (i < list.Count - 1)
                    {
                        text += separator;
                    }
                }
            }
            else
            {
                text = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("PhotoPageZeroFiltersText");
            }

            return text;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
