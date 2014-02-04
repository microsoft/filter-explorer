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

namespace FilterExplorer.Models
{
    public class PhotoModel
    {
        private uint _version = 0;
        private Windows.Storage.StorageFile _file = null;
        private IRandomAccessStream _photoStream = null;
        private IRandomAccessStream _previewStream = null;
        private IRandomAccessStream _thumbnailStream = null;

        public event EventHandler VersionChanged;

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

                    if (VersionChanged != null)
                    {
                        VersionChanged(this, EventArgs.Empty);
                    }
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

            if (other._photoStream != null)
            {
                _photoStream = other._photoStream.CloneStream();
            }

            if (other._previewStream != null)
            {
                _previewStream = other._previewStream.CloneStream();
            }

            if (other._thumbnailStream != null)
            {
                _thumbnailStream = other._thumbnailStream.CloneStream();
            }


            Filters = new ObservableList<Filter>(other.Filters);
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        ~PhotoModel()
        {
            Filters.ItemsChanged -= Filters_ItemsChanged;
        }

        public async Task<IRandomAccessStream> GetPhotoAsync()
        {
            if (_photoStream == null)
            {
                _photoStream = await _file.OpenReadAsync();
            }

            return _photoStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetPreviewAsync()
        {
            if (_previewStream == null)
            {
                using (var stream = await GetPhotoAsync())
                {
                    var buffer = new byte[stream.Size].AsBuffer();
                    await stream.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                    var resizeConfiguration = new AutoResizeConfiguration(3 * 1024 * 1024, new Size(1280, 1280), new Size(0, 0), AutoResizeMode.Automatic, 0.9, ColorSpace.Yuv420);
                    buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                    var resizedStream = new InMemoryRandomAccessStream();
                    await resizedStream.WriteAsync(buffer);

                    _previewStream = resizedStream;
                }
            }

            return _previewStream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetThumbnailAsync()
        {
            if (_thumbnailStream == null)
            {
                _thumbnailStream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);
            }

            return _thumbnailStream.CloneStream();
        }

        private void Filters_ItemsChanged(object sender, EventArgs e)
        {
            Version += 1;
        }
    }
}
