/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Utilities;
using FilterExplorer.ViewModels;
using System;
using System.Collections.Generic;
using Windows.Devices.Sensors;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FilterExplorer.Views
{
    public sealed partial class StreamPage : Page
    {
        private StreamPageViewModel _viewModel = null;
        private DispatcherTimer _timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 2) };

        public StreamPage()
        {
            this.InitializeComponent();

            var strategy = GetHighlightStrategyForOrientation(DisplayInformation.GetForCurrentView().CurrentOrientation);

            _viewModel = new StreamPageViewModel(strategy);

            DataContext = _viewModel;

            _timer.Tick += DispatcherTimer_Tick;
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (!_viewModel.IsInitialized)
            {
                await _viewModel.InitializeAsync();

                Window.Current.Activate();
            }

            _timer.Start();

            DisplayInformation.GetForCurrentView().OrientationChanged += DisplayInformation_OrientationChanged;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            _timer.Stop();

            DisplayInformation.GetForCurrentView().OrientationChanged -= DisplayInformation_OrientationChanged;
        }

        private void DisplayInformation_OrientationChanged(DisplayInformation sender, object args)
        {
            var strategy = GetHighlightStrategyForOrientation(DisplayInformation.GetForCurrentView().CurrentOrientation);

            if (_viewModel.ChangeHighlightStrategyCommand.CanExecute(strategy))
            {
                _viewModel.ChangeHighlightStrategyCommand.Execute(strategy);
            }
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var viewModel = DataContext as StreamPageViewModel;

            if (viewModel != null)
            {
                viewModel.SelectPhotoCommand.Execute(e.ClickedItem);
            }
        }

        private void DispatcherTimer_Tick(object sender, object e)
        {
            if (_viewModel.RefreshSomePhotosCommand.CanExecute(null))
            {
                _viewModel.RefreshSomePhotosCommand.Execute(null);
            }
        }

        private HighlightStrategy GetHighlightStrategyForOrientation(DisplayOrientations orientation)
        {
            switch (orientation)
            {
                case DisplayOrientations.Portrait:
                case DisplayOrientations.PortraitFlipped:
                    {
                        // Portrait
                        return new HighlightStrategy(18, new List<int> { 0, 5, 8});
                    }

                default:
                    {
                        // Landscape
                        return new HighlightStrategy(15, new List<int> { 0, 7, 11 });
                    }
            }
        }
    }
}
