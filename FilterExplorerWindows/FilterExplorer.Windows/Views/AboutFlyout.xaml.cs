/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.ViewModels;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.Views
{
    public sealed partial class AboutFlyout : SettingsFlyout
    {
        private AboutPageViewModel _viewModel = new AboutPageViewModel();

        public AboutFlyout()
        {
            this.InitializeComponent();

            DataContext = _viewModel;

            Title = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("AboutFlyoutCommandLabel");
        }
    }
}
