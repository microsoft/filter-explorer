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

namespace FilterExplorer.Commands
{
    public static class CommandFactory
    {
        public static IDelegateCommand CreateGoBackCommand()
        {
            return new DelegateCommand((parameter) =>
            {
                var frame = (Frame)Window.Current.Content;

                if (frame != null && frame.CanGoBack)
                {
                    frame.GoBack();
                }
            });
        }
    }
}
