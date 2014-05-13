using ImageProcessingApp.Models;
using ImageProcessingApp.Resources;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Windows.Storage.Streams;

namespace ImageProcessingApp
{
    public partial class PhotoPage : PhoneApplicationPage
    {
        #region Members

        private PhotoChooserTask _photoChooserTask = new PhotoChooserTask();
        private CameraCaptureTask _cameraCaptureTask = new CameraCaptureTask();
        private ShareMediaTask _shareMediaTask = new ShareMediaTask();

        private bool _busy = false;

        private ApplicationBarIconButton _addButton = null;
        private ApplicationBarIconButton _undoButton = null;
        private ApplicationBarIconButton _saveButton = null;
        private ApplicationBarIconButton _shareButton = null;

        private ApplicationBarMenuItem _libraryItem = null;
        private ApplicationBarMenuItem _cameraItem = null;
        private ApplicationBarMenuItem _aboutItem = null;

        private const double _maxWidth = 2048;
        private const double _maxHeight = 2048;

        private double _width = 0;
        private double _height = 0;

        private double _scale = 1.0;

        private bool _highQuality = false;

        private bool _pinching = false;

        private Point _relativeCenter;

        private bool _loaded = false;

        #endregion

        #region Properties

        private bool Busy
        {
            get
            {
                return _busy;
            }

            set
            {
                if (_busy != value)
                {
                    _busy = value;

                    _libraryItem.IsEnabled = !_busy;
                    _cameraItem.IsEnabled = !_busy;
                    _addButton.IsEnabled = !_busy && App.PhotoModel != null;
                    _undoButton.IsEnabled = !_busy && App.PhotoModel != null && App.PhotoModel.CanUndoFilter;
                    _saveButton.IsEnabled = !_busy && App.PhotoModel != null && App.PhotoModel.Dirty;
                    _shareButton.IsEnabled = !_busy && App.PhotoModel != null;

                    ProgressBar.Visibility = _busy ? Visibility.Visible : Visibility.Collapsed;
                    ProgressBar.IsIndeterminate = _busy;
                }
            }
        }

        private bool HighQuality
        {
            get
            {
                return _highQuality;
            }

            set
            {
                if (_highQuality != value)
                {
                    _highQuality = value;

                    if (_highQuality)
                    {
                        var task = RenderAsync();
                    }
                }
            }
        }

        #endregion

        public PhotoPage()
        {
            InitializeComponent();

            _photoChooserTask.Completed += Task_Completed;
            _cameraCaptureTask.Completed += Task_Completed;

            _addButton = new ApplicationBarIconButton
            {
                Text = AppResources.PhotoPage_Button_AddFilter,
                IsEnabled = false,
                IconUri = new Uri("/Assets/Icons/add.png", UriKind.Relative)
            };
            _addButton.Click += AddButton_Click;

            ApplicationBar.Buttons.Add(_addButton);

            _undoButton = new ApplicationBarIconButton
            {
                Text = AppResources.PhotoPage_Button_UndoFilter,
                IsEnabled = false,
                IconUri = new Uri("/Assets/Icons/undo.png", UriKind.Relative)
            };
            _undoButton.Click += UndoButton_Click;

            ApplicationBar.Buttons.Add(_undoButton);

            _saveButton = new ApplicationBarIconButton
            {
                Text = AppResources.PhotoPage_Button_Save,
                IsEnabled = true,
                IconUri = new Uri("/Assets/Icons/save.png", UriKind.Relative)
            };
            _saveButton.Click += SaveButton_Click;

            ApplicationBar.Buttons.Add(_saveButton);

            _shareButton = new ApplicationBarIconButton
            {
                Text = AppResources.PhotoPage_Button_Share,
                IsEnabled = true,
                IconUri = new Uri("/Assets/Icons/share.png", UriKind.Relative)
            };
            _shareButton.Click += ShareButton_Click;

            ApplicationBar.Buttons.Add(_shareButton);

            _libraryItem = new ApplicationBarMenuItem {Text = AppResources.PhotoPage_Menu_Library, IsEnabled = true};
            _libraryItem.Click += LibraryItem_Click;

            ApplicationBar.MenuItems.Add(_libraryItem);

            _cameraItem = new ApplicationBarMenuItem {Text = AppResources.PhotoPage_Menu_Camera, IsEnabled = true};
            _cameraItem.Click += CameraItem_Click;

            ApplicationBar.MenuItems.Add(_cameraItem);

            _aboutItem = new ApplicationBarMenuItem {Text = AppResources.App_Menu_About, IsEnabled = true};
            _aboutItem.Click += AboutItem_Click;

            ApplicationBar.MenuItems.Add(_aboutItem);

            Loaded += PhotoPage_Loaded;
        }

