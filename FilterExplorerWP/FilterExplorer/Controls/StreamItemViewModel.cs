using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;

namespace ImageProcessingApp.Models
{
    /// <summary>
    /// View model representing a single photo item in the application's Camera Roll photo stream.
    /// </summary>
    public class StreamItemViewModel : INotifyPropertyChanged
    {
        #region Members

        private enum Animation
        {
            None,
            FlipX,
            FlipY
        }

        private bool _ready = false;
        private bool _realized = false;
        private bool _dirty = false;
        private StreamItemModel _model = null;
        private WriteableBitmap _imageSource = null;
        private WriteableBitmap _newImageSource = null;

        private Animation _animation = Animation.None;
        private PlaneProjection _projection = new PlaneProjection();
        private Duration _duration = new Duration(TimeSpan.FromSeconds(0.3));
        private Storyboard _storyBoard = new Storyboard();
        private DoubleAnimation _animationX = new DoubleAnimation();
        private DoubleAnimation _animationY = new DoubleAnimation();
        
        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public StreamItemModel Model
        {
            get
            {
                return _model;
            }
        }

        /// <summary>
        /// Photo stream mosaic layout has images of three sizes.
        /// </summary>
        public enum Size
        {
            None,
            Small,
            Medium,
            Large
        };

        /// <summary>
        /// Size of the needed bitmap to represent this photo stream item.
        /// </summary>
        public Size RequestedSize { get; private set; }

        public bool Ready
        {
            get
            {
                return _ready;
            }

            private set
            {
                if (_ready != value)
                {
                    _ready = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Ready"));
                    }
                }
            }
        }

        public WriteableBitmap SmallImage
        {
            get
            {
                return HandleImageRequest(Size.Small);
            }
        }

        public WriteableBitmap MediumImage
        {
            get
            {
                return HandleImageRequest(Size.Medium);
            }
        }

        public WriteableBitmap LargeImage
        {
            get
            {
                return HandleImageRequest(Size.Large);
            }
        }

