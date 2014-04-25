/*
 * Copyright (c) 2014 Microsoft Mobile. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Utilities;
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class PreviewViewModel : ViewModelBase
    {
        private BitmapImage _preview = null;
        private Size? _resolution = null;

        internal FilteredPhotoModel Model { get; private set; }

        public ObservableList<Filter> Filters
        {
            get
            {
                return Model.Filters;
            }
        }

        public Size? Resolution
        {
            get
            {
                return _resolution;
            }

            set
            {
                if (_resolution != value)
                {
                    _resolution = value;

                    Notify("Resolution");
                }
            }
        }

        public BitmapImage Preview
        {
            get
            {
                return _preview;
            }

            private set
            {
                if (_preview != value)
                {
                    _preview = value;

                    Notify("Preview");
                }
            }
        }

        public PreviewViewModel(FilteredPhotoModel model)
        {
            Model = model;
            Model.FilteredPhotoChanged += Model_FilteredPhotoChanged;

            UpdateResolution();
            UpdatePreview();
        }

        ~PreviewViewModel()
        {
            Model.FilteredPhotoChanged -= Model_FilteredPhotoChanged;
        }

        private void Model_FilteredPhotoChanged(object sender, EventArgs e)
        {
            if (_preview != null)
            {
                UpdatePreview();
            }

            Notify("Filters");
        }

        private async void UpdatePreview()
        {
            Processing = true;

            try
            {
                using (var stream = await Model.GetFilteredPreviewAsync())
                {
                    var maximumSide = (int)Windows.UI.Xaml.Application.Current.Resources["PreviewSide"];
                    var bitmap = new BitmapImage() { DecodePixelWidth = maximumSide };
                    bitmap.SetSource(stream);

                    Preview = bitmap;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("UpdatePreview exception: " + ex.Message + '\n' + ex.StackTrace);
            }

            Processing = false;
        }

        private async void UpdateResolution()
        {
            Resolution = await Model.GetPhotoResolutionAsync();
        }
    }
}
