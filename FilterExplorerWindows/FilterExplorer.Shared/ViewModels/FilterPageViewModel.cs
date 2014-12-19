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

using FilterExplorer.Commands;
using FilterExplorer.Filters;
using FilterExplorer.Models;
using FilterExplorer.Utilities;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace FilterExplorer.ViewModels
{
    public class FilterPageViewModel : ViewModelBase, IDisposable
    {
        public IDelegateCommand GoBackCommand { get; private set; }
        public IDelegateCommand ApplyFilterCommand { get; private set; }

        public ThumbnailCollection Thumbnails { get; private set; }

        public FilterPageViewModel()
        {
            Thumbnails = new ThumbnailCollection();

            GoBackCommand = CommandFactory.CreateGoBackCommand();

            ApplyFilterCommand = new DelegateCommand((parameter) =>
            {
                var viewModel = (ThumbnailViewModel)parameter;

                SessionModel.Instance.Photo.Filters.Add(viewModel.Filter);

                var frame = (Frame)Window.Current.Content;
                frame.GoBack();
            });
        }

        public override Task<bool> InitializeAsync()
        {
            if (!IsInitialized && SessionModel.Instance.Photo != null)
            {
                Processing = true;

                var filters = FilterFactory.CreateAllFilters();

                foreach (var filter in filters)
                {
                    Thumbnails.Add(new ThumbnailViewModel(SessionModel.Instance.Photo, filter));
                }

                Processing = false;

                IsInitialized = true;
            }

            return Task.FromResult(IsInitialized);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
                return;
           
            Thumbnails.Clear();                    
        }

        public void Dispose()
        {
            Dispose(true);
        }
    }
}
