using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace FilterExplorer.Utilities
{
    public class RandomAccessStreamCache<T> where T : IRandomAccessStream
    {
        public uint Version { get; set; }
        public T Stream { get; set; }
        public Task<T> Task { get; set; }

        public RandomAccessStreamCache()
        {
        }

        public RandomAccessStreamCache(RandomAccessStreamCache<T> other)
        {
            if (other.Stream != null)
            {
                Stream = (T)other.Stream.CloneStream();
            }
        }

        public void Invalidate()
        {
            if (Stream != null)
            {
                Stream.Dispose();
                Stream = default(T);
            }

            if (Task != null)
            {
                Task.AsAsyncOperation().Cancel();
                Task = null;
            }
        }

        ~RandomAccessStreamCache()
        {
            Invalidate();
        }
    }
}
