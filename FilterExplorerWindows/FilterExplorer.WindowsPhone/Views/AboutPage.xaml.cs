/*
 * Copyright (c) 2014 Microsoft Mobile. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.ViewModels;
using System;
using System.Xml.Linq;
using Windows.Graphics.Display;
using Windows.UI;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Navigation;

namespace FilterExplorer.Views
{
    public sealed partial class AboutPage : Page
    {
        private AboutPageViewModel _viewModel = new AboutPageViewModel();

        public AboutPage()
        {
            this.InitializeComponent();

            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            if (!_viewModel.IsInitialized)
            {
                await _viewModel.InitializeAsync();

                Window.Current.Activate();
            }
        }

        private async void LinkTextBlock_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            var uri = new Uri(new Windows.ApplicationModel.Resources.ResourceLoader().GetString("ProjectLink"), UriKind.Absolute);

            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
