using ImageProcessingApp.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ImageProcessingApp.ViewModels
{
    public class StreamItemTappedEventArgs : EventArgs
    {
        public StreamItemModel Item { get; set; }
    }

    public class PhotoMosaicViewModel : INotifyPropertyChanged
    {
        #region Members

        private bool _realized = false;

        private List<StreamItemViewModel> _items = new List<StreamItemViewModel>();

        #endregion

        #region Properties

        public event PropertyChangedEventHandler PropertyChanged;

        public EventHandler<StreamItemTappedEventArgs> StreamItemTapped;

        public static int MaxItems = 18;

        public int Count
        {
            get
            {
                return _items.Count;
            }
        }

        public bool Realized
        {
            get
            {
                return _realized;
            }

            set
            {
                if (_realized != value)
                {
                    _realized = value;

                    foreach (StreamItemViewModel m in _items)
                    {
                        m.Realized = _realized;
                    }

                    if (PropertyChanged != null)
                    {
                        PropertyChanged(this, new PropertyChangedEventArgs("Realized"));
                    }
                }
            }
        }

        public StreamItemViewModel Image1 { get { return Item(0); } }
        public StreamItemViewModel Image2 { get { return Item(1); } }
        public StreamItemViewModel Image3 { get { return Item(2); } }
        public StreamItemViewModel Image4 { get { return Item(3); } }
        public StreamItemViewModel Image5 { get { return Item(4); } }
        public StreamItemViewModel Image6 { get { return Item(5); } }
        public StreamItemViewModel Image7 { get { return Item(6); } }
        public StreamItemViewModel Image8 { get { return Item(7); } }
        public StreamItemViewModel Image9 { get { return Item(8); } }
        public StreamItemViewModel Image10 { get { return Item(9); } }
        public StreamItemViewModel Image11 { get { return Item(10); } }
        public StreamItemViewModel Image12 { get { return Item(11); } }
        public StreamItemViewModel Image13 { get { return Item(12); } }
        public StreamItemViewModel Image14 { get { return Item(13); } }
        public StreamItemViewModel Image15 { get { return Item(14); } }
        public StreamItemViewModel Image16 { get { return Item(15); } }
        public StreamItemViewModel Image17 { get { return Item(16); } }
        public StreamItemViewModel Image18 { get { return Item(17); } }

        #endregion

        #region Public methods

        public void Add(StreamItemViewModel item)
        {
            System.Diagnostics.Debug.Assert(_items.Count < MaxItems);

            _items.Add(item);

            item.SizeHint = ImageSize(_items.Count - 1);

            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs("Image" + _items.Count));
            }
        }

        public void RandomizeFilter()
        {
            if (Count > 0)
            {
                Random r = new Random(DateTime.Now.Millisecond);

                int i = r.Next(0, Count - 1);

                if (_items[i].Ready)
                {
                    _items[i].Model.RandomizeFilter();
                }
            }
        }

        public void RaiseStreamItemTapped(int i)
        {
            if (StreamItemTapped != null)
            {
                StreamItemTapped(this, new StreamItemTappedEventArgs
                {
                    Item = Item(i - 1).Model
                });
            }
        }

        #endregion

        #region Private methods

        private StreamItemViewModel Item(int index)
        {
            return index < _items.Count ? _items[index] : null;
        }

        private StreamItemViewModel.Size ImageSize(int index)
        {
            StreamItemViewModel.Size size;

            switch (index)
            {
                case 0:
                case 10:
                    {
                        size = StreamItemViewModel.Size.Large;
                    }
                    break;

                case 1:
                case 2:
                case 7:
                case 8:
                case 9:
                case 11:
                case 12:
                case 17:
                    {
                        size = StreamItemViewModel.Size.Medium;
                    }
                    break;

                case 3:
                case 4:
                case 5:
                case 6:
                case 13:
                case 14:
                case 15:
                case 16:
                    {
                        size = StreamItemViewModel.Size.Small;
                    }
                    break;

                default:
                    {
                        size = StreamItemViewModel.Size.None;
                    }
                    break;
            }

            return size;
        }

        #endregion
    }
}
