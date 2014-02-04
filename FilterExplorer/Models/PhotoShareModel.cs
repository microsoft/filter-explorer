using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;

namespace FilterExplorer.Models
{
    public static class PhotoShareModel
    {
        private static FilteredPhotoModel _model = null;
        private static bool _available = true;

        public static event EventHandler AvailableChanged;
        public static event EventHandler SharePhotoFinished;

        public static bool Available
        {
            get
            {
                return _available;
            }

            set
            {
                if (_available != value)
                {
                    _available = value;

                    if (AvailableChanged != null)
                    {
                        AvailableChanged(null, EventArgs.Empty);
                    }
                }
            }
        }

        public static void SharePhotoAsync(FilteredPhotoModel photo)
        {
            if (Available)
            {
                Available = false;

                _model = new FilteredPhotoModel(photo);

                DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_DataRequested;
                DataTransferManager.ShowShareUI();
            }
            else
            {
                throw new Exception();
            }
        }

        private static async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= DataTransferManager_DataRequested;

            var deferral = e.Request.GetDeferral();

            try
            {
                var file = await PhotoLibraryModel.SaveTemporaryPhotoAsync(_model);
                var thumbnail = await _model.GetFilteredThumbnailAsync();
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                DataPackage data = e.Request.Data;
                data.Properties.ApplicationName = loader.GetString("ApplicationName");
                data.Properties.Description = loader.GetString("PhotoSharingDescription");
                data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromStream(thumbnail);
                data.Properties.Title = loader.GetString("PhotoSharingTitle");
                data.SetStorageItems(new List<StorageFile>() { file });
                data.SetText(loader.GetString("PhotoSharingText"));

                try
                {
                    data.Properties.ApplicationListingUri = Windows.ApplicationModel.Store.CurrentApp.LinkUri;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("Getting application store link URI failed: " + ex.Message);
                }
            }
            finally
            {
                _model = null;

                deferral.Complete();

                Available = true;

                if (SharePhotoFinished != null)
                {
                    SharePhotoFinished(null, EventArgs.Empty);
                }
            }
        }
    }
}
