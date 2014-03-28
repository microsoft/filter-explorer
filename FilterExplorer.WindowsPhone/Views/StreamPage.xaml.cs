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

            var strategy = new HighlightStrategy(12, new List<int> { 0, 7 });

            _viewModel = new StreamPageViewModel(strategy);

            DataContext = _viewModel;

            _timer.Tick += DispatcherTimer_Tick;

            SizeChanged += StreamPage_SizeChanged;
        }

        private void StreamPage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            var panel = Mosaic.ItemsPanelRoot as VariableSizedWrapGrid;

            if (panel != null)
            {
                var side = (e.NewSize.Width - panel.Margin.Left - panel.Margin.Right) / 3;

                panel.ItemWidth = side;
                panel.ItemHeight = side;
            }
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

            _timer.Start();
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            _timer.Stop();
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
            if (!_viewModel.Processing && _viewModel.RefreshSomePhotosCommand.CanExecute(null))
            {
                _viewModel.RefreshSomePhotosCommand.Execute(null);
            }
        }
    }
}
