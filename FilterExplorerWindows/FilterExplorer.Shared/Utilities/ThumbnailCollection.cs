using FilterExplorer.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace FilterExplorer.Utilities
{
    public class ThumbnailCollection : ObservableCollection<ThumbnailViewModel>
    {
        protected override void ClearItems()
        {
            for (int index = Count - 1; index >= 0; index--)
            {
                ThumbnailViewModel thumbnailViewModel = this[index];
                RemoveAt(index);                
            }

            base.ClearItems();
        }

        new public void Clear()
        {
            ClearItems();
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];            
            base.RemoveItem(index);
            ((IDisposable)item).Dispose();
        }
    }
}
