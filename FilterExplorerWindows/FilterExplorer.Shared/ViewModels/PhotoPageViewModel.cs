/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using FilterExplorer.Commands;
using FilterExplorer.Models;
using FilterExplorer.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.ViewModels
{
    public class PhotoPageViewModel : ViewModelBase
    {
        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand SavePhotoCommand { get; private set; }
        public IDelegateCommand SharePhotoCommand { get; private set; }
#if WINDOWS_PHONE_APP
        public IDelegateCommand ShowAboutCommand { get; private set; }
#endif
        public IDelegateCommand AddFilterCommand { get; private set; }
        public IDelegateCommand RemoveFilterCommand { get; private set; }
        public IDelegateCommand RemoveAllFiltersCommand { get; private set; }

        private PreviewViewModel _preview = null;
        private StorageFile _temporaryFile = null;

        public PreviewViewModel Preview
        {
            get
            {
                return _preview;
            }

            private set
            {
                if (_preview != value)
                {
                    if (_preview != null)
                    {
                        _preview.Model.Filters.ItemsChanged -= Preview_Model_Filters_ItemsChanged;
                    }

                    _preview = value;

                    if (_preview != null)
                    {
                        _preview.Model.Filters.ItemsChanged += Preview_Model_Filters_ItemsChanged;
                    }

                    AddFilterCommand.RaiseCanExecuteChanged();
                    RemoveFilterCommand.RaiseCanExecuteChanged();
                    RemoveAllFiltersCommand.RaiseCanExecuteChanged();

                    Notify("Preview");
                }
            }
        }

        public PhotoPageViewModel()
        {
            GoBackCommand = CommandFactory.CreateGoBackCommand();

#if WINDOWS_PHONE_APP
            SavePhotoCommand = new DelegateCommand(
                (parameter) =>
                    {
                        StartSavePhotoFile();
                    },
                () =>
                {
                    return !Processing;
                });
#else
            SavePhotoCommand = new DelegateCommand(
                async (parameter) =>
                    {
                        await SavePhotoAsync(Preview.Model);
                    },
                () =>
                    {
                        return !Processing;
                    });
#endif

            SharePhotoCommand = new DelegateCommand(
                (parameter) =>
                    {
                        SharePhotoAsync(Preview.Model);
                    },
                () =>
                    {
                        return !Processing;
                    });

#if WINDOWS_PHONE_APP
            ShowAboutCommand = new DelegateCommand((parameter) =>
                {
                    var frame = (Frame)Window.Current.Content;
                    frame.Navigate(typeof(AboutPage));
                });
#endif

            AddFilterCommand = new DelegateCommand(
                (parameter) =>
                    {
                        var frame = (Frame)Window.Current.Content;
                        frame.Navigate(typeof(FilterPage));
                    });

            RemoveFilterCommand = new DelegateCommand(
                (parameter) =>
                    {
                        Preview.Model.Filters.RemoveLast();
                    },
                () =>
                    {
                        return Preview != null ? Preview.Model.Filters.Count > 0 : false;
                    });

            RemoveAllFiltersCommand = new DelegateCommand(
                (parameter) =>
                {
                    Preview.Model.Filters.Clear();
                },
                () =>
                {
                    return Preview != null ? Preview.Model.Filters.Count > 0 : false;
                });
        }

        ~PhotoPageViewModel()
        {
            if (Preview != null)
            {
                Preview.Model.Filters.ItemsChanged -= Preview_Model_Filters_ItemsChanged;
            }
        }

        public override Task<bool> InitializeAsync()
        {
            if (!IsInitialized)
            {
                Processing = true;

                if (SessionModel.Instance.Photo != null)
                {
                    Preview = new PreviewViewModel(SessionModel.Instance.Photo);

                    IsInitialized = true;
                }

                Processing = false;
            }

            return Task.FromResult(IsInitialized);
        }

        private void Preview_Model_Filters_ItemsChanged(object sender, EventArgs e)
        {
            RemoveFilterCommand.RaiseCanExecuteChanged();
            RemoveAllFiltersCommand.RaiseCanExecuteChanged();
        }

        private void PhotoShareModel_AvailableChanged(object sender, EventArgs e)
        {
            SharePhotoCommand.RaiseCanExecuteChanged();
        }

#if WINDOWS_PHONE_APP
        private void StartSavePhotoFile()
        {
            var filenameFormat = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("PhotoSaveFilenameFormat");
            var filename = String.Format(filenameFormat, DateTime.Now.ToString("yyyyMMddHHmmss"));

            var picker = new FileSavePicker();
            picker.SuggestedFileName = filename;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add(".jpg", new List<string>() { ".jpg" });
            picker.ContinuationData["Operation"] = "SavePhotoFile";
            picker.PickSaveFileAndContinue();

            App.ContinuationEventArgsChanged += App_ContinuationEventArgsChanged;
        }

        private async void App_ContinuationEventArgsChanged(object sender, IContinuationActivatedEventArgs e)
        {
            App.ContinuationEventArgsChanged -= App_ContinuationEventArgsChanged;

            var args = e as FileSavePickerContinuationEventArgs;

            if (args != null && (args.ContinuationData["Operation"] as string) == "SavePhotoFile")
            {
                if (args.File != null)
                {
                    await SavePhotoAsync(Preview.Model, args.File);

                    System.Diagnostics.Debug.WriteLine("Photo saved to " + args.File.Path);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Photo not saved");
                }
            }
        }
#else
        public static async Task<StorageFile> SavePhotoAsync(FilteredPhotoModel photo)
        {
            var filenameFormat = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("PhotoSaveFilenameFormat");
            var filename = String.Format(filenameFormat, DateTime.Now.ToString("yyyyMMddHHmmss"));

            var picker = new FileSavePicker();
            picker.SuggestedFileName = filename;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeChoices.Add(".jpg", new List<string>() { ".jpg" });

            var file = await picker.PickSaveFileAsync();

            if (file != null)
            {
                file = await SavePhotoAsync(photo, file);
            }

            return file;
        }
#endif

        private static async Task<StorageFile> SavePhotoAsync(FilteredPhotoModel photo, StorageFile file)
        {
            CachedFileManager.DeferUpdates(file);

            using (var fileStream = await file.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite))
            using (var photoStream = await photo.GetFilteredPhotoAsync())
            using (var reader = new DataReader(photoStream))
            using (var writer = new DataWriter(fileStream))
            {
                await reader.LoadAsync((uint)photoStream.Size);
                var buffer = reader.ReadBuffer((uint)photoStream.Size);

                writer.WriteBuffer(buffer);
                await writer.StoreAsync();
                await writer.FlushAsync();
            }

            var status = await CachedFileManager.CompleteUpdatesAsync(file);

            if (status == Windows.Storage.Provider.FileUpdateStatus.Complete)
            {
                return file;
            }
            else
            {
                return null;
            }
        }

        private async void SharePhotoAsync(FilteredPhotoModel photo)
        {
            Processing = true;

            _temporaryFile = await SaveTemporaryPhotoAsync(Preview.Model);

            DataTransferManager.GetForCurrentView().DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            DataTransferManager.GetForCurrentView().DataRequested -= DataTransferManager_DataRequested;

            var deferral = e.Request.GetDeferral();

            try
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                DataPackage data = e.Request.Data;
                data.Properties.ApplicationName = loader.GetString("ApplicationName");
                data.Properties.Description = loader.GetString("PhotoSharingDescription");
                data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(_temporaryFile);
                data.Properties.Title = loader.GetString("PhotoSharingTitle");
                data.SetStorageItems(new List<StorageFile>() { _temporaryFile });
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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("DataTransferManager_DataRequested exception: " + ex.Message + '\n' + ex.StackTrace);
            }
            finally
            {
                deferral.Complete();

                _temporaryFile = null;

                Processing = false;
            }
        }

        private async Task<StorageFile> SaveTemporaryPhotoAsync(FilteredPhotoModel photo)
        {
            var filename = Application.Current.Resources["PhotoSaveTemporaryFilename"] as string;
            var folder = ApplicationData.Current.TemporaryFolder;
            var file = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            return await SavePhotoAsync(photo, file);
        }
    }
}
