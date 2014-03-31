/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Commands;
using FilterExplorer.Filters;
using FilterExplorer.Models;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.ViewModels
{
    public class FilterPageViewModel : ViewModelBase
    {
        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand ApplyFilterCommand { get; private set; }

        public ObservableCollection<ThumbnailViewModel> Thumbnails { get; private set; }

        public FilterPageViewModel()
        {
            Thumbnails = new ObservableCollection<ThumbnailViewModel>();

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            ApplyFilterCommand = new DelegateCommand((parameter) =>
            {
                var viewModel = (ThumbnailViewModel)parameter;

                SessionModel.Instance.Photo.Filters.Add(viewModel.Filter);

                var frame = (Frame)Window.Current.Content;
                frame.GoBack();
            });
        }

        public override Task<bool> InitializeAsync()
        {
            if (!IsInitialized && SessionModel.Instance.Photo != null)
            {
                Processing = true;

                var filters = FilterFactory.CreateAllFilters();

                foreach (var filter in filters)
                {
                    Thumbnails.Add(new ThumbnailViewModel(SessionModel.Instance.Photo, filter));
                }

                Processing = false;

                IsInitialized = true;
            }

            return Task.FromResult(IsInitialized);
        }
    }
}
