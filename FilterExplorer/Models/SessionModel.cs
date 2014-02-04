using FilterExplorer.Filters;
using FilterExplorer.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

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

        public void Store()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;
            var sessionJson = new JsonObject();

            if (Folder != null)
            {
                sessionJson.Add("Folder", JsonValue.CreateStringValue(Folder.Path));
            }

            if (Photo != null)
            {
                var photoJson = new JsonObject();
                photoJson.Add("Path", JsonValue.CreateStringValue(Photo.File.Path));
                var filterIdsJson = new JsonArray();
                
                foreach (var filter in Photo.Filters)
                {
                    filterIdsJson.Add(JsonValue.CreateStringValue(filter.Id));
                }

                photoJson.Add("FilterIds", filterIdsJson);

                sessionJson.Add("Photo", photoJson);
            }

            settings.Values["Session"] = sessionJson.Stringify();
        }

        public void Restore()
        {
            var settings = Windows.Storage.ApplicationData.Current.LocalSettings;

            if (settings.Values.ContainsKey("Session"))
            {
                var sessionJson = JsonObject.Parse(settings.Values["Session"] as string).GetObject();

                if (sessionJson.ContainsKey("Folder"))
                {
                    var folderTask = StorageFolder.GetFolderFromPathAsync(sessionJson["Folder"].GetString()).AsTask();
                    folderTask.Wait();
                    Folder = folderTask.Result;
                }
                else
                {
                    Folder = null;
                }

                if (sessionJson.ContainsKey("Photo"))
                {
                    var photoJson = sessionJson["Photo"].GetObject();
                    var filterIdsJson = photoJson["FilterIds"].GetArray();
                    var filters = new ObservableList<Filter>();

                    foreach (var id in filterIdsJson)
                    {
                        filters.Add(FilterFactory.CreateFilter(id.GetString()));
                    }

                    var path = photoJson["Path"].GetString();

                    var fileTask = Windows.Storage.StorageFile.GetFileFromPathAsync(path).AsTask();
                    fileTask.Wait();
                    var file = fileTask.Result;

                    Instance.Photo = new FilteredPhotoModel(file, filters);
                }
                else
                {
                    Photo = null;
                }
            }
        }
    }
}
