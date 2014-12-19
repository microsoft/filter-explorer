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

using FilterExplorer.Models;
using FilterExplorer.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.ApplicationSettings;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=234227

namespace FilterExplorer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    sealed partial class App : Application
    {
        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += OnSuspending;
            this.UnhandledException += App_UnhandledException;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {
#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            CreateRootFrame(e.PreviousExecutionState, e.Arguments);
        }

        protected override void OnActivated(IActivatedEventArgs e)
        {
            base.OnActivated(e);

            CreateRootFrame(e.PreviousExecutionState);
        }

        private void CreateRootFrame(ApplicationExecutionState state, string launchArguments = null)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.CacheSize = 0;

                // Set the default language
                rootFrame.Language = Windows.Globalization.ApplicationLanguages.Languages[0];

                rootFrame.NavigationFailed += OnNavigationFailed;

                if (state == ApplicationExecutionState.Terminated)
                {
                    SessionModel.Instance.Restore();

                    var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

                    if (settings.Values.ContainsKey("NavigationState"))
                    {
                        rootFrame.SetNavigationState(settings.Values["NavigationState"] as string);
                    }
                }

                // Place the frame in the current Window
                Window.Current.Content = rootFrame;
            }

            if (rootFrame.Content == null)
            {
                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                rootFrame.Navigate(typeof(Views.StreamPage), launchArguments);
            }
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        private void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            SessionModel.Instance.Store();

            var rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null)
            {
                var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
                settings.Values["NavigationState"] = rootFrame.GetNavigationState();
            }
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unhandled exception: " + e.Message + '\n' + e.Exception.StackTrace);
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            SettingsPane.GetForCurrentView().CommandsRequested += OnCommandsRequested;
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        private void OnCommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args)
        {
            var label = new Windows.ApplicationModel.Resources.ResourceLoader().GetString("AboutFlyoutCommandLabel");

            args.Request.ApplicationCommands.Add(new SettingsCommand("", label, (handler) => ShowAboutFlyout()));
        }

        public void ShowAboutFlyout()
        {
            var flyout = new AboutFlyout();

            flyout.Show();
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
        {
            if (SessionModel.Instance.Photo != null)
            {
                var deferral = e.Request.GetDeferral();

                var file = await SaveTemporaryPhotoAsync(SessionModel.Instance.Photo);

                try
                {
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                    DataPackage data = e.Request.Data;
                    data.Properties.ApplicationName = loader.GetString("ApplicationName");
                    data.Properties.Description = loader.GetString("PhotoSharingDescription");
                    data.Properties.Thumbnail = RandomAccessStreamReference.CreateFromFile(file);
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
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("DataTransferManager_DataRequested exception: " + ex.Message + '\n' + ex.StackTrace);
                }
                finally
                {
                    deferral.Complete();
                }
            }
        }

        private async Task<StorageFile> SaveTemporaryPhotoAsync(FilteredPhotoModel photo)
        {
            var filename = Application.Current.Resources["PhotoSaveTemporaryFilename"] as string;
            var folder = ApplicationData.Current.TemporaryFolder;
            if (filename != null)
            {
                var task = folder.CreateFileAsync(filename, Windows.Storage.CreationCollisionOption.ReplaceExisting).AsTask();
                task.Wait();
                var file = task.Result;

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

            return null;
        }
    }
}
