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
        public IDelegateCommand ShowAboutCommand { get; private set; }
        public IDelegateCommand AddFilterCommand { get; private set; }
        public IDelegateCommand RemoveFilterCommand { get; private set; }
        public IDelegateCommand RemoveAllFiltersCommand { get; private set; }
        
        private PreviewViewModel _preview = null;

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
                });
#else
            SavePhotoCommand = new DelegateCommand(
                async (parameter) =>
                    {
                        await PhotoLibraryModel.SavePhotoAsync(Preview.Model);
                    });
#endif

            SharePhotoCommand = new DelegateCommand(
                (parameter) =>
                    {
                        PhotoShareModel.SharePhotoAsync(Preview.Model);
                    },
                () =>
                    {
                        return PhotoShareModel.Available;
                    });

            ShowAboutCommand = new DelegateCommand((parameter) =>
                {
                    var frame = (Frame)Window.Current.Content;
                    frame.Navigate(typeof(AboutPage));
                });

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

            PhotoShareModel.AvailableChanged += PhotoShareModel_AvailableChanged;
        }

        ~PhotoPageViewModel()
        {
            if (Preview != null)
            {
                Preview.Model.Filters.ItemsChanged -= Preview_Model_Filters_ItemsChanged;
            }

            PhotoShareModel.AvailableChanged -= PhotoShareModel_AvailableChanged;
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
                    await PhotoLibraryModel.SavePhotoAsync(Preview.Model, args.File);

                    System.Diagnostics.Debug.WriteLine("Photo saved to " + args.File.Path);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Photo not saved");
                }
            }
        }
#endif
    }
}
