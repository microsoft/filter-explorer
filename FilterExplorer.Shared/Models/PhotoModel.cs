/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Utilities;
using Nokia.Graphics.Imaging;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;

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
            var maximumSide = (int)Windows.UI.Xaml.Application.Current.Resources["PreviewSide"];

            if (size.HasValue && (size.Value.Width > maximumSide || size.Value.Height > maximumSide))
            {
                using (var stream = await GetPhotoAsync())
                {
                    return await ResizeStreamAsync(stream, new Size(maximumSide, maximumSide));
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

            var maximumSide = (int)Windows.UI.Xaml.Application.Current.Resources["ThumbnailSide"];

            //using (var preview = await GetPreviewAsync())
            //using (var resizedStream = new InMemoryRandomAccessStream())
            //{
            //    var buffer = new byte[preview.Size].AsBuffer();

            //    await preview.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

            //    var resizeConfiguration = new AutoResizeConfiguration(
            //        (uint)(maximumSide * maximumSide * 4 * 2), new Size(maximumSide, maximumSide),
            //        new Size(0, 0), AutoResizeMode.Automatic, 0.7, ColorSpace.Yuv420);

            //    buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

            //    await resizedStream.WriteAsync(buffer);

            //    return resizedStream.CloneStream();
            //}

            using (var stream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView))
            {
                if (stream.ContentType == "image/jpeg")
                {
                    if (stream.OriginalWidth <= maximumSide || stream.OriginalHeight <= maximumSide)
                    {
                        using (var memoryStream = new InMemoryRandomAccessStream())
                        {
                            using (var reader = new DataReader(stream))
                            using (var writer = new DataWriter(memoryStream))
                            {
                                await reader.LoadAsync((uint)stream.Size);
                                var buffer = reader.ReadBuffer((uint)stream.Size);

                                writer.WriteBuffer(buffer);
                                await writer.StoreAsync();
                                await writer.FlushAsync();

                                return memoryStream.CloneStream();
                            }
                        }
                    }
                    else
                    {
                        return await ResizeStreamAsync(stream, new Size(maximumSide, maximumSide));
                    }
                }
                else
                {
                    // Imaging SDK does not handle BMP well at the moment, convert BMP to JPEG

                    using (var photo = await GetPreviewAsync())
                    {
                        return await ResizeStreamAsync(stream, new Size(maximumSide, maximumSide));
                    }

                    //using (var resizedStream = new InMemoryRandomAccessStream())
                    //{
                    //    var buffer = new byte[photo.Size].AsBuffer();

                    //    await photo.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                    //    var resizeConfiguration = new AutoResizeConfiguration(
                    //        512 * 1024, new Size(maximumSide, maximumSide),
                    //        new Size(0, 0), AutoResizeMode.Automatic, 0.8, ColorSpace.Yuv420);

                    //    buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                    //    await resizedStream.WriteAsync(buffer);

                    //    return resizedStream.CloneStream();
                    //}
                }
            }
        }

        private async Task<IRandomAccessStream> ResizeStreamAsync(IRandomAccessStream stream, Size size)
        {
            using (var resizedStream = new InMemoryRandomAccessStream())
            {
                var buffer = new byte[stream.Size].AsBuffer();

                await stream.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                var resizeConfiguration = new AutoResizeConfiguration(
                    (uint)(size.Width * size.Height * 4 * 2), size,
                    new Size(0, 0), AutoResizeMode.Automatic, 0.7, ColorSpace.Yuv420);

                buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                await resizedStream.WriteAsync(buffer);

                return resizedStream.CloneStream();
            }
        }
    }
}
