using FilterExplorer.Filters;
using FilterExplorer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.Models
{
    public class PhotoModel
    {
        private uint _version = 0;
        private bool _modified = false;
        private Windows.Storage.StorageFile _file = null;
        private IRandomAccessStream _thumbnailStream = null; // TODO change to WeakReference?
        private IRandomAccessStream _photoStream = null; // TODO change to WeakReference?
        private IRandomAccessStream _filteredThumbnailStream = null; // TODO change to WeakReference?
        private IRandomAccessStream _filteredPhotoStream = null; // TODO change to WeakReference?

        public event EventHandler VersionChanged;
        public event EventHandler ModifiedChanged;
        public event EventHandler FilteredPhotoChanged;
        public event EventHandler FilteredThumbnailChanged;

        public uint Version
        {
            get
            {
                return _version;
            }

            private set
            {
                if (_version != value)
                {
                    _version = value;

                    RaiseVersionChanged();
                }
            }
        }

        public bool Modified
        {
            get
            {
                return _modified;
            }

            internal set
            {
                if (_modified != value)
                {
                    _modified = value;

                    RaiseModifiedChanged();
                }
            }
        }

        public bool FromFilesystem { get; private set; }

        public ObservableList<Filter> Filters { get; private set; }

        public PhotoModel(Windows.Storage.StorageFile file)
        {
            _file = file;

            FromFilesystem = true;

            Filters = new ObservableList<Filter>();
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public PhotoModel(PhotoModel other)
        {
            _file = other._file;
            _version = other._version;
            _thumbnailStream = other._thumbnailStream;
            _photoStream = other._photoStream;
            _filteredThumbnailStream = other._filteredThumbnailStream;
            _filteredPhotoStream = other._filteredPhotoStream;

            FromFilesystem = other.FromFilesystem;
            Modified = other.Modified;

            Filters = new ObservableList<Filter>(other.Filters);
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        ~PhotoModel()
        {
            Filters.ItemsChanged -= Filters_ItemsChanged;

            if (_photoStream != null)
            {
                _photoStream.Dispose();
                _photoStream = null;
            }

            if (_thumbnailStream != null)
            {
                _thumbnailStream.Dispose();
                _thumbnailStream = null;
            }
        }

        public async Task<IRandomAccessStream> GetThumbnailAsync()
        {
            if (_thumbnailStream == null)
            {
                _thumbnailStream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            }

            return _thumbnailStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetPhotoAsync()
        {
            if (_photoStream == null)
            {
                _photoStream = await _file.OpenReadAsync();
            }

            return _photoStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredThumbnailAsync()
        {
            if (_filteredThumbnailStream == null)
            {
                _filteredThumbnailStream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            }

            return _filteredThumbnailStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredPhotoAsync()
        {
            if (_filteredPhotoStream == null)
            {
                _filteredPhotoStream = await _file.OpenReadAsync();
            }

            return _filteredPhotoStream.CloneStream();
        }

        private void Filters_ItemsChanged(object sender, EventArgs e)
        {
            Version += 1;
            Modified = Filters.Count > 0 || !FromFilesystem;

            RaiseFilteredThumbnailChanged();
            RaiseFilteredPhotoChanged();
        }

        private void RaiseModifiedChanged()
        {
            if (ModifiedChanged != null)
            {
                ModifiedChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseVersionChanged()
        {
            if (VersionChanged != null)
            {
                VersionChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseFilteredThumbnailChanged()
        {
            if (FilteredThumbnailChanged != null)
            {
                FilteredThumbnailChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseFilteredPhotoChanged()
        {
            if (FilteredPhotoChanged != null)
            {
                FilteredPhotoChanged(this, EventArgs.Empty);
            }
        }
    }
}
