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
    public class StreamThumbnailViewModel : INotifyPropertyChanged
    {
        private BitmapImage _thumbnail = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal FilteredPhotoModel Model { get; private set; }

        public BitmapImage Thumbnail
        {
            get
            {
                return _thumbnail;
            }

            private set
            {
                if (_thumbnail != value)
                {
                    _thumbnail = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Thumbnail"));
                    }
                }
            }
        }

        public StreamThumbnailViewModel(FilteredPhotoModel photo)
        {
            Model = new FilteredPhotoModel(photo);
            Model.FilteredThumbnailChanged += Model_FilteredThumbnailChanged;

            UpdateFilteredThumbnailBitmap();
        }

        ~StreamThumbnailViewModel()
        {
            Model.FilteredThumbnailChanged -= Model_FilteredThumbnailChanged;
        }

        private void Model_FilteredThumbnailChanged(object sender, EventArgs e)
        {
            UpdateFilteredThumbnailBitmap();
        }

        private async void UpdateFilteredThumbnailBitmap()
        {
            using (var stream = await Model.GetFilteredThumbnailAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);

                Thumbnail = bitmap;
            }
        }
    }
}
