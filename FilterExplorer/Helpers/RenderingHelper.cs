using Nokia.Graphics.Imaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;

namespace FilterExplorer.Helpers
{
    public class RenderingHelper
    {
        public static async Task<IRandomAccessStream> GetFilteredStreamAsync(IRandomAccessStream stream, List<IFilter> filters)
        {
            IRandomAccessStream filteredStream = null;

            if (filters.Count > 0)
            {
                filteredStream = new InMemoryRandomAccessStream();

                stream.Seek(0);

                using (var source = new RandomAccessStreamImageSource(stream))
                using (var effect = new FilterEffect(source) { Filters = filters })
                using (var renderer = new JpegRenderer(effect))
                {
                    var buffer = await renderer.RenderAsync();

                    await filteredStream.WriteAsync(buffer);
                }
            }
            else
            {
                filteredStream = stream.CloneStream();
            }

            return filteredStream;
        }

        public static async Task<WriteableBitmap> GetFilteredBitmapAsync(IRandomAccessStream stream, List<IFilter> filters)
        {
            WriteableBitmap bitmap = null;

            stream.Seek(0);

            using (var source = new RandomAccessStreamImageSource(stream))
            using (var effect = new FilterEffect(source) { Filters = filters })
            {
                var info = await effect.GetInfoAsync();

                bitmap = new WriteableBitmap((int)info.ImageSize.Width, (int)info.ImageSize.Height);

                using (var renderer = new WriteableBitmapRenderer(effect, bitmap))
                {
                    await renderer.RenderAsync();
                }
            }

            return bitmap;
        }
    }
}
