/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Streams;
using Windows.UI.Xaml;

namespace FilterExplorer.Models
{
    public class PhotoLibraryModel
    {
#if !WINDOWS_PHONE_APP
        public static async Task<StorageFolder> PickPhotoFolderAsync()
        {
            var picker = new FolderPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;

            return await picker.PickSingleFolderAsync();
        }
#endif
        
#if !WINDOWS_PHONE_APP
        public static async Task<StorageFile> PickPhotoFileAsync()
        {
            var picker = new FileOpenPicker();
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add("*");
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.ViewMode = PickerViewMode.Thumbnail;

            return await picker.PickSingleFileAsync();
        }
#endif

#if !WINDOWS_PHONE_APP
        public static async Task<StorageFile> CapturePhotoFileAsync()
        {
            var captureUi = new Windows.Media.Capture.CameraCaptureUI();
            captureUi.PhotoSettings.Format = Windows.Media.Capture.CameraCaptureUIPhotoFormat.Jpeg;

            var file = await captureUi.CaptureFileAsync(Windows.Media.Capture.CameraCaptureUIMode.Photo);

            if (file != null)
            {
                var filename = Application.Current.Resources["PhotoCaptureTemporaryFilename"] as string;
                var folder = ApplicationData.Current.TemporaryFolder;
                var temporaryFile = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);

                CachedFileManager.DeferUpdates(temporaryFile);
                await file.CopyAndReplaceAsync(temporaryFile);
                await CachedFileManager.CompleteUpdatesAsync(temporaryFile);

                file = temporaryFile;
            }

            return file;
        }
#endif

        public static async Task<List<FilteredPhotoModel>> GetPhotosFromFolderAsync(StorageFolder folder, uint amount)
        {
            var list = new List<FilteredPhotoModel>();
            var files = await folder.GetFilesAsync();

            foreach (var file in files)
            {
                if (list.Count == amount)
                {
                    break;
                }

                var properties = await file.GetBasicPropertiesAsync();

                if (properties.Size > 0 && file.ContentType == "image/jpeg")
                {
                    list.Add(new FilteredPhotoModel(file));
                }
            }

            return list;
        }

#if !WINDOWS_PHONE_APP
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

        internal static async Task<StorageFile> SaveTemporaryPhotoAsync(FilteredPhotoModel photo)
        {
            var filename = Application.Current.Resources["PhotoSaveTemporaryFilename"] as string;
            var folder = ApplicationData.Current.TemporaryFolder;
            var file = await folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting);

            return await SavePhotoAsync(photo, file);
        }

        public static async Task<StorageFile> SavePhotoAsync(FilteredPhotoModel photo, StorageFile file)
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
    }
}
