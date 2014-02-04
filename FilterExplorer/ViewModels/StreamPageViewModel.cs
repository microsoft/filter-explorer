using FilterExplorer.Commands;
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
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class StreamPageViewModel : INotifyPropertyChanged
    {
        private string _folderName = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand SelectPhotoCommand { get; private set; }
        public IDelegateCommand OpenPhotoCommand { get; private set; }
        public IDelegateCommand OpenFolderCommand { get; private set; }

        public ObservableCollection<StreamThumbnailViewModel> Thumbnails { get; private set; }

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

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("FolderName"));
                    }
                }
            }
        }

        public StreamPageViewModel()
        {
            Thumbnails = new ObservableCollection<StreamThumbnailViewModel>();

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            SelectPhotoCommand = new DelegateCommand((parameter) =>
                {
                    var viewModel = (StreamThumbnailViewModel)parameter;
                    var modelCopy = new FilteredPhotoModel(viewModel.Model);

                    SessionModel.Instance.Photo = modelCopy;

                    var frame = (Frame)Window.Current.Content;
                    frame.Navigate(typeof(PhotoPage));
                });

            OpenPhotoCommand = new DelegateCommand(
                async (parameter) =>
                {
                    var file = await PhotoLibraryModel.PickPhotoFileAsync();

                    if (file != null)
                    {
                        var photo = new FilteredPhotoModel(file);

                        SessionModel.Instance.Photo = photo;

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

                        UpdateThumbnailsAsync();
                    }
                });


            UpdateThumbnailsAsync();
        }

        private async void UpdateThumbnailsAsync()
        {
            Thumbnails.Clear();

            if (SessionModel.Instance.Folder != null)
            {
                FolderName = SessionModel.Instance.Folder.Name;

                var photos = await PhotoLibraryModel.GetPhotosFromFolderAsync(SessionModel.Instance.Folder, 128);

                foreach (var photo in photos)
                {
                    Thumbnails.Add(new StreamThumbnailViewModel(photo));
                }
            }
            else
            {
                var photos = await PhotoLibraryModel.GetPhotosFromFolderAsync(Windows.Storage.KnownFolders.CameraRoll, 128);

                if (photos.Count > 0)
                {
                    FolderName = Windows.Storage.KnownFolders.CameraRoll.Name;
                }
                else
                {
                    photos = await PhotoLibraryModel.GetPhotosFromFolderAsync(Windows.Storage.KnownFolders.PicturesLibrary, 128);

                    FolderName = Windows.Storage.KnownFolders.PicturesLibrary.Name;
                }

                foreach (var photo in photos)
                {
                    Thumbnails.Add(new StreamThumbnailViewModel(photo));
                }
            }
        }
    }
}
