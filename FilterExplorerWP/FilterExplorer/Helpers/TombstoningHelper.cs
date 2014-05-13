using ImageProcessingApp.Models;
using System.IO;
using System.IO.IsolatedStorage;
using System.Xml.Serialization;
using Windows.Storage.Streams;
using System.Runtime.InteropServices.WindowsRuntime;

namespace ImageProcessingApp.Helpers
{
    static class TombstoningHelper
    {
        private static string _photoModelPath = @"\Tombstoning\v1\PhotoModel";
        private static string _photoModelBufferFilename = @"buffer.data";
        private static string _photoModelPropertiesFilename = @"properties.xml";

        public static void StorePhotoModel(PhotoModel model)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.DirectoryExists(_photoModelPath))
                {
                    storage.CreateDirectory(_photoModelPath);
                }

                // buffer.data

                if (storage.FileExists(_photoModelPath + @"\" + _photoModelBufferFilename))
                {
                    storage.DeleteFile(_photoModelPath + @"\" + _photoModelBufferFilename);
                }

                IBuffer buffer = model.Buffer;

                if (buffer != null)
                {
                    IsolatedStorageFileStream originalFile = storage.CreateFile(_photoModelPath + @"\" + _photoModelBufferFilename);

                    Stream bufferStream = buffer.AsStream();

                    bufferStream.CopyTo(originalFile);
                    bufferStream.Flush();
                    bufferStream.Close();
                    bufferStream.Dispose();

                    originalFile.Flush();
                    originalFile.Close();
                    originalFile.Dispose();
                }

                // properties.xml

                if (storage.FileExists(_photoModelPath + @"\" + _photoModelPropertiesFilename))
                {
                    storage.DeleteFile(_photoModelPath + @"\" + _photoModelPropertiesFilename);
                }

                IsolatedStorageFileStream propertiesFile = storage.CreateFile(_photoModelPath + @"\" + _photoModelPropertiesFilename);
                XmlSerializer serializer = new XmlSerializer(typeof(PhotoModel));
                TextWriter textWriter = new StreamWriter(propertiesFile);

                serializer.Serialize(textWriter, model);

                textWriter.Flush();
                propertiesFile.Flush();

                textWriter.Close();
                propertiesFile.Close();
            }
        }

        public static PhotoModel RestorePhotoModel()
        {
            PhotoModel model = null;

            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                // parameters.xml

                if (storage.FileExists(_photoModelPath + @"\" + _photoModelPropertiesFilename))
                {
                    IsolatedStorageFileStream propertiesFile = storage.OpenFile(_photoModelPath + @"\" + _photoModelPropertiesFilename, FileMode.Open, FileAccess.Read);

                    XmlSerializer serializer = new XmlSerializer(typeof(PhotoModel));

                    model = serializer.Deserialize(propertiesFile) as PhotoModel;

                    propertiesFile.Flush();
                    propertiesFile.Close();
                }

                // buffer.data

                if (model != null && storage.FileExists(_photoModelPath + @"\" + _photoModelBufferFilename))
                {
                    IsolatedStorageFileStream originalFile = storage.OpenFile(_photoModelPath + @"\" + _photoModelBufferFilename, FileMode.Open, FileAccess.Read);

                    MemoryStream stream = new MemoryStream();
                    originalFile.CopyTo(stream);

                    model.Buffer = stream.GetWindowsRuntimeBuffer();

                    originalFile.Flush();
                    originalFile.Close();
                }
            }

            return model;
        }
    }
}
