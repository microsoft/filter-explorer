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

            DisplayInformation.AutoRotationPreferences = DisplayOrientations.Landscape | DisplayOrientations.Portrait;

            if (!_viewModel.IsInitialized)
            {
                await _viewModel.InitializeAsync();

                Window.Current.Activate();
            }
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
