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

using System;
using System.ComponentModel;
using System.Reflection;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace FilterExplorer.Controls
{
    public sealed partial class RotatingThumbnailView : UserControl
    {
        private static Random _random = new Random(DateTime.Now.Millisecond + 1);

        private enum Animation
        {
            None,
            FlipVerticalIn,
            FlipVerticalOut,
            FlipHorizontalIn,
            FlipHorizontalOut,
        }

        private INotifyPropertyChanged _viewModel = null;
        private Animation _animation = Animation.None;
        private PlaneProjection _projection = new PlaneProjection();
        private Duration _duration = new Duration(TimeSpan.FromSeconds(0.3));
        private Storyboard _storyBoard = new Storyboard();
        private DoubleAnimation _animationX = new DoubleAnimation();
        private DoubleAnimation _animationY = new DoubleAnimation();

        public string ImageSourcePropertyName { get; set; }

        public RotatingThumbnailView()
        {
            this.InitializeComponent();

            base.DataContextChanged += Grid_DataContextChanged;

            _storyBoard.Children.Add(_animationX);
            _storyBoard.Children.Add(_animationY);
            _storyBoard.Duration = _duration;

            _animationX.Duration = _duration;
            _animationY.Duration = _duration;

            Storyboard.SetTarget(_animationX, _projection);
            Storyboard.SetTarget(_animationY, _projection);

            Storyboard.SetTargetProperty(_animationX, "RotationX");
            Storyboard.SetTargetProperty(_animationY, "RotationY");

            Projection = _projection;

            ThumbnailImage.Source = null;
        }

        private void Animate()
        {
            if (_animation == Animation.None)
            {
                _animation = _random.Next() % 2 == 0 ? Animation.FlipVerticalOut : Animation.FlipHorizontalOut;

                if (_animation == Animation.FlipHorizontalOut)
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
        }

        private void StoryBoard_Completed(object sender, object e)
        {
            if (_animation == Animation.FlipVerticalOut || _animation == Animation.FlipHorizontalOut)
            {
                if (_animation == Animation.FlipHorizontalOut)
                {
                    _animation = Animation.FlipHorizontalIn;

                    _animationX.From = -90;
                    _animationX.To = 0;

                    _animationY.From = 0;
                    _animationY.To = 0;
                }
                else
                {
                    _animation = Animation.FlipVerticalIn;

                    _animationX.From = 0;
                    _animationX.To = 0;

                    _animationY.From = 90;
                    _animationY.To = 0;
                }

                if (_viewModel != null)
                {
                    var property = _viewModel.GetType().GetTypeInfo().GetDeclaredProperty(ImageSourcePropertyName);
                    var source = property.GetValue(_viewModel) as ImageSource;

                    ThumbnailImage.Source = source;

                    _storyBoard.Begin();
                }
                else
                {
                    _animation = Animation.None;

                    _animationX.From = 0;
                    _animationX.To = 0;

                    _animationY.From = 0;
                    _animationY.To = 0;

                    _storyBoard.Completed -= StoryBoard_Completed;

                    ThumbnailImage.Source = null;
                }
            }
            else
            {
                _animation = Animation.None;

                _animationX.From = 0;
                _animationX.To = 0;

                _animationY.From = 0;
                _animationY.To = 0;

                _storyBoard.Completed -= StoryBoard_Completed;
            }
        }

        private void Grid_DataContextChanged(FrameworkElement sender, DataContextChangedEventArgs args)
        {
            if (_viewModel != null)
            {
                _viewModel.PropertyChanged -= ThumbnailViewModel_PropertyChanged;

                ThumbnailImage.Source = null;
            }

            _viewModel = args.NewValue as INotifyPropertyChanged;

            if (_viewModel != null)
            {
                var property = _viewModel.GetType().GetTypeInfo().GetDeclaredProperty(ImageSourcePropertyName);
                var source = property.GetValue(_viewModel) as ImageSource;

                if (source != null)
                {
                    ThumbnailImage.Source = source;
                }

                _viewModel.PropertyChanged += ThumbnailViewModel_PropertyChanged;
            }
        }

        private void ThumbnailViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == ImageSourcePropertyName)
            {
                var property = _viewModel.GetType().GetTypeInfo().GetDeclaredProperty(ImageSourcePropertyName);
                var source = property.GetValue(_viewModel) as ImageSource;

                if (ThumbnailImage.Source == null)
                {
                    ThumbnailImage.Source = source;
                }
                else if (source != null)
                {
                    if (source != ThumbnailImage.Source)
                    {
                        Animate();
                    }
                }
                else
                {
                    ThumbnailImage.Source = null;
                }
            }
        }
    }
}
