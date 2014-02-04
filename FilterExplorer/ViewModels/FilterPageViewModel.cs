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
    public class FilterPageViewModel
    {
        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand ApplyFilterCommand { get; private set; }

        public ObservableCollection<FilterThumbnailViewModel> Thumbnails { get; private set; }

        public FilterPageViewModel()
        {
            Thumbnails = new ObservableCollection<FilterThumbnailViewModel>();

            InitializeAsync(SessionModel.Instance.Photo);

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            ApplyFilterCommand = new DelegateCommand((parameter) =>
                {
                    var viewModel = (FilterThumbnailViewModel)parameter;

                    SessionModel.Instance.Photo.Filters.Add(viewModel.Filter);

                    var frame = (Frame)Window.Current.Content;
                    frame.GoBack();
                });
        }

        private void InitializeAsync(FilteredPhotoModel model)
        {
            var filters = FilterFactory.CreateAllFilters();

            foreach (var filter in filters)
            {
                Thumbnails.Add(new FilterThumbnailViewModel(model, filter));
            }
        }
    }
}
