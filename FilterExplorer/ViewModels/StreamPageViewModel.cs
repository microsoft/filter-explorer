using FilterExplorer.Commands;
using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Utilities;
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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class StreamPageViewModel : PageViewModelBase
    {
        private string _folderName = null;
        private Random _random = new Random(DateTime.Now.Millisecond + 1);

        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand SelectPhotoCommand { get; private set; }
        public IDelegateCommand OpenPhotoCommand { get; private set; }
        public IDelegateCommand OpenFolderCommand { get; private set; }
        public IDelegateCommand CapturePhotoCommand { get; private set; }
        public IDelegateCommand RefreshPhotosCommand { get; private set; }

        public ObservableCollection<ThumbnailViewModel> Thumbnails { get; private set; }

        public string FolderName
        {
            get
            {
                return _folderName;
            }

            private set
            {
                if (_folderName != value)
                {
                    _folderName = value;

                    Notify("FolderName");
                }
            }
        }

        public StreamPageViewModel()
        {
            Thumbnails = new ObservableCollection<ThumbnailViewModel>();

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            SelectPhotoCommand = new DelegateCommand((parameter) =>
                {
                    var viewModel = (ThumbnailViewModel)parameter;

                    SessionModel.Instance.Photo = new FilteredPhotoModel(viewModel.Model);

                    var frame = (Frame)Window.Current.Content;
                    frame.Navigate(typeof(PhotoPage));
                });

            OpenPhotoCommand = new DelegateCommand(
                async (parameter) =>
                {
                    var file = await PhotoLibraryModel.PickPhotoFileAsync();

                    if (file != null)
                    {
                        SessionModel.Instance.Photo = new FilteredPhotoModel(file);

                        var frame = (Frame)Window.Current.Content;
                        frame.Navigate(typeof(PhotoPage));
                    }
                });

            OpenFolderCommand = new DelegateCommand(
                async (parameter) =>
                {
                    var folder = await PhotoLibraryModel.PickPhotoFolderAsync();

                    if (folder != null && (SessionModel.Instance.Folder == null || !folder.IsEqual(SessionModel.Instance.Folder)))
                    {
                        SessionModel.Instance.Folder = folder;

                        await Refresh();
                    }
                });

            CapturePhotoCommand = new DelegateCommand(
                async (parameter) =>
                {
                    var file = await PhotoLibraryModel.CapturePhotoFileAsync();

                    if (file != null)
                    {
                        SessionModel.Instance.Photo = new FilteredPhotoModel(file);

                        var frame = (Frame)Window.Current.Content;
                        frame.Navigate(typeof(PhotoPage));
                    }
                });

            RefreshPhotosCommand = new DelegateCommand(
                async (parameter) =>
                    {
                        await Refresh();
                    },
                () =>
                    {
                        return !Processing;
                    });
        }

        public override async Task<bool> InitializeAsync()
        {
            if (!IsInitialized)
            {
                await Refresh();

                IsInitialized = true;
            }

            return IsInitialized;
        }

        private async Task Refresh()
        {
            Processing = true;

            RefreshPhotosCommand.RaiseCanExecuteChanged();

            Thumbnails.Clear();

            var filters = FilterFactory.CreateStreamFilters();

            if (SessionModel.Instance.Folder != null)
            {
                FolderName = SessionModel.Instance.Folder.Name;

                var models = await PhotoLibraryModel.GetPhotosFromFolderAsync(SessionModel.Instance.Folder, 128);

                foreach (var model in models)
                {
                    Thumbnails.Add(new ThumbnailViewModel(model, TakeRandomFilter(filters)));
                }
            }
            else
            {
                var models = await PhotoLibraryModel.GetPhotosFromFolderAsync(Windows.Storage.KnownFolders.CameraRoll, 128);

                if (models.Count > 0)
                {
                    FolderName = Windows.Storage.KnownFolders.CameraRoll.Name;
                }
                else
                {
                    models = await PhotoLibraryModel.GetPhotosFromFolderAsync(Windows.Storage.KnownFolders.PicturesLibrary, 128);

                    FolderName = Windows.Storage.KnownFolders.PicturesLibrary.Name;
                }

                foreach (var model in models)
                {
                    Thumbnails.Add(new ThumbnailViewModel(model, TakeRandomFilter(filters)));
                }
            }

            Processing = false;

            RefreshPhotosCommand.RaiseCanExecuteChanged();
        }

        private Filter TakeRandomFilter(ObservableList<Filter> filters)
        {
            var index = _random.Next(0, filters.Count - 1);
            var filter = filters[index];

            return filter;
        }
    }
}
