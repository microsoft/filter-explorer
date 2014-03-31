using Microsoft.Xna.Framework.Media;
using System.ComponentModel;

namespace ImageProcessingApp.Models
{
    public class StreamItemModel : INotifyPropertyChanged
    {
        #region Members

        private Picture _picture;
        private FilterModel _filter;

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public FilterModel Filter
        {
            get
            {
                return _filter;
            }

            private set
            {
                if (_filter != value)
                {
                    _filter = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Filter"));
                    }
                }
            }
        }

        public Picture Picture
        {
            get
            {
                return _picture;
            }

            set
            {
                if (_picture != value)
                {
                    _picture = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Picture"));
                    }
                }
            }
        }

        #endregion

        #region Public methods

        public StreamItemModel(Picture picture, FilterModel filter = null)
        {
            _picture = picture;
            _filter = filter;
        }

        ~StreamItemModel()
        {
            _picture.Dispose();
        }

        public void RandomizeFilter()
        {
            Filter = App.FilterModel.RandomFilter();
        }

        #endregion
    }
}