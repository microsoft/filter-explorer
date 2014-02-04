using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class PhotoViewModel : INotifyPropertyChanged
    {
        private BitmapImage _photo = null;
        private Size? _resolution = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal FilteredPhotoModel Model { get; private set; }

        public ObservableList<Filter> Filters
        {
            get
            {
                return Model.Filters;
            }
        }

        public Size? Resolution
        {
            get
            {
                if (!_resolution.HasValue)
                {
                    UpdateResolution();
                }

                return _resolution;
            }

            set
            {
                if (_resolution != value)
                {
                    _resolution = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Resolution"));
                    }
                }
            }
        }

        public BitmapImage Photo
        {
            get
            {
                if (_photo == null)
                {
                    UpdatePhoto();
                }

                return _photo;
            }

            private set
            {
                if (_photo != value)
                {
                    _photo = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Photo"));
                    }
                }
            }
        }

        public PhotoViewModel(FilteredPhotoModel photo)
        {
            Model = photo;
            Model.FilteredPhotoChanged += Model_FilteredPhotoChanged;
        }

        ~PhotoViewModel()
        {
            Model.FilteredPhotoChanged -= Model_FilteredPhotoChanged;
        }

        private void Model_FilteredPhotoChanged(object sender, EventArgs e)
        {
            Photo = null;
            Resolution = null;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Filters"));
            }
        }

        private async void UpdatePhoto()
        {
            using (var stream = await Model.GetFilteredPreviewAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);

                Photo = bitmap;
            }
        }

        private async void UpdateResolution()
        {
            Resolution = await Model.GetPhotoResolutionAsync();
        }
    }
}