        public PlaneProjection Projection
        {
            get
            {
                return _projection;
            }

            private set
            {
                if (_projection != value)
                {
                    _projection = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Projection"));
                    }
                }
            }
        }

        public Size SizeHint
        {
            set
            {
                if (RequestedSize == Size.None)
                {
                    RequestedSize = value;

                    if (_realized)
                    {
                        if (!App.PhotoStreamHelper.Contains(this))
                        {
                            App.PhotoStreamHelper.Add(this, true);
                        }
                    }
                }
            }
        }

        public bool Realized
        {
            get
            {
                return _realized;
            }

            set
            {
                if (_realized != value)
                {
                    _realized = value;

                    if (_realized)
                    {
                        if (RequestedSize != Size.None && _imageSource == null)
                        {
                            if (!App.PhotoStreamHelper.Contains(this))
                            {
                                App.PhotoStreamHelper.Add(this, true);
                            }
                        }
                    }
                    else
                    {
                        _imageSource = null; // release handle to bitmap for photo stream virtualization to be effective
                    }

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Realized"));
                    }
                }
            }
        }

        #endregion

        #region Public methods

        public StreamItemViewModel(StreamItemModel model)
        {
            _model = model;
            _model.PropertyChanged += Model_PropertyChanged;

            _storyBoard.Children.Add(_animationX);
            _storyBoard.Children.Add(_animationY);
            _storyBoard.Duration = _duration;

            _animationX.Duration = _duration;
            _animationY.Duration = _duration;

            Storyboard.SetTarget(_animationX, _projection);
            Storyboard.SetTarget(_animationY, _projection);

            Storyboard.SetTargetProperty(_animationX, new PropertyPath("RotationX"));
            Storyboard.SetTargetProperty(_animationY, new PropertyPath("RotationY"));

            RequestedSize = Size.None;
        }

        /// <summary>
        /// Method to be called when a bitmap has been rendered for this photo stream item.
        /// 
        /// Starts animated transition to the new image.
        /// </summary>
        /// <param name="bitmap">New image bitmap</param>
        public void TransitionToImage(WriteableBitmap bitmap)
        {
            if (_animation != Animation.None)
            {
                _newImageSource = bitmap;

                if (_animation == Animation.FlipX)
                {
                    _animationX.From = 0;
                    _animationX.To = 90;

                    _animationY.From = 0;
                    _animationY.To = 0;
                }
                else
                {
                    _animationX.From = 0;
                    _animationX.To = 0;

                    _animationY.From = 0;
                    _animationY.To = -90;
                }

                _storyBoard.Completed += StoryBoard_Completed;

                _storyBoard.Begin();
            }
            else
            {
                ImageSource = bitmap;
            }
        }

        #endregion

        #region Private methods

        private void Model_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Filter" || e.PropertyName == "Picture")
            {
                if (_animation == Animation.None)
                {
                    if (RequestedSize == Size.Small)
                    {
                        _animation = new Random().Next() % 2 == 0 ? Animation.FlipX : Animation.FlipY;
                    }
                    else
                    {
                        _animation = Animation.FlipX;
                    }

                    App.PhotoStreamHelper.Add(this, false);
                }
                else
                {
                    _dirty = true;
                }
            }
        }

        private void StoryBoard_Completed(object sender, EventArgs e)
        {
            try
            {
                if (_newImageSource != null)
                {
                    ImageSource = _newImageSource;

                    _newImageSource = null;

                    if (_animation == Animation.FlipX)
                    {
                        _animationX.From = -90;
                        _animationX.To = 0;

                        _animationY.From = 0;
                        _animationY.To = 0;
                    }
                    else
                    {
                        _animationX.From = 0;
                        _animationX.To = 0;

                        _animationY.From = 90;
                        _animationY.To = 0;
                    }

                    _storyBoard.Begin();
                }
                else
                {
                    _animation = Animation.None;

                    _storyBoard.Completed -= StoryBoard_Completed;

                    if (_dirty)
                    {
                        _dirty = false;
                        _animation = new Random().Next() % 2 == 0 ? Animation.FlipX : Animation.FlipY;

                        App.PhotoStreamHelper.Add(this, false);
                    }
                }
            }
            catch (Exception)
            {
            }
        }

        private WriteableBitmap ImageSource
        {
            set
            {
                if (_imageSource != value)
                {
                    _imageSource = value;

                    if (PropertyChanged != null)
                    {
                        if (RequestedSize == Size.Small)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("SmallImage"));
                        }
                        else if (RequestedSize == Size.Medium)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("MediumImage"));
                        }
                        else if (RequestedSize == Size.Large)
                        {
                            PropertyChanged(this, new PropertyChangedEventArgs("LargeImage"));
                        }
                    }
                }
            }
        }

        private WriteableBitmap CreateTemporaryBitmap()
        {
            WriteableBitmap b = new WriteableBitmap(100, 100);
            b.SetSource(_model.Picture.GetThumbnail());

            return b;
        }

        private WriteableBitmap HandleImageRequest(Size size)
        {
            WriteableBitmap b;

            if (RequestedSize != size)
            {
                // rendering of image of different size is pending,
                // make sure correct size image is going to be rendered
                // and return thumbnail now

                _imageSource = null;

                RequestedSize = size;
                Ready = false;

                if (!App.PhotoStreamHelper.Contains(this))
                {
                    App.PhotoStreamHelper.Add(this, true);
                }

                b = CreateTemporaryBitmap();
            }
            else if (_imageSource == null)
            {
                // rendering of image of correct size may be pending,
                // return thumbnail now and wait for rendering to finish

                Ready = false;

                if (!App.PhotoStreamHelper.Contains(this))
                {
                    App.PhotoStreamHelper.Add(this, true);
                }

                b = CreateTemporaryBitmap();
            }
            else
            {
                // correct size image has been rendered, return it; reference to
                // bitmap will be released when Realized property is set to false

                Ready = true;

                b = _imageSource;
            }

            return b;
        }

        #endregion
    }
}