        private async void PhotoPage_Loaded(object sender, RoutedEventArgs e)
        {
            _loaded = true;

            await SetupAsync();
            await RenderAsync();
        }

        #region Protected methods

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (App.PhotoModel != null)
            {
                _saveButton.IsEnabled = App.PhotoModel.Dirty;

                if (!Busy)
                {
                    await SetupAsync();
                }
            }
            else if (e.Uri.ToString().Contains("ViewfinderLaunch"))
            {
                if (e.NavigationMode == NavigationMode.Back)
                {
                    Application.Current.Terminate();
                }
                else
                {
                    // uncomment the following lines to test tombstoning (without tasks)

                    //if (App.PhotoModel != null)
                    //{
                    //    App.PhotoModel.Dispose();
                    //}

                    //using (MemoryStream stream = new MemoryStream())
                    //{
                    //    Application.GetResourceStream(new Uri("SplashScreenImage.jpg", UriKind.Relative)).Stream.CopyTo(stream);

                    //    App.PhotoModel = new PhotoModel() { Buffer = stream.GetWindowsRuntimeBuffer() };
                    //    App.PhotoModel.Captured = true;
                    //    App.PhotoModel.Dirty = App.PhotoModel.Captured;
                    //}

                    //await SetupAsync(true, true);

                    //return;

                    _cameraCaptureTask.Show();
                }
            }
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            if (e.NavigationMode == NavigationMode.Back && e.IsCancelable)
            {
                if (Busy)
                {
                    e.Cancel = true;
                }
                else if (App.PhotoModel != null && App.PhotoModel.Dirty)
                {
                    e.Cancel = true;

                    Dispatcher.BeginInvoke(() =>
                    {
                        MessageBoxResult result = MessageBox.Show(AppResources.PhotoPage_MessageBox_Discard_Message,
                            AppResources.PhotoPage_MessageBox_Discard_Caption, MessageBoxButton.OKCancel);

                        if (result == MessageBoxResult.OK)
                        {
                            App.PhotoModel.Dirty = false;

                            NavigationService.GoBack();
                        }
                    });
                }
                else
                {
                    if (App.PhotoModel != null) 
                        App.PhotoModel.Dispose();
                    App.PhotoModel = null;

                    GC.Collect();
                }
            }
        }

        protected override void OnOrientationChanged(OrientationChangedEventArgs e)
        {
            base.OnOrientationChanged(e);

            if (App.PhotoModel != null)
            {
                ConfigureViewport();
                SetupTitlePanel();
            }
        }

        #endregion

        #region Private methods

        private void LibraryItem_Click(object sender, EventArgs e)
        {
            _photoChooserTask.Show();
        }

        private void CameraItem_Click(object sender, EventArgs e)
        {
            _cameraCaptureTask.Show();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/FilterPage.xaml", UriKind.Relative));
        }

        private async void UndoButton_Click(object sender, EventArgs e)
        {
            App.PhotoModel.UndoFilter();
            App.PhotoModel.Dirty = !App.PhotoModel.CanUndoFilter && App.PhotoModel.Captured || App.PhotoModel.CanUndoFilter;

            await SetupAsync();
            await RenderAsync();
        }

        private async void SaveButton_Click(object sender, EventArgs e)
        {
            await SaveAsync();
        }

        private async void ShareButton_Click(object sender, EventArgs e)
        {
            await ShareAsync();
        }

        private async Task ShareAsync()
        {
            if (App.PhotoModel.Dirty)
            {
                await SaveAsync();
            }

            if (App.PhotoModel.Path != null && App.PhotoModel.Path.Length > 0)
            {
                _shareMediaTask.FilePath = App.PhotoModel.Path;
                _shareMediaTask.Show();
            }
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
        }

