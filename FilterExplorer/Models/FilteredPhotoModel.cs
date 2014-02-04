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
    public class FilteredPhotoModel : PhotoModel
    {
        private class PhotoCache
        {
            public uint Version { get; set; }
            public IRandomAccessStream Stream { get; set; }
            public Task<IRandomAccessStream> Task { get; set; }

            public PhotoCache()
            {
            }

            public PhotoCache(PhotoCache other)
            {
                Version = other.Version;

                if (other.Stream != null)
                {
                    Stream = other.Stream.CloneStream();
                }
            }

            public void Invalidate()
            {
                if (Stream != null)
                {
                    Stream.Dispose();
                    Stream = null;
                }

                if (Task != null)
                {
                    Task.AsAsyncOperation().Cancel();
                    Task = null;
                }
            }

            ~PhotoCache()
            {
                Invalidate();
            }
        }

        private PhotoCache _filteredPhotoCache = null;
        private PhotoCache _filteredPreviewCache = null;
        private PhotoCache _filteredThumbnailCache = null;

        public event EventHandler FilteredPhotoChanged;
        public event EventHandler FilteredPreviewChanged;
        public event EventHandler FilteredThumbnailChanged;

        public FilteredPhotoModel(Windows.Storage.StorageFile file)
            : base(file)
        {
            _filteredPhotoCache = new PhotoCache();
            _filteredPreviewCache = new PhotoCache();
            _filteredThumbnailCache = new PhotoCache();

            base.VersionChanged += PhotoModel_VersionChanged;
        }

        public FilteredPhotoModel(FilteredPhotoModel other)
            : base(other)
        {
            _filteredPhotoCache = new PhotoCache(other._filteredPhotoCache);
            _filteredPreviewCache = new PhotoCache(other._filteredPreviewCache);
            _filteredThumbnailCache = new PhotoCache(other._filteredThumbnailCache);

            base.VersionChanged += PhotoModel_VersionChanged;
        }

        public FilteredPhotoModel(PhotoModel other)
            : base(other)
        {
            base.VersionChanged += PhotoModel_VersionChanged;
        }

        ~FilteredPhotoModel()
        {
            VersionChanged -= PhotoModel_VersionChanged;
        }

        public async Task<IRandomAccessStream> GetFilteredPhotoAsync()
        {
            if (_filteredPhotoCache.Task != null)
            {
                await Task.WhenAll(_filteredPhotoCache.Task);
            }

            if (_filteredPhotoCache.Stream == null || _filteredPhotoCache.Version != Version)
            {
                _filteredPhotoCache.Invalidate();
                _filteredPhotoCache.Version = Version;
                _filteredPhotoCache.Task = GetFilteredPhotoStreamAsync();
                _filteredPhotoCache.Stream = await _filteredPhotoCache.Task;
                _filteredPhotoCache.Task = null;
            }

            return _filteredPhotoCache.Stream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredPreviewAsync()
        {
            if (_filteredPreviewCache.Task != null)
            {
                await Task.WhenAll(_filteredPreviewCache.Task);
            }

            if (_filteredPreviewCache.Stream == null || _filteredPreviewCache.Version != Version)
            {
                _filteredPreviewCache.Invalidate();
                _filteredPreviewCache.Version = Version;
                _filteredPreviewCache.Task = GetFilteredPreviewStreamAsync();
                _filteredPreviewCache.Stream = await _filteredPreviewCache.Task;
                _filteredPreviewCache.Task = null;
            }

            return _filteredPreviewCache.Stream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredThumbnailAsync()
        {
            if (_filteredThumbnailCache.Task != null)
            {
                await Task.WhenAll(_filteredThumbnailCache.Task);
            }

            if (_filteredThumbnailCache.Stream == null || _filteredThumbnailCache.Version != Version)
            {
                _filteredThumbnailCache.Invalidate();
                _filteredThumbnailCache.Version = Version;
                _filteredThumbnailCache.Task = GetFilteredThumbnailStreamAsync();
                _filteredThumbnailCache.Stream = await _filteredThumbnailCache.Task;
                _filteredThumbnailCache.Task = null;
            }

            return _filteredThumbnailCache.Stream.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredPhotoStreamAsync()
        {
            System.Diagnostics.Debug.WriteLine("GetFilteredPhotoStreamAsync invoked " + this.GetHashCode());

            IRandomAccessStream filteredStream = null;

            using (var stream = await GetPhotoAsync())
            {
                if (Filters.Count > 0)
                {
                    var list = new List<IFilter>();

                    foreach (var filter in Filters)
                    {
                        list.Add(filter.GetFilter());
                    }

                    filteredStream = new InMemoryRandomAccessStream();

                    using (var source = new RandomAccessStreamImageSource(stream))
                    using (var effect = new FilterEffect(source) { Filters = list })
                    using (var renderer = new JpegRenderer(effect))
                    {
                        var buffer = await renderer.RenderAsync();

                        await filteredStream.WriteAsync(buffer);
                    }
                }
                else
                {
                    filteredStream = stream.CloneStream();
                }
            }

            return filteredStream;
        }

        public async Task<IRandomAccessStream> GetFilteredPreviewStreamAsync()
        {
            System.Diagnostics.Debug.WriteLine("GetFilteredPreviewStreamAsync invoked " + this.GetHashCode());

            IRandomAccessStream filteredStream = null;

            using (var stream = await GetPreviewAsync())
            {
                if (Filters.Count > 0)
                {
                    var list = new List<IFilter>();

                    foreach (var filter in Filters)
                    {
                        list.Add(filter.GetFilter());
                    }

                    filteredStream = new InMemoryRandomAccessStream();

                    using (var source = new RandomAccessStreamImageSource(stream))
                    using (var effect = new FilterEffect(source) { Filters = list })
                    using (var renderer = new JpegRenderer(effect))
                    {
                        var buffer = await renderer.RenderAsync();

                        await filteredStream.WriteAsync(buffer);
                    }
                }
                else
                {
                    filteredStream = stream.CloneStream();
                }
            }

            return filteredStream;
        }

        public async Task<IRandomAccessStream> GetFilteredThumbnailStreamAsync()
        {
            System.Diagnostics.Debug.WriteLine("GetFilteredThumbnailStreamAsync invoked " + this.GetHashCode());

            IRandomAccessStream filteredStream = null;

            using (var stream = await GetThumbnailAsync())
            {
                if (Filters.Count > 0)
                {
                    var list = new List<IFilter>();

                    foreach (var filter in Filters)
                    {
                        list.Add(filter.GetFilter());
                    }

                    filteredStream = new InMemoryRandomAccessStream();

                    using (var source = new RandomAccessStreamImageSource(stream))
                    using (var effect = new FilterEffect(source) { Filters = list })
                    using (var renderer = new JpegRenderer(effect))
                    {
                        var buffer = await renderer.RenderAsync();

                        await filteredStream.WriteAsync(buffer);
                    }
                }
                else
                {
                    filteredStream = stream.CloneStream();
                }
            }

            return filteredStream;
        }

        private void PhotoModel_VersionChanged(object sender, EventArgs e)
        {
            RaiseFilteredPhotoChanged();
            RaiseFilteredPreviewChanged();
            RaiseFilteredThumbnailChanged();
        }

        private void RaiseFilteredPhotoChanged()
        {
            if (FilteredPhotoChanged != null)
            {
                FilteredPhotoChanged(this, EventArgs.Empty);
            }
        }

        private void RaiseFilteredPreviewChanged()
        {
            if (FilteredPreviewChanged != null)
            {
                FilteredPreviewChanged(this, EventArgs.Empty);
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
