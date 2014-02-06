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
        private TaskResultCache<IRandomAccessStream> _photoCache = new TaskResultCache<IRandomAccessStream>();
        private TaskResultCache<IRandomAccessStream> _previewCache = new TaskResultCache<IRandomAccessStream>();
        private TaskResultCache<IRandomAccessStream> _thumbnailCache = new TaskResultCache<IRandomAccessStream>();

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
        }

        public PhotoModel(PhotoModel other)
        {
            _file = other._file;
        }

        ~PhotoModel()
        {
            _photoCache.Invalidate();
            _previewCache.Invalidate();
            _thumbnailCache.Invalidate();
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
            if (_photoCache.Pending)
            {
                await _photoCache.WaitAsync();
            }
            else if (_photoCache.Result == null)
            {
                await _photoCache.Execute(GetPhotoStreamAsync());
            }

            return _photoCache.Result.CloneStream();
        }

        public async Task<IRandomAccessStream> GetPreviewAsync()
        {
            if (_previewCache.Pending)
            {
                await _previewCache.WaitAsync();
            }
            else if (_previewCache.Result == null)
            {
                await _previewCache.Execute(GetPreviewStreamAsync());
            }

            return _previewCache.Result.CloneStream();
        }

        public async Task<IRandomAccessStream> GetThumbnailAsync()
        {
            if (_thumbnailCache.Pending)
            {
                await _thumbnailCache.WaitAsync();
            }
            else if (_thumbnailCache.Result == null)
            {
                await _thumbnailCache.Execute(GetThumbnailStreamAsync());
            }

            return _thumbnailCache.Result.CloneStream();
        }

        private async Task<IRandomAccessStream> GetPhotoStreamAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetPhotoStreamAsync invoked " + this.GetHashCode());
#endif

            return await _file.OpenReadAsync();
        }

        private async Task<IRandomAccessStream> GetPreviewStreamAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetPreviewStreamAsync invoked " + this.GetHashCode());
#endif

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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetThumbnailStreamAsync invoked " + this.GetHashCode());
#endif

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
