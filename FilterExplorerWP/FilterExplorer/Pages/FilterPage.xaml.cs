using ImageProcessingApp.Controls;
using ImageProcessingApp.Models;
using Microsoft.Phone.Controls;
using Lumia.Imaging;
using Lumia.InteropServices.WindowsRuntime;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageProcessingApp
{
    public partial class FilterPage : PhoneApplicationPage
    {
        #region Members

        private bool _busy = false;

        #endregion

        #region Properties

        private bool Busy
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

                    ProgressBar.Visibility = _busy ? Visibility.Visible : Visibility.Collapsed;
                    ProgressBar.IsIndeterminate = _busy;
                }
            }
        }

        #endregion

        public FilterPage()
        {
            InitializeComponent();

            ApplyOrientation(Orientation);

            Loaded += FilterPage_Loaded;
        }

        #region Protected methods

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            ApplyOrientation(e.Orientation);
        }

        #endregion

        #region Private methods

        private async void FilterPage_Loaded(object sender, RoutedEventArgs e)
        {
            await RenderAsync();
        }

        /// <summary>
        /// Asynchronously renders filter image thumbnails.
        /// </summary>
        private async Task RenderAsync()
        {
            if (!Busy)
            {
                Busy = true;

                int side = 136;

                try
                {
                    using (Bitmap bitmap = await App.PhotoModel.RenderThumbnailBitmapAsync(side))
                    {
                        await RenderThumbnailsAsync(bitmap, side, App.FilterModel.ArtisticFilters, StandardFiltersWrapPanel);
                        await RenderThumbnailsAsync(bitmap, side, App.FilterModel.EnhancementFilters, EnhancementFiltersWrapPanel);
                    }
                }
                catch (Exception)
                {
                    NavigationService.GoBack();
                }

                Busy = false;
            }
        }

        /// <summary>
        /// For the given bitmap renders filtered thumbnails for each filter in given list and populates
        /// the given wrap panel with the them.
        /// 
        /// For quick rendering, renders 10 thumbnails synchronously and then releases the calling thread.
        /// </summary>
        /// <param name="bitmap">Source bitmap to be filtered</param>
        /// <param name="side">Side length of square thumbnails to be generated</param>
        /// <param name="list">List of filters to be used, one per each thumbnail to be generated</param>
        /// <param name="panel">Wrap panel to be populated with the generated thumbnails</param>
        private async Task RenderThumbnailsAsync(Bitmap bitmap, int side, List<FilterModel> list, WrapPanel panel)
        {
            using (var source = new BitmapImageSource(bitmap))
            using (var effect = new FilterEffect(source))
            {
                foreach (FilterModel filter in list)
                {
                    effect.Filters = filter.Components;

                    WriteableBitmap writeableBitmap = new WriteableBitmap(side, side);

                    using (var renderer = new WriteableBitmapRenderer(effect, writeableBitmap))
                    {
                        await renderer.RenderAsync();

                        writeableBitmap.Invalidate();

                        var photoThumbnail = new PhotoThumbnail()
                        {
                            Bitmap = writeableBitmap,
                            Text = filter.Name,
                            Width = side,
                            Margin = new Thickness(6)
                        };

                        FilterModel tempFilter = filter;

                        photoThumbnail.Tap += (object sender, System.Windows.Input.GestureEventArgs e) =>
                        {
                            App.PhotoModel.ApplyFilter(tempFilter);
                            App.PhotoModel.Dirty = true;

                            NavigationService.GoBack();
                        };

                        panel.Children.Add(photoThumbnail);
                    }
                }
            }
        }

        /// <summary>
        /// Apply orientation specific layout.
        /// </summary>
        /// <param name="orientation">Page orientation</param>
        private void ApplyOrientation(PageOrientation orientation)
        {
            if (orientation.HasFlag(PageOrientation.Portrait))
            {
                TitleImage.Visibility = Visibility.Visible;
                Pivot.Margin = new Thickness(0);
            }
            else
            {
                TitleImage.Visibility = Visibility.Collapsed;
                Pivot.Margin = new Thickness(0, -16, 0, 0);
            }
        }

        #endregion
    }
}