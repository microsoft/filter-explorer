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
using Nokia.Graphics.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using FilterExplorer.Helpers;

namespace FilterExplorer.Models
{
    public class PhotoModel
    {
        private uint _version = 0;
        private bool _modified = false;
        private Windows.Storage.StorageFile _file = null;
        private IRandomAccessStream _photoStream = null; // TODO change to WeakReference?
        private IRandomAccessStream _thumbnailStream = null; // TODO change to WeakReference?
        private IRandomAccessStream _filteredPhotoStream = null; // TODO change to WeakReference?
        private IRandomAccessStream _filteredThumbnailStream = null; // TODO change to WeakReference?

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

        public ObservableList<Filter> Filters { get; private set; }

        public PhotoModel(Windows.Storage.StorageFile file)
        {
            _file = file;

            Filters = new ObservableList<Filter>();
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public PhotoModel(PhotoModel other)
        {
            _file = other._file;
            _version = other._version;

            if (other._thumbnailStream != null)
            {
                _thumbnailStream = other._thumbnailStream.CloneStream();
            }

            if (other._photoStream != null)
            {
                _photoStream = other._photoStream.CloneStream();
            }

            if (other._filteredThumbnailStream != null)
            {
                _filteredThumbnailStream = other._filteredThumbnailStream.CloneStream();
            }

            if (other._filteredPhotoStream != null)
            {
                _filteredPhotoStream = other._filteredPhotoStream.CloneStream();
            }

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

        public async Task<IRandomAccessStream> GetPhotoAsync()
        {
            if (_photoStream == null)
            {
                using (var stream = await _file.OpenReadAsync())
                {
                    var buffer = new byte[stream.Size].AsBuffer();
                    await stream.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                    var resizeConfiguration = new AutoResizeConfiguration(2 * 1024 * 1024, new Size(1920, 1920), new Size(0, 0), AutoResizeMode.Automatic, 0.9, ColorSpace.Yuv420);
                    buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                    var resizedStream = new InMemoryRandomAccessStream();
                    await resizedStream.WriteAsync(buffer);

                    _photoStream = resizedStream;
                }
            }

            return _photoStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetThumbnailAsync()
        {
            if (_thumbnailStream == null)
            {
                _thumbnailStream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            }

            return _thumbnailStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredPhotoAsync()
        {
            if (_filteredPhotoStream == null)
            {
                var list = new List<IFilter>();

                foreach (var filter in Filters)
                {
                    list.Add(filter.GetFilter());
                }

                using (var stream = await GetPhotoAsync())
                {
                    _filteredPhotoStream = await RenderingHelper.GetFilteredStreamAsync(stream, list);
                }
            }

            return _filteredPhotoStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredThumbnailAsync()
        {
            if (_filteredThumbnailStream == null)
            {
                var list = new List<IFilter>();

                foreach (var filter in Filters)
                {
                    list.Add(filter.GetFilter());
                }

                using (var stream = await GetThumbnailAsync())
                {
                    _filteredThumbnailStream = await RenderingHelper.GetFilteredStreamAsync(stream, list);
                }
            }

            return _filteredThumbnailStream.CloneStream();
        }

        private void Filters_ItemsChanged(object sender, EventArgs e)
        {
            Version += 1;

            if (_filteredPhotoStream != null)
            {
                _filteredPhotoStream.Dispose();
                _filteredPhotoStream = null;
            }

            if (_filteredThumbnailStream != null)
            {
                _filteredThumbnailStream.Dispose();
                _filteredThumbnailStream = null;
            }

            RaiseFilteredPhotoChanged();
            RaiseFilteredThumbnailChanged();
        }

        private void RaiseVersionChanged()
        {
            if (VersionChanged != null)
            {
                VersionChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseFilteredPhotoChanged()
        {
            if (FilteredPhotoChanged != null)
            {
                FilteredPhotoChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseFilteredThumbnailChanged()
        {
            if (FilteredThumbnailChanged != null)
            {
                FilteredThumbnailChanged(this, EventArgs.Empty);
            }
        }
    }
}
