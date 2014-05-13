using FilterExplorer.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Application template is documented at http://go.microsoft.com/fwlink/?LinkId=391641

namespace FilterExplorer
{
    /// <summary>
    /// Provides application-specific behavior to supplement the default Application class.
    /// </summary>
    public sealed partial class App : Application
    {
        private TransitionCollection transitions;

        public static IContinuationActivatedEventArgs ContinuationActivatedEventArgs { get; private set; }
        public static EventHandler<IContinuationActivatedEventArgs> ContinuationEventArgsChanged;

        /// <summary>
        /// Initializes the singleton application object.  This is the first line of authored code
        /// executed, and as such is the logical equivalent of main() or WinMain().
        /// </summary>
        public App()
        {
            this.InitializeComponent();
            this.Suspending += this.OnSuspending;
            this.UnhandledException += App_UnhandledException;

            Windows.Phone.UI.Input.HardwareButtons.BackPressed += HardwareButtons_BackPressed;
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used when the application is launched to open a specific file, to display
        /// search results, and so forth.
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
            CreateRootFrame(e.PreviousExecutionState);

            ContinuationActivatedEventArgs = e as IContinuationActivatedEventArgs;
            
            if (ContinuationEventArgsChanged != null)
            {
                ContinuationEventArgsChanged(this, ContinuationActivatedEventArgs);
            }
        }

        private void CreateRootFrame(ApplicationExecutionState state, string launchArguments = null)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            // Do not repeat app initialization when the Window already has content
            if (rootFrame == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                rootFrame = new Frame();
                rootFrame.Background = new Windows.UI.Xaml.Media.SolidColorBrush(Windows.UI.Colors.Black);

                // TODO: change this value to a cache size that is appropriate for your application
                rootFrame.CacheSize = 0;

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

                Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync().AsTask().Wait();
            }

            if (rootFrame.Content == null)
            {
                // Removes the turnstile navigation for startup.
                if (rootFrame.ContentTransitions != null)
                {
                    this.transitions = new TransitionCollection();
                    foreach (var c in rootFrame.ContentTransitions)
                    {
                        this.transitions.Add(c);
                    }
                }

                rootFrame.ContentTransitions = null;
                rootFrame.Navigated += this.RootFrame_FirstNavigated;

                // When the navigation stack isn't restored navigate to the first page,
                // configuring the new page by passing required information as a navigation
                // parameter
                if (!rootFrame.Navigate(typeof(Views.StreamPage), launchArguments))
                {
                    throw new Exception("Failed to create initial page");
                }
            }
        }

        /// <summary>
        /// Restores the content transitions after the app has launched.
        /// </summary>
        /// <param name="sender">The object where the handler is attached.</param>
        /// <param name="e">Details about the navigation event.</param>
        private void RootFrame_FirstNavigated(object sender, NavigationEventArgs e)
        {
            var rootFrame = sender as Frame;
            if (rootFrame != null)
            {
                rootFrame.ContentTransitions = this.transitions ?? new TransitionCollection() { new NavigationThemeTransition() };
                rootFrame.Navigated -= this.RootFrame_FirstNavigated;
            }
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

        private void HardwareButtons_BackPressed(object sender, Windows.Phone.UI.Input.BackPressedEventArgs e)
        {
            Frame rootFrame = Window.Current.Content as Frame;

            if (rootFrame != null)
            {
                if (rootFrame.CanGoBack)
                {
                    rootFrame.GoBack();
                    e.Handled = true;
                }
            }
        }

        private void App_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Unhandled exception: " + e.Message + '\n' + e.Exception.StackTrace);
        }

        protected override void OnWindowCreated(WindowCreatedEventArgs args)
        {
            DataTransferManager.GetForCurrentView().DataRequested += OnDataRequested;
        }

        private async void OnDataRequested(DataTransferManager sender, DataRequestedEventArgs e)
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

        private async Task<StorageFile> SaveTemporaryPhotoAsync(FilteredPhotoModel photo)
        {
            var filename = Application.Current.Resources["PhotoSaveTemporaryFilename"] as string;
            var folder = ApplicationData.Current.TemporaryFolder;
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
    }
}