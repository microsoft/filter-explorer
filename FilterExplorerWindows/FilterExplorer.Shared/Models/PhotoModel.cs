/*
 * Copyright (c) 2014 Microsoft Mobile. All rights reserved.
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
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Streams;

namespace FilterExplorer.Models
{
    public class PhotoModel
    {
        private StorageFile _file = null;
        private TaskResultCache<ImageProperties> _propertiesCache = new TaskResultCache<ImageProperties>();
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
            _propertiesCache.Invalidate();
            _photoCache.Invalidate();
            _previewCache.Invalidate();
            _thumbnailCache.Invalidate();
        }

        public async Task<Size?> GetPhotoResolutionAsync()
        {
            Size? size = null;

            if (_propertiesCache.Pending)
            {
                await _propertiesCache.WaitAsync();
            }
            else if (_propertiesCache.Result == null)
            {
                await _propertiesCache.Execute(_file.Properties.GetImagePropertiesAsync().AsTask());
            }

            if (_propertiesCache.Result != null)
            {
                size = new Size(_propertiesCache.Result.Width, _propertiesCache.Result.Height);
            }

            return size;
        }

        public async Task<PhotoOrientation?> GetPhotoOrientationAsync()
        {
            PhotoOrientation? orientation = null;

            if (_propertiesCache.Pending)
            {
                await _propertiesCache.WaitAsync();
            }
            else if (_propertiesCache.Result == null)
            {
                await _propertiesCache.Execute(_file.Properties.GetImagePropertiesAsync().AsTask());
            }

            if (_propertiesCache.Result != null)
            {
                orientation = _propertiesCache.Result.Orientation;
            }

            return orientation;
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

            using (var stream = await GetPhotoAsync())
            {
                var orientation = await GetPhotoOrientationAsync();
                var orientationValue = orientation.HasValue ? orientation.Value : PhotoOrientation.Unspecified;

                if (size.HasValue && (size.Value.Width > maximumSide || size.Value.Height > maximumSide) || orientationValue != PhotoOrientation.Normal)
                {
                    return await ResizeStreamAsync(stream, new Size(maximumSide, maximumSide), orientationValue);
                }
                else
                {
                    return stream.CloneStream();
                }
            }
        }

        private async Task<IRandomAccessStream> GetThumbnailStreamAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetThumbnailStreamAsync invoked " + this.GetHashCode());
#endif

            var maximumSide = (int)Windows.UI.Xaml.Application.Current.Resources["ThumbnailSide"];
            var orientation = await GetPhotoOrientationAsync();
            var orientationValue = orientation.HasValue ? orientation.Value : PhotoOrientation.Unspecified;

            using (var stream = await _file.GetThumbnailAsync(Windows.Storage.FileProperties.ThumbnailMode.PicturesView))
            {
                if (stream.ContentType == "image/jpeg")
                {
                    if ((stream.OriginalWidth <= maximumSide || stream.OriginalHeight <= maximumSide) && orientationValue == PhotoOrientation.Normal)
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
                        return await ResizeStreamAsync(stream, new Size(maximumSide, maximumSide), orientationValue);
                    }
                }
                else
                {
                    using (var preview = await GetPreviewAsync())
                    {
                        return await ResizeStreamAsync(preview, new Size(maximumSide, maximumSide), orientationValue);
                    }
                }
            }
        }

        private async Task<IRandomAccessStream> ResizeStreamAsync(IRandomAccessStream stream, Size size, PhotoOrientation orientation)
        {
            var rotation = 0;

            switch (orientation)
            {
                case PhotoOrientation.Rotate180:
                    {
                        rotation = -180;
                    };
                    break;

                case PhotoOrientation.Rotate270:
                    {
                        rotation = -270;
                    };
                    break;

                case PhotoOrientation.Rotate90:
                    {
                        rotation = -90;
                    };
                    break;
            }

            using (var resizedStream = new InMemoryRandomAccessStream())
            {
                var buffer = new byte[stream.Size].AsBuffer();

                stream.Seek(0);

                await stream.ReadAsync(buffer, buffer.Length, InputStreamOptions.None);

                var resizeConfiguration = new AutoResizeConfiguration(
                    (uint)(size.Width * size.Height * 4 * 2), size,
                    new Size(0, 0), AutoResizeMode.Automatic, 0.7, ColorSpace.Yuv420);

                buffer = await JpegTools.AutoResizeAsync(buffer, resizeConfiguration);

                await resizedStream.WriteAsync(buffer);
                await resizedStream.FlushAsync();

                if (rotation != 0)
                {
                    resizedStream.Seek(0);

                    var filters = new List<IFilter>() { new RotationFilter(rotation) };

                    using (var source = new RandomAccessStreamImageSource(resizedStream))
                    using (var effect = new FilterEffect(source) { Filters = filters })
                    using (var renderer = new JpegRenderer(effect))
                    {
                        buffer = await renderer.RenderAsync();

                        using (var rotatedResizedStream = new InMemoryRandomAccessStream())
                        {
                            await rotatedResizedStream.WriteAsync(buffer);
                            await rotatedResizedStream.FlushAsync();

                            return rotatedResizedStream.CloneStream();
                        }
                    }
                }
                else
                {
                    return resizedStream.CloneStream();
                }
            }
        }
    }
}
