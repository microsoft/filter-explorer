using ImageProcessingApp.Controls;
using ImageProcessingApp.Models;
using ImageProcessingApp.Resources;
using ImageProcessingApp.ViewModels;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using Microsoft.Xna.Framework.Media.PhoneExtensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;

namespace ImageProcessingApp
{
    public partial class StreamPage : PhoneApplicationPage
    {
        #region Members

        private StreamPageViewModel _viewModel = new StreamPageViewModel();

        private PhotoChooserTask _photoChooserTask = new PhotoChooserTask();
        private CameraCaptureTask _cameraCaptureTask = new CameraCaptureTask();

        private ApplicationBarIconButton _libraryButton = null;
        private ApplicationBarIconButton _cameraButton = null;
        private ApplicationBarIconButton _refreshButton = null;

        private ApplicationBarMenuItem _aboutItem = null;

        private DispatcherTimer _timer = new DispatcherTimer() { Interval = new TimeSpan(0, 0, 0, 0, 1000) };

        private List<PhotoMosaic> _realizedPhotoMosaics = new List<PhotoMosaic>();

        private bool _busy = false;

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

                    _refreshButton.IsEnabled = !_busy;

                    ProgressBar.IsIndeterminate = _busy;
                    ProgressBar.Visibility = _busy ? Visibility.Visible : Visibility.Collapsed;
                }
            }
        }

        #endregion

        public StreamPage()
        {
            _viewModel.StreamItemTapped += StreamViewModel_StreamItemTapped;

            DataContext = _viewModel;

            InitializeComponent();

            _photoChooserTask.Completed += Task_Completed;
            _cameraCaptureTask.Completed += Task_Completed;

            _libraryButton = new ApplicationBarIconButton
            {
                Text = AppResources.StreamPage_Button_Library,
                IsEnabled = true,
                IconUri = new Uri("/Assets/Icons/folder.png", UriKind.Relative)
            };
            _libraryButton.Click += LibraryButton_Click;

            ApplicationBar.Buttons.Add(_libraryButton);

            _cameraButton = new ApplicationBarIconButton
            {
                Text = AppResources.StreamPage_Button_Camera,
                IsEnabled = true,
                IconUri = new Uri("/Assets/Icons/camera.png", UriKind.Relative)
            };
            _cameraButton.Click += CameraButton_Click;

            ApplicationBar.Buttons.Add(_cameraButton);

            _refreshButton = new ApplicationBarIconButton
            {
                Text = AppResources.StreamPage_Button_Refresh,
                IsEnabled = true,
                IconUri = new Uri("/Assets/Icons/refresh.png", UriKind.Relative)
            };
            _refreshButton.Click += RefreshButton_Click;

            ApplicationBar.Buttons.Add(_refreshButton);

            _aboutItem = new ApplicationBarMenuItem {Text = AppResources.App_Menu_About, IsEnabled = true};
            _aboutItem.Click += AboutItem_Click;

            ApplicationBar.MenuItems.Add(_aboutItem);

            Loaded += StreamPage_Loaded;

            _timer.Tick += DispatcherTimer_Tick;

            ListBox.ItemRealized += ListBox_ItemRealized;
            ListBox.ItemUnrealized += ListBox_ItemUnrealized;

            App.PhotoStreamHelper.PropertyChanged += PhotoStreamHelper_PropertyChanged;
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            foreach (PhotoMosaic v in _realizedPhotoMosaics)
            {
                PhotoMosaicViewModel m = v.DataContext as PhotoMosaicViewModel;

                if (m != null)
                {
                    try
                    {
                        GeneralTransform transform = v.TransformToVisual(Application.Current.RootVisual);
                        Point offset = transform.Transform(new Point(0, 0));

                        if (offset.Y > -ActualHeight && offset.Y < ActualHeight)
                        {
                            m.RandomizeFilter();
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
        }

        private void ListBox_ItemRealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoMosaic v = GetVisualChild<PhotoMosaic>(e.Container);

            if (v != null && !_realizedPhotoMosaics.Contains(v))
            {
                _realizedPhotoMosaics.Add(v);

                PhotoMosaicViewModel m = v.DataContext as PhotoMosaicViewModel;

                if (m != null)
                {
                    m.Realized = true;
                }
            }
        }

        private void ListBox_ItemUnrealized(object sender, ItemRealizationEventArgs e)
        {
            PhotoMosaic v = GetVisualChild<PhotoMosaic>(e.Container);

            if (v != null && _realizedPhotoMosaics.Contains(v))
            {
                _realizedPhotoMosaics.Remove(v);

                PhotoMosaicViewModel m = v.DataContext as PhotoMosaicViewModel;

                if (m != null)
                {
                    m.Realized = false;
                }
            }
        }

        #region Protected methods

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            App.PhotoStreamHelper.Enabled = true;

            _timer.Start();
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);

            _timer.Stop();

            App.PhotoStreamHelper.Enabled = false;

            _realizedPhotoMosaics.Clear();
        }

        #endregion

        #region Private methods

        private void StreamViewModel_StreamItemTapped(object sender, StreamItemTappedEventArgs e)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                e.Item.Picture.GetImage().CopyTo(stream);

                App.PhotoModel = new PhotoModel
                {
                    Buffer = stream.GetWindowsRuntimeBuffer(),
                    Captured = false,
                    Dirty = true,
                    Path = e.Item.Picture.GetPath()
                };

                if (e.Item.Filter != null)
                {
                    App.PhotoModel.ApplyFilter(e.Item.Filter);
                }
            }

            NavigationService.Navigate(new Uri("/Pages/PhotoPage.xaml", UriKind.Relative));
        }

        private void StreamPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_viewModel.ListBoxItems.Count == 0)
            {
                _viewModel.RefreshPhotoStream();
            }
        }

        private void PhotoStreamHelper_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Busy")
            {
                Busy = App.PhotoStreamHelper.Busy;
            }
        }

        private void LibraryButton_Click(object sender, EventArgs e)
        {
            _photoChooserTask.Show();
        }

        private void CameraButton_Click(object sender, EventArgs e)
        {
            _cameraCaptureTask.Show();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            ScrollViewer v = GetVisualChild<ScrollViewer>(ListBox);

            if (v != null)
            {
                v.ScrollToVerticalOffset(0);
            }

            _viewModel.RefreshPhotoStream();
        }

        private void AboutItem_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Pages/AboutPage.xaml", UriKind.Relative));
        }

        private void Task_Completed(object sender, PhotoResult e)
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

                    using (MemoryStream stream = new MemoryStream())
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

                    Dispatcher.BeginInvoke(() => NavigationService.Navigate(new Uri("/Pages/PhotoPage.xaml", UriKind.Relative)));
                }
                else
                {
                    MessageBox.Show(AppResources.App_MessageBox_UnsupportedImage_Message,
                        AppResources.App_MessageBox_UnsupportedImage_Caption, MessageBoxButton.OK);
                }
            }
        }

        private T GetVisualChild<T>(UIElement parent) where T : UIElement
        {
            T child = null;

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < numVisuals; i++)
            {
                UIElement element = (UIElement)VisualTreeHelper.GetChild(parent, i);

                child = element as T;

                if (child == null)
                {
                    child = GetVisualChild<T>(element);
                }

                if (child != null)
                {
                    break;
                }
            }

            return child;
        }

        private List<T> GetVisualChildren<T>(UIElement parent) where T : UIElement
        {
            List<T> children = new List<T>();

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < numVisuals; i++)
            {
                UIElement element = (UIElement)VisualTreeHelper.GetChild(parent, i);

                T child = element as T;

                if (child != null)
                {
                    children.Add(child);
                }
                else
                {
                    foreach (T s in GetVisualChildren<T>(element))
                    {
                        children.Add(s);
                    }
                }
            }

            return children;
        }

        #endregion
    }
}