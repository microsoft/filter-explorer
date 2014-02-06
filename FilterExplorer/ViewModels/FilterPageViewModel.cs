using FilterExplorer.Commands;
using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class FilterPageViewModel : PageViewModelBase
    {
        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand ApplyFilterCommand { get; private set; }

        public ObservableCollection<FilterThumbnailViewModel> Thumbnails { get; private set; }

        public FilterPageViewModel()
        {
            Thumbnails = new ObservableCollection<FilterThumbnailViewModel>();

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            ApplyFilterCommand = new DelegateCommand((parameter) =>
            {
                var viewModel = (FilterThumbnailViewModel)parameter;

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
                    Thumbnails.Add(new FilterThumbnailViewModel(SessionModel.Instance.Photo, filter));
                }

                Processing = false;

                IsInitialized = true;
            }

            return Task.FromResult(IsInitialized);
        }
    }
}
