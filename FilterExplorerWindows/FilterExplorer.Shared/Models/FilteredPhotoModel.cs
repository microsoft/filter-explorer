/*
 * Copyright (c) 2014 Microsoft Mobile
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using System.Linq;
using FilterExplorer.Filters;
using FilterExplorer.Utilities;
using Lumia.Imaging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;

namespace FilterExplorer.Models
{
    public class FilteredPhotoModel : IDisposable
    {
        private PhotoModel _photo = null;
        private uint _version = 0;
        private TaskResultCache<IRandomAccessStream> _photoCache = new TaskResultCache<IRandomAccessStream>();
        private TaskResultCache<IRandomAccessStream> _previewCache = new TaskResultCache<IRandomAccessStream>();
        private TaskResultCache<IRandomAccessStream> _thumbnailCache = new TaskResultCache<IRandomAccessStream>();

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

            Filters = new ObservableList<Filter>();
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public FilteredPhotoModel(Windows.Storage.StorageFile file, ObservableList<Filter> filters)
        {
            _photo = new PhotoModel(file);

            Filters = filters;
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public FilteredPhotoModel(FilteredPhotoModel other)
        {
            _photo = other._photo;

            Filters = new ObservableList<Filter>(other.Filters);
            Filters.ItemsChanged += Filters_ItemsChanged;
        }

        public FilteredPhotoModel(PhotoModel photo)
        {
            _photo = photo;

            Filters = new ObservableList<Filter>();
            Filters.ItemsChanged += Filters_ItemsChanged;
        }
        
        public async Task<Size?> GetPhotoResolutionAsync()
        {
            return await _photo.GetPhotoResolutionAsync();
        }

        public async Task<IRandomAccessStream> GetFilteredPhotoAsync()
        {
            if (_photoCache.Pending)
            {
                await _photoCache.WaitAsync();
            }
            else if (_photoCache.Result == null || _photoCache.Version != Version)
            {
                _photoCache.Invalidate();

                try
                {
                    await _photoCache.Execute(GetFilteredPhotoStreamAsync(), Version);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("GetFilteredPhotoStreamAsync threw: " + ex.Message);
                }
            }

            return _photoCache.Result.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredPreviewAsync()
        {
            if (_previewCache.Pending)
            {
                await _previewCache.WaitAsync();
            }
            else if (_previewCache.Result == null || _previewCache.Version != Version)
            {
                _previewCache.Invalidate();

                try
                {
                    await _previewCache.Execute(GetFilteredPreviewStreamAsync(), Version);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("GetFilteredPreviewStreamAsync threw: " + ex.Message);
                }
            }

            return _previewCache.Result.CloneStream();
        }

        public async Task<IRandomAccessStream> GetFilteredThumbnailAsync()
        {
            if (_thumbnailCache.Pending)
            {
                await _thumbnailCache.WaitAsync();
            }
            else if (_thumbnailCache.Result == null || _thumbnailCache.Version != Version)
            {
                _thumbnailCache.Invalidate();

                try
                {
                    await _thumbnailCache.Execute(GetFilteredThumbnailStreamAsync(), Version);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("GetFilteredThumbnailStreamAsync threw: " + ex.Message);
                }
            }

            return _thumbnailCache.Result.CloneStream();
        }

        private async Task<IRandomAccessStream> GetFilteredPhotoStreamAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetFilteredPhotoStreamAsync invoked " + this.GetHashCode());
#endif

            IRandomAccessStream filteredStream = null;

            using (var ticket = await TicketManager.AcquireTicket())
            using (var stream = await _photo.GetPhotoAsync())
            {
                if (Filters.Count > 0)
                {
                    var list = Filters.Select(filter => filter.GetFilter()).ToList();

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

        private async Task<IRandomAccessStream> GetFilteredPreviewStreamAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetFilteredPreviewStreamAsync invoked " + this.GetHashCode());
#endif

            IRandomAccessStream filteredStream = null;

            using (var ticket = await TicketManager.AcquireTicket())
            using (var stream = await _photo.GetPreviewAsync())
            {
                if (Filters.Count > 0)
                {
                    var list = Filters.Select(filter => filter.GetFilter()).ToList();

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

        private async Task<IRandomAccessStream> GetFilteredThumbnailStreamAsync()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("GetFilteredThumbnailStreamAsync invoked " + this.GetHashCode());
#endif

            IRandomAccessStream filteredStream = null;

            using (var ticket = await TicketManager.AcquireTicket())
            using (var stream = await _photo.GetThumbnailAsync())
            {
                if (Filters.Count > 0)
                {
                    var list = Filters.Select(filter => filter.GetFilter()).ToList();

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

        private  void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            Filters.ItemsChanged -= Filters_ItemsChanged;
            Filters.Clear();
        }

        public void Dispose()
        {
            Dispose(true);
        }

    }
}
