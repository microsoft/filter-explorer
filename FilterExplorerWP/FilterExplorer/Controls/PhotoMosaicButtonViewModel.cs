using System;
using System.ComponentModel;

namespace ImageProcessingApp.ViewModels
{
    public class PhotoMosaicButtonViewModelRetrieveEventArgs : EventArgs
    {

    }

    public class PhotoMosaicButtonViewModel : INotifyPropertyChanged
    {
        private bool _isEnabled = true;
        private string _text;

        public event PropertyChangedEventHandler PropertyChanged;
        public EventHandler<PhotoMosaicButtonViewModelRetrieveEventArgs> RetrieveRequested;

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("IsEnabled"));
                    }
                }
            }
        }

        public string Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (_text != value)
                {
                    _text = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                    }
                }
            }
        }

        public void RaiseRetrieveRequest()
        {
            if (RetrieveRequested != null)
            {
                RetrieveRequested(this, new PhotoMosaicButtonViewModelRetrieveEventArgs());
            }
        }
    }
}
