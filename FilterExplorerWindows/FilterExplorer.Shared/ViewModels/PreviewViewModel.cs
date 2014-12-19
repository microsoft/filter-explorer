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

using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Utilities;
using System;
using Windows.Foundation;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class PreviewViewModel : ViewModelBase, IDisposable
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

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposeing)
        {
            if (!disposeing)
                return;

            Model.FilteredPhotoChanged -= Model_FilteredPhotoChanged;
        }
    }
}
