using ImageProcessingApp.Models;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Microsoft.Xna.Framework.Media.PhoneExtensions;

namespace ImageProcessingApp.ViewModels
{
    public class StreamPageViewModel
    {
        #region Private member

        private PhotoMosaicViewModel _currentMosaicViewModel;
        private PictureCollection _pictures = null;
        private IEnumerator<Picture> _enumerator = null;
        private int _photoSetSize = Int32.MaxValue; // 3 * PhotoMosaicViewModel.MaxItems; // amount of items in a single set (sets are separated with "more")

        #endregion

        #region Properties

        public ObservableCollection<object> ListBoxItems { get; set; }
        public EventHandler<StreamItemTappedEventArgs> StreamItemTapped;

        #endregion

        public StreamPageViewModel()
        {
            ListBoxItems = new ObservableCollection<object>();
        }

        ~StreamPageViewModel()
        {
            ClearPhotoStream();
        }

        public void RefreshPhotoStream()
        {
            ClearPhotoStream();

            using (MediaLibrary library = new MediaLibrary())
            {
                foreach (PictureAlbum album in library.RootPictureAlbum.Albums)
                {
                    if (album.Name == "Camera Roll")
                    {
                        _pictures = album.Pictures;
                        _enumerator = _pictures.Reverse().GetEnumerator();

                        AddPhotoSet();

                        break;
                    }
                }
            }
        }

        public void ClearPhotoStream()
        {
            App.PhotoStreamHelper.RemoveAll();

            _currentMosaicViewModel = null;

            if (_pictures != null)
            {
                _pictures.Dispose();
                _pictures = null;
            }

            if (_enumerator != null)
            {
                _enumerator.Dispose();
                _enumerator = null;
            }

            try
            {
                ListBoxItems.Clear();
            }
            catch (Exception)
            {
            }

            GC.Collect();
        }

        #region Private methods

        private void PhotoMosaicViewModel_PhotoStreamItemTapped(object sender, StreamItemTappedEventArgs e)
        {
            if (StreamItemTapped != null)
            {
                StreamItemTapped(this, new StreamItemTappedEventArgs() { Item = e.Item });
            }
        }

        private void PhotoMosaicButtonViewModel_RetrieveRequested(object sender, PhotoMosaicButtonViewModelRetrieveEventArgs e)
        {
            var photoMosaicButtonViewModel = sender as PhotoMosaicButtonViewModel;
            if (photoMosaicButtonViewModel != null) photoMosaicButtonViewModel.IsEnabled = false;

            AddPhotoSet();
        }

        private void AddPhotoSet()
        {
            bool more = false;

            for (int i = 0; i < _photoSetSize && (more = _enumerator.MoveNext());)
            {
                if (Helpers.FileHelpers.IsValidPicture(_enumerator.Current.GetPath()))
                {
                    AddItemToViewModel(new StreamItemModel(_enumerator.Current, App.FilterModel.RandomFilter()));

                    i++;
                }
            }

            if (more)
            {
                ListBoxItems.Add(new PhotoMosaicButtonViewModel()
                {
                    RetrieveRequested = PhotoMosaicButtonViewModel_RetrieveRequested,
                    Text = Resources.AppResources.StreamPage_Button_More
                });
            }
        }

        private void AddItemToViewModel(StreamItemModel item)
        {
            if (_currentMosaicViewModel == null || _currentMosaicViewModel.Count == PhotoMosaicViewModel.MaxItems)
            {
                _currentMosaicViewModel = new PhotoMosaicViewModel()
                {
                    StreamItemTapped = PhotoMosaicViewModel_PhotoStreamItemTapped
                };

                ListBoxItems.Add(_currentMosaicViewModel);
            }

            StreamItemViewModel m = new StreamItemViewModel(item);
            //m.Render(StreamItemViewModel.Size.Small); // pre-render

            _currentMosaicViewModel.Add(m);
        }

        #endregion
    }
}