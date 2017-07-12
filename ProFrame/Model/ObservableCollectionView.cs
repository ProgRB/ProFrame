using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ProFrame.Model
{

    /// <summary>Представление для данных типа <see cref="ObservableCollection{T}"/> с возможностью сортировки и фильтрациии </summary>
    /// <typeparam name="TItem">Tип элементов </typeparam>
    public abstract class ObservableCollectionViewBase<TItem> : IList<TItem>, IDisposable, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private MtObservableCollection<TItem> _internalCollection = new MtObservableCollection<TItem>();

        private readonly object _syncRoot = new object();

        /// <summary>Конструктор нового экзепляра класса <see cref="ObservableCollectionViewBase{TItem}"/> </summary>
        protected ObservableCollectionViewBase() : this(new ObservableCollection<TItem>())
        {
        }

        /// <summary>Конструктор экземпляра класса <see cref="ObservableCollectionViewBase{TItem}"/> </summary>
        /// <param name="items">The source item list. </param>
        protected ObservableCollectionViewBase(IList<TItem> items)
        {
            Items = items;
            _internalCollection.CollectionChanged += OnInternalCollectionChanged;
            _internalCollection.PropertyChanged += OnInternalPropertyChanged;
            Refresh();
        }

        /// <summary>Gets the original items source. </summary>
        public IList<TItem> Items { get; private set; }


        /// <summary>Adds a multiple elements to the underlying collection. </summary>
        /// <param name="items">The items to add. </param>
        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void AddRange(IEnumerable<TItem> items)
        {

            if (Items is MtObservableCollection<TItem>)
                ((MtObservableCollection<TItem>)Items).AddRange(items);
            else
            {
                foreach (var i in items)
                    Add(i);
            }

        }

        /// <summary>Releases all used resources and deregisters all events on the items and the underlying collection. </summary>
        public void Dispose()
        {
            _internalCollection = null;
            Items = null;
        }

        /// <summary>Refreshes the view. </summary>
        public virtual void Refresh()
        {
            lock (SyncRoot)
            {
                _internalCollection.Initialize(Items);
            }
        }

        #region Interfaces

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnInternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            CollectionChanged?.Invoke(this, e);
        }

        private void OnInternalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }

        public int Count
        {
            get
            {
                lock (SyncRoot)
                    return _internalCollection.Count;
            }
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            lock (SyncRoot)
                return _internalCollection.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            lock (SyncRoot)
                return _internalCollection.GetEnumerator();
        }

        public int IndexOf(TItem item)
        {
            lock (SyncRoot)
                return _internalCollection.IndexOf(item);
        }

        public TItem this[int index]
        {
            get
            {
                lock (SyncRoot)
                    return _internalCollection[index];
            }
            set { throw new NotSupportedException("Используйте ObservableCollectionViewBase.Items[] вместо индексатора списка"); }
        }

        object IList.this[int index]
        {
            get
            {
                lock (SyncRoot)
                    return _internalCollection[index];
            }
            set { throw new NotSupportedException("Используйте ObservableCollectionViewBase.Items[] вместо индексатора списка"); }
        }

        public bool Contains(TItem item)
        {
            lock (SyncRoot)
                return _internalCollection.Contains(item);
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Contains(object value)
        {
            lock (SyncRoot)
                return value is TItem && _internalCollection.Contains((TItem)value);
        }

        public int IndexOf(object value)
        {
            if (!(value is TItem))
                return -1;

            lock (SyncRoot)
                return _internalCollection.IndexOf((TItem)value);
        }

        public bool IsFixedSize
        {
            get { return false; }
        }

        public bool IsSynchronized
        {
            get { return true; }
        }

        public object SyncRoot
        {
            get { return _syncRoot; }
        }

        public void CopyTo(Array array, int index)
        {
            CopyTo((TItem[])array, index);
        }

        public void CopyTo(TItem[] array, int arrayIndex)
        {
            lock (SyncRoot)
                _internalCollection.CopyTo(array, arrayIndex);
        }



        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        int IList.Add(object value)
        {
            return ((IList)Items).Add(value);
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void Add(TItem item)
        {
            Items.Add(item);
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void Clear()
        {
            Items.Clear();
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public bool Remove(TItem item)
        {
            return Items.Remove(item);
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void Remove(object value)
        {
            ((IList)Items).Remove(value);
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void Insert(int index, TItem item)
        {
            throw new NotSupportedException("Используйте ObservableCollectionViewBase.Insert() вместо этого метода");
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void Insert(int index, object value)
        {
            throw new NotSupportedException("Используйте ObservableCollectionViewBase.Insert() вместо этого метода");
        }

        [Obsolete("Используйте методы поля Items вместо этого метода. 11.07.2017")]
        public void RemoveAt(int index)
        {
            throw new NotSupportedException("Используйте ObservableCollectionViewBase.Insert() вместо этого метода");
        }

        #endregion
    }


    /// <summary>Provides a safe collection changed event which always provides the added 
    /// and removed items, some more events and more range methods. </summary>
    /// <typeparam name="T"></typeparam>
    public class MtObservableCollection<T> : ObservableCollection<T>
    {
        private event EventHandler<MtNotifyCollectionChangedEventArgs<T>> _extendedCollectionChanged;

        /// <summary>Initializes a new instance of the <see cref="MtObservableCollection{T}"/> class.</summary>
        public MtObservableCollection()
        {
        }

        /// <summary>Initializes a new instance of the <see cref="MtObservableCollection{T}"/> class.</summary>
        /// <param name="collection">The collection.</param>
        public MtObservableCollection(IEnumerable<T> collection) : base(collection)
        {
        }


        /// <summary>Occurs when a property value changes. 
        /// This is the same event as on the <see cref="ObservableCollection{T}"/> except that it is public. </summary>
        public new event PropertyChangedEventHandler PropertyChanged
        {
            add { base.PropertyChanged += value; }
            remove { base.PropertyChanged -= value; }
        }

        /// <summary>Adds multiple items to the collection. </summary>
        /// <param name="collection">The items to add. </param>
        /// <exception cref="ArgumentNullException">The value of 'collection' cannot be null. </exception>
        public void AddRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection)
                Items.Add(item);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>Removes multiple items from the collection. </summary>
        /// <param name="collection">The items to remove. </param>
        /// <exception cref="ArgumentNullException">The value of 'collection' cannot be null. </exception>
        public void RemoveRange(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            foreach (var item in collection.ToList())
                Items.Remove(item);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        /// <summary>Resets the whole collection with a given list. </summary>
        /// <param name="collection">The collection. </param>
        /// <exception cref="ArgumentNullException">The value of 'collection' cannot be null. </exception>
        public void Initialize(IEnumerable<T> collection)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");

            Items.Clear();
            foreach (var i in collection)
                Items.Add(i);

            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private List<T> _oldCollection = null;
        public event EventHandler<MtNotifyCollectionChangedEventArgs<T>> ExtendedCollectionChanged
        {
            add
            {
                lock (this)
                {
                    if (_extendedCollectionChanged == null)
                        _oldCollection = new List<T>(this);
                    _extendedCollectionChanged += value;
                }
            }
            remove
            {
                lock (this)
                {
                    _extendedCollectionChanged -= value;
                    if (_extendedCollectionChanged == null)
                        _oldCollection = null;
                }
            }
        }

        /// <summary>Вызывает события System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged с изменными элементами </summary>
        /// <param name="e">параметры события </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            if (_extendedCollectionChanged != null)
            {
                var addedItems = new List<T>();
                foreach (var item in this.Where(x => !_oldCollection.Contains(x))) // new items
                {
                    addedItems.Add(item);
                    _oldCollection.Add(item);
                }

                var removedItems = new List<T>();
                foreach (var item in _oldCollection.Where(x => !Contains(x)).ToArray()) // deleted items
                {
                    removedItems.Add(item);
                    _oldCollection.Remove(item);
                }

                _extendedCollectionChanged(this, new MtNotifyCollectionChangedEventArgs<T>(addedItems, removedItems));
            }
        }
    }

    public class MtNotifyCollectionChangedEventArgs<T> : PropertyChangedEventArgs
    {

        public MtNotifyCollectionChangedEventArgs(IList<T> addedItems, IList<T> removedItems)
            : base(null)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
        }

        /// <summary>
        /// Gets or sets the list of added items. 
        /// </summary>
        public IList<T> AddedItems { get; private set; }

        /// <summary>
        /// Gets or sets the list of removed items. 
        /// </summary>
        public IList<T> RemovedItems { get; private set; }

    }

}