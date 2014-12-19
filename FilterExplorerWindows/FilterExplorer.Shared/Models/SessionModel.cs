/*
 * Copyright (c) 2014 Microsoft Mobile
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */

using FilterExplorer.Filters;
using FilterExplorer.Utilities;
using System;
using Windows.Data.Json;
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
                var photoJson = new JsonObject {{"Path", JsonValue.CreateStringValue(Photo.File.Path)}};
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
