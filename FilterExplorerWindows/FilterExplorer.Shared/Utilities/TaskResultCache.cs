/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
using System.Threading.Tasks;

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

#if DEBUG
                if (Result != null)
                {
                    System.Diagnostics.Debug.WriteLine("TaskResultCache.Execute warning: Previous result already exists and may leak if disposable");
                }
#endif

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
