using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Utilities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            using (var stream = await Model.GetFilteredPreviewAsync())
            {
                var bitmap = new BitmapImage();
                bitmap.SetSource(stream);

                Preview = bitmap;
            }

            Processing = false;
        }

        private async void UpdateResolution()
        {
            Resolution = await Model.GetPhotoResolutionAsync();
        }
    }
}
