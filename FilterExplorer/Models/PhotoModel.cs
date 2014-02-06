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
        private Windows.Storage.StorageFile _file = null;
        private Windows.Storage.FileProperties.ImageProperties _properties = null;
        private RandomAccessStreamCache<IRandomAccessStreamWithContentType> _photoCache = null;
        private RandomAccessStreamCache<IRandomAccessStream> _previewCache = null;
        private RandomAccessStreamCache<IRandomAccessStream> _thumbnailCache = null;

        internal Windows.Storage.StorageFile File
        {
            get
            {
                return _file;
            }
        }

        public PhotoModel(Windows.Storage.StorageFile file)
        {
            _file = file;
            _photoCache = new RandomAccessStreamCache<IRandomAccessStreamWithContentType>();
            _previewCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _thumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>();
        }

        public PhotoModel(Windows.Storage.StorageFile file, ObservableList<Filter> filters)
        {
            _file = file;
            _photoCache = new RandomAccessStreamCache<IRandomAccessStreamWithContentType>();
            _previewCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _thumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>();
        }

        public PhotoModel(PhotoModel other)
        {
            _file = other._file;
            _photoCache = new RandomAccessStreamCache<IRandomAccessStreamWithContentType>(other._photoCache);
            _previewCache = new RandomAccessStreamCache<IRandomAccessStream>(other._previewCache);
            _thumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>(other._thumbnailCache);
        }

        ~PhotoModel()
        {
        }

        public async Task<Size?> GetPhotoResolutionAsync()
        {
            Size? size = null; ;

            if (_properties == null)
            {
                _properties = await _file.Properties.GetImagePropertiesAsync();
            }

            if (_properties != null)
            {
                size = new Size(_properties.Width, _properties.Height);
            }

            return size;
        }

        public async Task<IRandomAccessStream> GetPhotoAsync()
        {
            if (_photoCache.Task != null)
            {
                await Task.WhenAll(_photoCache.Task);
            }

            if (_photoCache.Stream == null)
            {
                _photoCache.Invalidate();
                _photoCache.Task = _file.OpenReadAsync().AsTask();
                _photoCache.Stream = await _photoCache.Task;
                _photoCache.Task = null;
            }

            return _photoCache.Stream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetPreviewAsync()
        {
            if (_previewCache.Task != null)
            {
                await Task.WhenAll(_previewCache.Task);
            }

            if (_previewCache.Stream == null)
            {
                _previewCache.Invalidate();
                _previewCache.Task = GetPreviewStreamAsync();
                _previewCache.Stream = await _previewCache.Task;
                _previewCache.Task = null;
            }

            return _previewCache.Stream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetThumbnailAsync()
        {
            if (_thumbnailCache.Task != null)
            {
                await Task.WhenAll(_thumbnailCache.Task);
            }

            if (_thumbnailCache.Stream == null)
            {
                _thumbnailCache.Invalidate();
                _thumbnailCache.Task = GetThumbnailStreamAsync();
                _thumbnailCache.Stream = await _thumbnailCache.Task;
                _thumbnailCache.Task = null;
            }

            return _thumbnailCache.Stream.CloneStream();
        }

        private async Task<IRandomAccessStream> GetPreviewStreamAsync()
        {
            var size = await GetPhotoResolutionAsync();

            if (size.HasValue && (size.Value.Width > 1920 || size.Value.Height > 1920))
            {
                using (var stream = await GetPhotoAsync())
                {
                    var buffer = new byte[stream.Size].AsBuffer();
                    await stream.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                    var resizeConfiguration = new AutoResizeConfiguration(5 * 1024 * 1024, new Size(1920, 1920), new Size(0, 0), AutoResizeMode.Automatic, 0.9, ColorSpace.Yuv420);
                    buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                    var resizedStream = new InMemoryRandomAccessStream();
                    await resizedStream.WriteAsync(buffer);

                    return resizedStream;
                }
            }
            else
            {
                return await GetPhotoAsync();
            }
        }

        private async Task<IRandomAccessStream> GetThumbnailStreamAsync()
        {
            var stream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView);

            if (stream.ContentType == "image/jpeg")
            {
                return stream;
            }
            else
            {
                // Imaging SDK does not handle BMP well at the moment, convert BMP to JPEG

                using (var photo = await GetPhotoAsync())
                {
                    var buffer = new byte[photo.Size].AsBuffer();
                    await photo.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                    var resizeConfiguration = new AutoResizeConfiguration(512 * 1024, new Size(300, 300), new Size(0, 0), AutoResizeMode.Automatic, 0.8, ColorSpace.Yuv420);
                    buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                    var resizedStream = new InMemoryRandomAccessStream();
                    await resizedStream.WriteAsync(buffer);

                    return resizedStream;
                }
            }
        }
    }
}
