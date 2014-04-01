/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Filters;
using FilterExplorer.Models;
using System;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class ThumbnailViewModel : ViewModelBase
    {
        private string _title = null;
        private Filter _filter = null;
        private BitmapImage _thumbnail = null;
        private bool _highlight = false;

        internal FilteredPhotoModel Model { get; private set; }

        internal Filter Filter
        {
            get
            {
                return _filter;
            }

            set
            {
                if (_filter != value)
                {
                    _filter = value;

                    Title = _filter != null ? _filter.Name : string.Empty;

                    if (Model.Filters.Count > 0)
                    {
                        Model.Filters.RemoveLast();
                    }

                    Model.Filters.Add(_filter);

                    if (_thumbnail != null && !Processing)
                    {
                        UpdateThumbnailAsync();
                    }
                }
            }
        }

        public string Title
        {
            get
            {
                return _title;
            }

            private set
            {
                if (_title != value)
                {
                    _title = value;

                    Notify("Title");
                }
            }
        }

        public BitmapImage Thumbnail
        {
            get
            {
                if (_thumbnail == null && !Processing)
                {
                    UpdateThumbnailAsync();
                }

                return _thumbnail;
            }

            private set
            {
                if (_thumbnail != value)
                {
                    if (_thumbnail != null)
                    {
                        _thumbnail.ImageOpened -= BitmapImage_ImageOpened;
                        _thumbnail.ImageFailed -= BitmapImage_ImageFailed;
                    }

                    _thumbnail = value;

                    if (_thumbnail != null)
                    {
                        _thumbnail.ImageOpened += BitmapImage_ImageOpened;
                        _thumbnail.ImageFailed += BitmapImage_ImageFailed;
                    }
                }
            }
        }

        public bool Highlight
        {
            get
            {
                return _highlight;
            }

            private set
            {
                if (_highlight != value)
                {
                    _highlight = value;

                    Notify("Highlight");
                }
            }
        }

        public ThumbnailViewModel(FilteredPhotoModel model, Filter filter, bool highlight = false)
        {
            Model = new FilteredPhotoModel(model);
            Highlight = highlight;

            _filter = filter;
            Title = _filter != null ? _filter.Name : string.Empty;
            Model.Filters.Add(_filter);
        }

        private async void UpdateThumbnailAsync()
        {
            Processing = true;

            int maximumSide = (int)Windows.UI.Xaml.Application.Current.Resources["ThumbnailSide"];

            try
            {
                using (var stream = await Model.GetFilteredThumbnailAsync())
                {
                    var bitmap = new BitmapImage() { DecodePixelWidth = maximumSide };
                    bitmap.SetSource(stream);

                    Thumbnail = bitmap;
                }
            }
            catch (Exception ex)
            {
                Processing = false;

                System.Diagnostics.Debug.WriteLine("UpdateThumbnailAsync exception: " + ex.Message + '\n' + ex.StackTrace);
            }
        }

        private void BitmapImage_ImageFailed(object sender, Windows.UI.Xaml.ExceptionRoutedEventArgs e)
        {
            Processing = false;

            Notify("Thumbnail");
        }

        private void BitmapImage_ImageOpened(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            Processing = false;

            Notify("Thumbnail");
        }
    }
}
