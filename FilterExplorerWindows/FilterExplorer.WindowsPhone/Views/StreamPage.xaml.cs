/*
 * Copyright (c) 2014 Microsoft Mobile
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using FilterExplorer.Models;
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

            Mosaic.LayoutUpdated += Mosaic_LayoutUpdated;
        }

        private void Mosaic_LayoutUpdated(object sender, object e)
        {
            var panel = Mosaic.ItemsPanelRoot as VariableSizedWrapGrid;

            if (panel != null)
            {
                var side = (ActualWidth - panel.Margin.Left - panel.Margin.Right) / 3;

                panel.ItemWidth = side;
                panel.ItemHeight = side;
            }
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Portrait;

            SessionModel.Instance.Photo = null;

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
