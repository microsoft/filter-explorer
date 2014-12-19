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

using FilterExplorer.ViewModels;
using System;
using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace FilterExplorer.Views
{
    public sealed partial class FilterPage : Page
    {
        private FilterPageViewModel _viewModel = new FilterPageViewModel();

        public FilterPage()
        {
            this.InitializeComponent();

            DataContext = _viewModel;

            FilterGrid.LayoutUpdated += FilterGrid_LayoutUpdated;
        }

        private void FilterGrid_LayoutUpdated(object sender, object e)
        {
            var panel = FilterGrid.ItemsPanelRoot as WrapGrid;

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

            if (!_viewModel.IsInitialized)
            {
                await _viewModel.InitializeAsync();

                Window.Current.Activate();
            }
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ((IDisposable)_viewModel).Dispose();
            base.OnNavigatedFrom(e);
        }

        private void GridView_ItemClick(object sender, ItemClickEventArgs e)
        {
            var viewModel = DataContext as FilterPageViewModel;

            if (viewModel != null)
            {
                viewModel.ApplyFilterCommand.Execute(e.ClickedItem);
            }
        }
    }
}
