using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace FilterExplorer.Models
{
    public class SessionModel
    {
        private static SessionModel _singleton = null;

        public static SessionModel Instance
        {
            get
            {
                if (_singleton == null)
                {
                    _singleton = new SessionModel();
                }

                return _singleton;
            }
        }

        public FilteredPhotoModel Photo { get; set; }
        public StorageFolder Folder { get; set; }

        public async Task StoreAsync()
        {
        }

        public async Task RestoreAsync()
        {
        }
    }
}
