using FilterExplorer.Commands;
using FilterExplorer.Models;
using FilterExplorer.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.ViewModels
{
    public class PhotoPageViewModel : INotifyPropertyChanged
    {
        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand OpenPhotoCommand { get; private set; }
        public IDelegateCommand SavePhotoCommand { get; private set; }
        public IDelegateCommand SharePhotoCommand { get; private set; }
        public IDelegateCommand AddFilterCommand { get; private set; }
        public IDelegateCommand RemoveFilterCommand { get; private set; }
        public IDelegateCommand RemoveAllFiltersCommand { get; private set; }

        private PhotoPreviewViewModel _preview = null;

        public event PropertyChangedEventHandler PropertyChanged;

        public PhotoPreviewViewModel Preview
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

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Preview"));
                    }
                }
            }
        }

        public PhotoPageViewModel()
        {
            GoBackCommand = CommandFactory.CreateGoBackCommand();

            OpenPhotoCommand = new DelegateCommand(
                async (parameter) =>
                {
                    var file = await PhotoLibraryModel.PickPhotoFileAsync();

                    if (file != null)
                    {
                        var photo = new FilteredPhotoModel(file);

                        SessionModel.Instance.Photo = photo;

                        Preview = new PhotoPreviewViewModel(SessionModel.Instance.Photo);
                    }
                });

            SavePhotoCommand = new DelegateCommand(
                async (parameter) =>
                    {
                        await PhotoLibraryModel.SavePhotoAsync(Preview.Model);
                    });

            SharePhotoCommand = new DelegateCommand(
                (parameter) =>
                    {
                        PhotoShareModel.SharePhotoAsync(Preview.Model);
                    },
                () =>
                    {
                        return PhotoShareModel.Available;
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

            Preview = new PhotoPreviewViewModel(SessionModel.Instance.Photo);

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

        private void Preview_Model_Filters_ItemsChanged(object sender, EventArgs e)
        {
            RemoveFilterCommand.RaiseCanExecuteChanged();
            RemoveAllFiltersCommand.RaiseCanExecuteChanged();
        }

        private void PhotoShareModel_AvailableChanged(object sender, EventArgs e)
        {
            SharePhotoCommand.RaiseCanExecuteChanged();
        }
    }
}
