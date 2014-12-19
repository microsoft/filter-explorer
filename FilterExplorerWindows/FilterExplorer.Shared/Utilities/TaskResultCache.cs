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
                var result = Result as IDisposable;
                if (result != null)
                {
                    result.Dispose();
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
