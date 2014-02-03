using FilterExplorer.Filters;
using FilterExplorer.Models;
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

        public event PropertyChangedEventHandler PropertyChanged;

        internal PhotoModel Model { get; private set; }

        public BitmapImage Photo
        {
            get
            {
                if (_photo == null)
                {
                    UpdateFilteredPhotoBitmap();
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

        public PhotoViewModel(PhotoModel photo)
        {
            Model = photo;
            Model.FilteredPhotoChanged += Model_FilteredPhotoChanged;

            UpdateFilteredPhotoBitmap();
        }

        ~PhotoViewModel()
        {
            Model.FilteredPhotoChanged -= Model_FilteredPhotoChanged;
        }

        private void Model_FilteredPhotoChanged(object sender, EventArgs e)
        {
            if (_photo != null)
            {
                UpdateFilteredPhotoBitmap();
            }
        }

        private async void UpdateFilteredPhotoBitmap()
        {
            using (var stream = await Model.GetFilteredPhotoAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);

                Photo = bitmap;
            }
        }
    }
}
