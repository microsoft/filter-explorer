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
    public class FilteredPhotoModel
    {
        private PhotoModel _photo = null;
        private uint _version = 0;
        private RandomAccessStreamCache<IRandomAccessStream> _filteredPhotoCache = null;
        private RandomAccessStreamCache<IRandomAccessStream> _filteredPreviewCache = null;
        private RandomAccessStreamCache<IRandomAccessStream> _filteredThumbnailCache = null;

        public event EventHandler FilteredPhotoChanged;
        public event EventHandler FilteredPreviewChanged;
        public event EventHandler FilteredThumbnailChanged;

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

        public StorageFile File
        {
            get
            {
                return _photo.File;
            }
        }

        public ObservableList<Filter> Filters { get; private set; }

        public FilteredPhotoModel(Windows.Storage.StorageFile file)
        {
            _photo = new PhotoModel(file);
            _filteredPhotoCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _filteredPreviewCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _filteredThumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>();

            Filters = new ObservableList<Filter>();
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public FilteredPhotoModel(Windows.Storage.StorageFile file, ObservableList<Filter> filters)
        {
            _photo = new PhotoModel(file, filters);
            _filteredPhotoCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _filteredPreviewCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _filteredThumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>();

            Filters = filters;
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public FilteredPhotoModel(FilteredPhotoModel other)
        {
            _photo = other._photo;
            _filteredPhotoCache = new RandomAccessStreamCache<IRandomAccessStream>(other._filteredPhotoCache);
            _filteredPreviewCache = new RandomAccessStreamCache<IRandomAccessStream>(other._filteredPreviewCache);
            _filteredThumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>(other._filteredThumbnailCache);

            Filters = new ObservableList<Filter>(other.Filters);
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public FilteredPhotoModel(PhotoModel photo)
        {
            _photo = photo;
            _filteredPhotoCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _filteredPreviewCache = new RandomAccessStreamCache<IRandomAccessStream>();
            _filteredThumbnailCache = new RandomAccessStreamCache<IRandomAccessStream>();

            Filters = new ObservableList<Filter>();
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        ~FilteredPhotoModel()
        {
            Filters.ItemsChanged -= Filters_ItemsChanged;
        }

        public async Task<Size?> GetPhotoResolutionAsync()
        {
            return await _photo.GetPhotoResolutionAsync();
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetFilteredPhotoStreamAsync invoked " + this.GetHashCode());
#endif

            IRandomAccessStream filteredStream = null;

            using (var stream = await _photo.GetPhotoAsync())
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetFilteredPreviewStreamAsync invoked " + this.GetHashCode());
#endif

            IRandomAccessStream filteredStream = null;

            using (var stream = await _photo.GetPreviewAsync())
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
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetFilteredThumbnailStreamAsync invoked " + this.GetHashCode());
#endif

            IRandomAccessStream filteredStream = null;

            using (var stream = await _photo.GetThumbnailAsync())
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

        private void Filters_ItemsChanged(object sender, EventArgs e)
        {
            Version += 1;

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
