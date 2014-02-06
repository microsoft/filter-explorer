using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;

namespace FilterExplorer.Utilities
{
    public class TaskResultCache<T> where T : class
    {
        private Task<T> _task = null;

        public uint Version { get; set; }
        public T Result { get; set; }

        public bool Pending
        {
            get
            {
                return _task != null;
            }
        }

        public async Task WaitAsync()
        {
            await _task;
        }

        public async Task Execute(Task<T> task)
        {
            _task = task;

            await _task;

            if (_task.IsCompleted)
            {
                Result = _task.Result;
            }

            _task = null;
        }

        public async Task Execute(Task<T> task, uint version)
        {
            _task = task;

            await _task;

            if (_task.IsCompleted)
            {
                Version = version;
                Result = _task.Result;
            }

            _task = null;
        }

        public void Cancel()
        {
            if (Pending)
            {
                _task.AsAsyncOperation().Cancel();
            }
        }

        public void Invalidate()
        {
            if (Result != null)
            {
                if (Result is IDisposable)
                {
                    var disposable = (IDisposable)Result;
                    disposable.Dispose();
                }

                Result = null;
            }
        }

        ~TaskResultCache()
        {
            Cancel();
            Invalidate();
        }
    }
}
