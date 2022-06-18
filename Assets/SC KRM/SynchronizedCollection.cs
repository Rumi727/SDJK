//Source: https://referencesource.microsoft.com/#System.ServiceModel/System/ServiceModel/SynchronizedCollection.cs
using System.Threading;

namespace System.Collections.Generic
{
    [Runtime.InteropServices.ComVisible(false)]
    public class SynchronizedCollection<T> : IList<T>, IList
    {
        List<T> items;
        int sync = 0;

        public SynchronizedCollection() => items = new List<T>();

        public SynchronizedCollection(IEnumerable<T> list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            items = new List<T>(list);
        }

        public SynchronizedCollection(params T[] list)
        {
            if (list == null)
                throw new ArgumentNullException("list");

            items = new List<T>(list);
        }

        public int Count
        {
            get 
            {
                while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                    Thread.Sleep(1);
                
                int count = items.Count;

                Interlocked.Decrement(ref sync);

                return count;
            }
        }

        protected List<T> Items => items;

        public T this[int index]
        {
            get
            {
                while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                    Thread.Sleep(1);

                try
                {
                    return items[index];
                }
                finally
                {
                    Interlocked.Decrement(ref sync);
                }
            }
            set
            {
                while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                    Thread.Sleep(1);

                try
                {
                    if (index < 0 || index >= items.Count)
                        throw new ArgumentOutOfRangeException();

                    SetItem(index, value);
                }
                finally
                {
                    Interlocked.Decrement(ref sync);
                }
            }
        }

        public void Add(T item)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                int index = items.Count;
                InsertItem(index, item);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public void Clear()
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                ClearItems();
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public void CopyTo(T[] array, int index)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                items.CopyTo(array, index);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public bool Contains(T item)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                return items.Contains(item);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                return items.GetEnumerator();
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public int IndexOf(T item)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                return InternalIndexOf(item);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public void Insert(int index, T item)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                if (index < 0 || index > items.Count)
                    throw new ArgumentOutOfRangeException();

                InsertItem(index, item);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        int InternalIndexOf(T item)
        {
            int count = items.Count;

            for (int i = 0; i < count; i++)
            {
                if (Equals(items[i], item))
                    return i;
            }
            return -1;
        }

        public bool Remove(T item)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                int index = InternalIndexOf(item);
                if (index < 0)
                    return false;

                RemoveItem(index);
                return true;
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        public void RemoveAt(int index)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                if (index < 0 || index >= items.Count)
                    throw new ArgumentOutOfRangeException();

                RemoveItem(index);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        protected virtual void ClearItems() => items.Clear();

        protected virtual void InsertItem(int index, T item) => items.Insert(index, item);

        protected virtual void RemoveItem(int index) => items.RemoveAt(index);

        protected virtual void SetItem(int index, T item) => items[index] = item;

        bool ICollection<T>.IsReadOnly => false;

        IEnumerator IEnumerable.GetEnumerator() => ((IList)items).GetEnumerator();

        bool ICollection.IsSynchronized => true;

        object ICollection.SyncRoot => sync;

        void ICollection.CopyTo(Array array, int index)
        {
            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                ((IList)items).CopyTo(array, index);
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        object IList.this[int index]
        {
            get => this[index];
            set
            {
                VerifyValueType(value);
                this[index] = (T)value;
            }
        }

        bool IList.IsReadOnly => false;

        bool IList.IsFixedSize => false;

        int IList.Add(object value)
        {
            VerifyValueType(value);

            while (Interlocked.CompareExchange(ref sync, 1, 0) != 0)
                Thread.Sleep(1);

            try
            {
                Add((T)value);
                return Count - 1;
            }
            finally
            {
                Interlocked.Decrement(ref sync);
            }
        }

        bool IList.Contains(object value)
        {
            VerifyValueType(value);
            return Contains((T)value);
        }

        int IList.IndexOf(object value)
        {
            VerifyValueType(value);
            return IndexOf((T)value);
        }

        void IList.Insert(int index, object value)
        {
            VerifyValueType(value);
            Insert(index, (T)value);
        }

        void IList.Remove(object value)
        {
            VerifyValueType(value);
            Remove((T)value);
        }

        static void VerifyValueType(object value)
        {
            if (value == null)
            {
                if (typeof(T).IsValueType)
                    throw new ArgumentException();
            }
            else if (value is not T)
                throw new ArgumentException();
        }
    }
}