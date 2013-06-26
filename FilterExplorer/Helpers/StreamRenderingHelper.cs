using Nokia.Graphics.Imaging;
using Nokia.InteropServices.WindowsRuntime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows.Media.Imaging;

namespace ImageProcessingApp.Models
{
    /// <summary>
    /// Helper class for asynchronously rendering many images in a fast pace. Rendering is done
    /// by invoking many single quick rendering operations in the calling thread's dispatcher.
    /// </summary>
    public class StreamRenderingHelper : INotifyPropertyChanged
    {
        #region Members

        private List<StreamItemViewModel> _priorityQueue = new List<StreamItemViewModel>();
        private List<StreamItemViewModel> _standardQueue = new List<StreamItemViewModel>();
        private bool _enabled = false;
        private bool _busy = false;
        private int _processingNow = 0;
        private int _processingMax = 18;

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public bool Busy
        {
            get
            {
                return _busy;
            }

            set
            {
                if (_busy != value)
                {
                    _busy = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Busy"));
                    }
                }
            }
        }

        /// <summary>
        /// Enable or disable processing of rendering queue.
        /// </summary>
        public bool Enabled
        {
            get
            {
                return _enabled;
            }

            set
            {
                if (_enabled != value)
                {
                    _enabled = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Enabled"));
                    }

                    if (_enabled)
                    {
                        EnsureProcessing();
                    }
                }
            }
        }

        /// <summary>
        /// Total amoun of photo stream items currently in the rendering queue.
        /// </summary>
        public int Count
        {
            get
            {
                return _priorityQueue.Count + _standardQueue.Count;
            }
        }

        #endregion

        /// <summary>
        /// Adds a photo stream item to the rendering queue.
        /// 
        /// Item's void TransitionToImage(WriteableBitmap) will be called when rendering has been completed.
        /// </summary>
        /// <param name="item">Photo stream item to add</param>
        /// <param name="priority">True if rendering this item is high priority, otherwise false</param>
        public void Add(StreamItemViewModel item, bool priority)
        {
            if (priority)
            {
                _priorityQueue.Add(item);
            }
            else
            {
                _standardQueue.Add(item);
            }

            EnsureProcessing();
        }

        public bool Contains(StreamItemViewModel item)
        {
            return _priorityQueue.Contains(item) || _standardQueue.Contains(item);
        }

        public void Remove(StreamItemViewModel item)
        {
            if (_priorityQueue.Contains(item))
            {
                _priorityQueue.Remove(item);
            }

            if (_standardQueue.Contains(item))
            {
                _standardQueue.Remove(item);
            }
        }

        public void RemoveAll()
        {
            _standardQueue.Clear();
            _priorityQueue.Clear();
        }

        #region Private methods

        /// <summary>
        /// Ensures that images are being rendered.
        /// </summary>
        private void EnsureProcessing()
        {
            for (int i = 0; _enabled && _processingNow < _processingMax && _processingNow < Count; i++)
            {
                Process();
            }
        }

        /// <summary>
        /// Begins a photo stream item rendering process loop. Loop is executed asynchronously item by item
        /// until there are no more items in the queue.
        /// </summary>
        private async void Process()
        {
            _processingNow++;

            while (_enabled && Count > 0)
            {
                StreamItemViewModel item;

                if (_priorityQueue.Count > 0)
                {
                    Busy = true;

                    item = _priorityQueue[0];

                    _priorityQueue.RemoveAt(0);
                }
                else
                {
                    item = _standardQueue[0];

                    _standardQueue.RemoveAt(0);
                }

                try
                {
                    WriteableBitmap bitmap = null;

                    using (MemoryStream thumbnailStream = new MemoryStream())
                    {
                        System.Diagnostics.Debug.Assert(item.RequestedSize != StreamItemViewModel.Size.None);

                        if (item.RequestedSize == StreamItemViewModel.Size.Large)
                        {
                            bitmap = new WriteableBitmap(280, 280);

                            item.Model.Picture.GetImage().CopyTo(thumbnailStream);
                        }
                        else if (item.RequestedSize == StreamItemViewModel.Size.Medium)
                        {
                            bitmap = new WriteableBitmap(140, 140);

                            item.Model.Picture.GetThumbnail().CopyTo(thumbnailStream);
                        }
                        else
                        {
                            bitmap = new WriteableBitmap(70, 70);

                            item.Model.Picture.GetThumbnail().CopyTo(thumbnailStream);
                        }

                        using (EditingSession session = new EditingSession(thumbnailStream.GetWindowsRuntimeBuffer()))
                        {
                            Windows.Foundation.Rect rect;

                            if (session.Dimensions.Width > session.Dimensions.Height)
                            {
                                rect = new Windows.Foundation.Rect()
                                {
                                    Width = session.Dimensions.Height,
                                    Height = session.Dimensions.Height,
                                    X = session.Dimensions.Width / 2 - session.Dimensions.Height / 2,
                                    Y = 0
                                };
                            }
                            else
                            {
                                rect = new Windows.Foundation.Rect()
                                {
                                    Width = session.Dimensions.Width,
                                    Height = session.Dimensions.Width,
                                    X = 0,
                                    Y = session.Dimensions.Height / 2 - session.Dimensions.Width / 2
                                };
                            }

                            session.AddFilter(FilterFactory.CreateCropFilter(rect));

                            if (item.Model.Filter != null)
                            {
                                foreach (IFilter f in item.Model.Filter.Components)
                                {
                                    session.AddFilter(f);
                                }
                            }

                            await session.RenderToBitmapAsync(bitmap.AsBitmap());
                        }
                    }
                    
                    item.TransitionToImage(bitmap);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Rendering stream item failed:" + ex.Message);

                    item.TransitionToImage(null);
                }
            }

            _processingNow--;

            if (_processingNow == 0)
            {
                Busy = false;
            }
        }

        #endregion
    }
}
