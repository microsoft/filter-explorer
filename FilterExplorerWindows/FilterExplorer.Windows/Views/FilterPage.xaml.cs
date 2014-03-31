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
        }

        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

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
