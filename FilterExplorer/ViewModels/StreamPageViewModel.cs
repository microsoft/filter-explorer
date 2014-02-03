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

        public ObservableCollection<PhotoThumbnailViewModel> Thumbnails { get; private set; }

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
            Thumbnails = new ObservableCollection<PhotoThumbnailViewModel>();

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            SelectPhotoCommand = new DelegateCommand((parameter) =>
                {
                    var viewModel = (PhotoThumbnailViewModel)parameter;

                    SessionModel.Instance.Photo = viewModel.Model;

                    var frame = (Frame)Window.Current.Content;
                    frame.Navigate(typeof(PhotoPage));
                });

            OpenPhotoCommand = new DelegateCommand(
                async (parameter) =>
                {
                    var file = await PhotoLibraryModel.PickPhotoFileAsync();

                    if (file != null)
                    {
                        var photo = new PhotoModel(file);

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

                        UpdateThumbnailsAsync(SessionModel.Instance.Folder);
                    }
                });


            if (SessionModel.Instance.Folder != null)
            {
                UpdateThumbnailsAsync(SessionModel.Instance.Folder);
            }
            else
            {
                UpdateThumbnailsAsync(Windows.Storage.KnownFolders.CameraRoll);
            }
        }

        private async void UpdateThumbnailsAsync(StorageFolder folder)
        {
            try
            {
                FolderName = folder.Name;

                Thumbnails.Clear();

                var photos = await PhotoLibraryModel.GetPhotosFromFolderAsync(folder);

                for (int i = 0; i < photos.Count; i++)
                {
                    for (int k = 0; k < 4 * 9 && i < photos.Count;)
                    {
                        Thumbnails.Add(new PhotoThumbnailViewModel(photos[i]));

                        k++;
                        i++;
                    }

                    await Task.Delay(TimeSpan.FromMilliseconds(1000));
                }
            }
            catch (Exception)
            {
            }
        }
    }
}
