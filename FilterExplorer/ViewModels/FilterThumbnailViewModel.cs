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
    public class FilterThumbnailViewModel : INotifyPropertyChanged
    {
        private string _title = null;
        private Filter _filter = null;
        private BitmapImage _thumbnail = null;

        public event PropertyChangedEventHandler PropertyChanged;

        internal PhotoModel Model { get; private set; }

        internal Filter Filter
        {
            get
            {
                return _filter;
            }

            private set
            {
                if (_filter != value)
                {
                    _filter = value;

                    Title = new Windows.ApplicationModel.Resources.ResourceLoader().GetString(_filter.Id.Split('.').Last() + "Name");
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            private set
            {
                if (_title != value)
                {
                    _title = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Title"));
                    }
                }
            }
        }

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

        public FilterThumbnailViewModel(PhotoModel photo, Filter filter)
        {
            Model = photo;
            Model.Filters.ItemsChanged += Model_Filters_ItemsChanged;

            Filter = filter;

            UpdateFilteredThumbnailBitmap();
        }

        ~FilterThumbnailViewModel()
        {
            Model.Filters.ItemsChanged -= Model_Filters_ItemsChanged;
        }

        private void Model_Filters_ItemsChanged(object sender, EventArgs e)
        {
            UpdateFilteredThumbnailBitmap();
        }

        private async void UpdateFilteredThumbnailBitmap()
        {
            var modelCopy = new PhotoModel(Model);
            modelCopy.Filters.Add(Filter);

            using (var stream = await modelCopy.GetFilteredThumbnailAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);

                Thumbnail = bitmap;
            }
        }
    }
}