        private void LayoutRoot_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ApplicationBar.Mode = ApplicationBar.Mode == ApplicationBarMode.Default ?
                ApplicationBarMode.Minimized : ApplicationBarMode.Default;

            TitlePanel.Visibility = ApplicationBar.Mode == ApplicationBarMode.Default ?
                Visibility.Visible : Visibility.Collapsed;
        }

        private async void Task_Completed(object sender, PhotoResult e)
        {
            if (e.TaskResult == TaskResult.OK)
            {
                if (Helpers.FileHelpers.IsValidPicture(e.OriginalFileName))
                {
                    if (App.PhotoModel != null)
                    {
                        App.PhotoModel.Dispose();
                        App.PhotoModel = null;

                        GC.Collect();
                    }

                    using (var stream = new MemoryStream())
                    {
                        e.ChosenPhoto.CopyTo(stream);

                        App.PhotoModel = new PhotoModel
                        {
                            Buffer = stream.GetWindowsRuntimeBuffer(),
                            Captured = (sender == _cameraCaptureTask)
                        };
                        App.PhotoModel.Dirty = App.PhotoModel.Captured;
                        App.PhotoModel.Path = e.OriginalFileName;
                    }

                    if (_loaded)
                    {
                        await SetupAsync();
                        await RenderAsync();
                    }
                }
                else
                {
                    MessageBox.Show(AppResources.App_MessageBox_UnsupportedImage_Message,
                        AppResources.App_MessageBox_UnsupportedImage_Caption, MessageBoxButton.OK);
                }
            }
        }

        private void Viewport_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (Image.Width < Viewport.ActualWidth)
            {
                ConfigureViewport();
            }
        }

        private void UpdateInfoDisplay(Windows.Foundation.Size dimensions)
        {
            ResolutionTextBlock.Text = dimensions.Width + " x " + dimensions.Height;

            if (App.PhotoModel.AppliedFilters.Count > 0)
            {
                string filterNames = "";

                foreach (FilterModel f in App.PhotoModel.AppliedFilters)
                {
                    if (filterNames != "")
                    {
                        filterNames += " \u21E2 ";
                    }

                    filterNames += f.Name;
                }

                FiltersTextBlock.Text = filterNames;
            }
            else
            {
                FiltersTextBlock.Text = AppResources.PhotoPage_TextBlock_NoFilters;
            }
        }

        private void SetupTitlePanel()
        {
            if (Orientation.HasFlag(PageOrientation.LandscapeLeft))
            {
                TitlePanel.HorizontalAlignment = HorizontalAlignment.Left;

                TitlePanel.RenderTransform = new CompositeTransform()
                {
                    Rotation = -90,
                    TranslateY = 480
                };
            }
            else if (Orientation.HasFlag(PageOrientation.LandscapeRight))
            {
                TitlePanel.HorizontalAlignment = HorizontalAlignment.Right;

                TitlePanel.RenderTransform = new CompositeTransform()
                {
                    Rotation = 90,
                    TranslateX = 480
                };
            }
            else
            {
                TitlePanel.RenderTransform = null;
            }
        }

        private async Task SetupAsync()
        {
            try
            {
                Windows.Foundation.Size dimensions = await App.PhotoModel.GetImageSizeAsync();

                double scale;

                if (dimensions.Width > dimensions.Height)
                {
                    scale = _maxWidth / dimensions.Width;
                }
                else
                {
                    scale = _maxHeight / dimensions.Height;
                }

                _width = dimensions.Width * scale;
                _height = dimensions.Height * scale;

                HighQuality = false;

                ConfigureViewport();
                UpdateInfoDisplay(dimensions);
            }
            catch (Exception)
            {
            }
        }

        private async Task RenderAsync()
        {
            if (!Busy)
            {
                Busy = true;

                bool hq;

                do
                {
                    hq = _highQuality;

                    double w = hq ? _width : _width * _scale;
                    double h = hq ? _height : _height * _scale;

                    WriteableBitmap writeableBitmap = new WriteableBitmap((int)w, (int)h);

                    await App.PhotoModel.RenderBitmapAsync(writeableBitmap);

                    Image.Source = writeableBitmap;
                }
                while (hq != _highQuality);

                Busy = false;
            }
        }

        private async Task SaveAsync()
        {
            if (!Busy)
            {
                Busy = true;

                GC.Collect();

                try
                {
                    IBuffer buffer = await App.PhotoModel.RenderFullBufferAsync();

                    using (MediaLibrary library = new MediaLibrary())
                    {
                        using (Picture picture = library.SavePicture(DateTime.UtcNow.Ticks.ToString(), buffer.AsStream()))
                        {
                            App.PhotoModel.Path = picture.GetPath();
                        }
                    }

                    App.PhotoModel.Dirty = false;
                }
                catch (Exception)
                {
                    MessageBox.Show(AppResources.App_MessageBox_SaveFailed_Message,
                        AppResources.App_MessageBox_SaveFailed_Caption, MessageBoxButton.OK);
                }

                Busy = false;
            }
        }

        private void ConfigureViewport()
        {
            if (_width < _height)
            {
                _scale = Viewport.ActualHeight / _height;
            }
            else
            {
                _scale = Viewport.ActualWidth / _width;
            }

            Image.Width = _width * _scale;
            Image.Height = _height * _scale;

            Viewport.Bounds = new Rect(0, 0, Image.Width, Image.Height);
            Viewport.SetViewportOrigin(new Point(
                Image.Width / 2 - Viewport.Viewport.Width / 2,
                Image.Height / 2 - Viewport.Viewport.Height / 2));
        }

        private void Viewport_ManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            if (_pinching)
            {
                e.Handled = true;

                CompletePinching();
            }
        }

        private void Viewport_ManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            if (e.PinchManipulation != null)
            {
                e.Handled = true;

                if (!_pinching)
                {
                    _pinching = true;

                    _relativeCenter = new Point(
                        e.PinchManipulation.Original.Center.X / Image.Width,
                        e.PinchManipulation.Original.Center.Y / Image.Height);
                }

                //System.Diagnostics.Debug.WriteLine("X={0} Y={1}", e.PinchManipulation.Original.Center.X, e.PinchManipulation.Original.Center.Y);

                double w, h;

                if (_width < _height)
                {
                    h = _height * _scale * e.PinchManipulation.CumulativeScale;
                    h = Math.Max(Viewport.ActualHeight, h);
                    h = Math.Min(h, _height);

                    w = h * _width / _height;
                }
                else
                {
                    w = _width * _scale * e.PinchManipulation.CumulativeScale;
                    w = Math.Max(Viewport.ActualWidth, w);
                    w = Math.Min(w, _width);

                    h = w * _height / _width;
                }

                Image.Width = w;
                Image.Height = h;

                Viewport.Bounds = new Rect(0, 0, w, h);

                GeneralTransform transform = Image.TransformToVisual(Viewport);
                Point p = transform.Transform(e.PinchManipulation.Original.Center);

                double x = _relativeCenter.X * w - p.X;
                double y = _relativeCenter.Y * h - p.Y;

                if (w < _width && h < _height)
                {
                    //System.Diagnostics.Debug.WriteLine("Viewport.ActualWidth={0} .ActualHeight={1} Origin.X={2} .Y={3} Image.Width={4} .Height={5}",
                    //    Viewport.ActualWidth, Viewport.ActualHeight, x, y, Image.Width, Image.Height);

                    Viewport.SetViewportOrigin(new Point(x, y));
                }
            }
            else if (_pinching)
            {
                e.Handled = true;

                CompletePinching();
            }
        }

        private void Viewport_ManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            if (_pinching)
            {
                e.Handled = true;

                CompletePinching();
            }
        }

        private void CompletePinching()
        {
            _pinching = false;

            double sw = Image.Width / _width;
            double sh = Image.Height / _height;

            _scale = Math.Min(sw, sh);

            WriteableBitmap bitmap = Image.Source as WriteableBitmap;

            if (Image.Width > _width / 2)
            {
                HighQuality = true;
            }
        }

        #endregion
    }
}