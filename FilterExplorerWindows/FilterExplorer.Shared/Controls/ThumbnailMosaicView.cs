/*
 * Copyright (c) 2014 Microsoft Mobile. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.Controls
{
    public class ThumbnailMosaicView : GridView
    {
        protected override void PrepareContainerForItemOverride(DependencyObject element, object item)
        {
            try
            {
                dynamic localItem = item;
                var span = localItem.Highlight ? 2 : 0;
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, span);
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, span);
            }
            catch
            {
                element.SetValue(VariableSizedWrapGrid.RowSpanProperty, 1);
                element.SetValue(VariableSizedWrapGrid.ColumnSpanProperty, 1);
            }

            base.PrepareContainerForItemOverride(element, item);
        }
    }
}
