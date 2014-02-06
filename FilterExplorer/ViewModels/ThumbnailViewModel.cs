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
    public class ThumbnailViewModel : ViewModelBase
    {
        private string _title = null;
        private Filter _filter = null;
        private BitmapImage _thumbnail = null;

        internal FilteredPhotoModel Model { get; private set; }

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

                    Title = _filter != null ? _filter.Name : string.Empty;
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

                    Notify("Title");
                }
            }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                if (_thumbnail == null)
                {
                    UpdateThumbnailAsync();
                }

                return _thumbnail;
            }

            private set
            {
                if (_thumbnail != value)
                {
                    _thumbnail = value;

                    Notify("Thumbnail");
                }
            }
        }

        public ThumbnailViewModel(FilteredPhotoModel model, Filter filter)
        {
            Model = new FilteredPhotoModel(model);
            Model.Filters.Add(filter);

            Filter = filter;
        }

        private async void UpdateThumbnailAsync()
        {
            Processing = true;

            using (var stream = await Model.GetFilteredThumbnailAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);

                Thumbnail = bitmap;
            }

            Processing = false;
        }
    }
}
