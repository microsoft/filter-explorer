/*
 * Copyright (c) 2014 Microsoft Mobile. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
using System.Collections.Generic;

namespace FilterExplorer.Utilities
{
    public class ObservableList<T> : IList<T>
    {
        private List<T> _list = new List<T>();

        public delegate void ChangedEventHandler(object sender, EventArgs e);

        public event ChangedEventHandler ItemsChanged;

        public ObservableList()
        {
        }

        public ObservableList(ObservableList<T> other)
        {
            foreach (var item in other._list)
            {
                _list.Add(item);
            }
        }

        public int IndexOf(T item)
        {
            return _list.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            _list.Insert(index, item);

            RaiseItemsChanged();
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);

            RaiseItemsChanged();
        }

        public T this[int index]
        {
            get
            {
                return _list[index];
            }

            set
            {
                _list[index] = value;
            }
        }

        public void Add(T item)
        {
            _list.Add(item);

            RaiseItemsChanged();
        }

        public void Clear()
        {
            _list.Clear();

            RaiseItemsChanged();
        }

        public bool Contains(T item)
        {
            return _list.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _list.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool Remove(T item)
        {
            bool found = _list.Remove(item);

            RaiseItemsChanged();

            return found;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        public void RemoveLast()
        {
            _list.RemoveAt(_list.Count - 1);

            RaiseItemsChanged();
        }

        private void RaiseItemsChanged()
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(this, EventArgs.Empty);
            }
        }
    }
}
