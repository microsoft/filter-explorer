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
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Navigation;

namespace FilterExplorer.Views
{
    public sealed partial class PhotoPage : Page
    {
        private PhotoPageViewModel _viewModel = new PhotoPageViewModel();

        public PhotoPage()
        {
            this.InitializeComponent();

            DataContext = _viewModel;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.Portrait;

            if (!_viewModel.IsInitialized)
            {
                await _viewModel.InitializeAsync();

                Window.Current.Activate();
            }

            AdaptToOrientation();

            DisplayInformation.GetForCurrentView().OrientationChanged += DisplayInformation_OrientationChanged;
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            AdaptToOrientation();
        }

        private void Grid_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;

            if (orientation == DisplayOrientations.Portrait)
            {
                InformationGrid.Visibility = InformationGrid.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        private void AdaptToOrientation()
        {
            var orientation = DisplayInformation.GetForCurrentView().CurrentOrientation;

            InformationGrid.Opacity = orientation == DisplayOrientations.Portrait ? 1 : 0;
        }
    }
}
