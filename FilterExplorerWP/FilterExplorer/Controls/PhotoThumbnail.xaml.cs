using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace ImageProcessingApp.Controls
{
    public partial class PhotoThumbnail : UserControl, INotifyPropertyChanged
    {
        private WriteableBitmap _bitmap = null;
        private string _text = null;

        public WriteableBitmap Bitmap
        {
            get
            {
                return _bitmap;
            }

            set
            {
                if (_bitmap != value)
                {
                    _bitmap = value;

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Bitmap"));
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

        public event PropertyChangedEventHandler PropertyChanged;

        public PhotoThumbnail()
        {
            InitializeComponent();

            DataContext = this;
        }
    }
}
