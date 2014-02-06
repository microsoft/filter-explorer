using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FilterExplorer.ViewModels
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        private bool _isInitialized = false;
        private bool _processing = false;

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsInitialized
        {
            get
            {
                return _isInitialized;
            }

            set
            {
                if (_isInitialized != value)
                {
                    _isInitialized = value;

                    Notify("IsInitialized");
                }
            }
        }

        public bool Processing
        {
            get
            {
                return _processing;
            }

            set
            {
                if (_processing != value)
                {
                    _processing = value;

                    Notify("Processing");
                }
            }
        }

        protected void Notify(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public virtual Task<bool> InitializeAsync()
        {
            return Task.FromResult(false);
        }
    }
}
