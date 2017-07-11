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

    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
    /// <typeparam name="TItem">The type of an item. </typeparam>
    public class ObservableCollectionView<TItem> : ObservableCollectionViewBase<TItem>, IObservableCollectionView
    {
        private Func<TItem, bool> _filter;
        private Func<TItem, object> _order;

        private int _offset=0;
        private int _limit=0;
        private bool _ascending = true;

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        public ObservableCollectionView()
            : this(new ObservableCollection<TItem>())
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        public ObservableCollectionView(IList<TItem> items)
            : this(items, null, null, true, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter)
            : this(items, filter, null, true, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        /// <param name="orderBy">The order key of the view. </param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter, Func<TItem, object> orderBy)
            : this(items, filter, orderBy, true, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        /// <param name="orderBy">The order key of the view. </param>
        /// <param name="ascending">The value indicating whether to sort ascending. </param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter, Func<TItem, object> orderBy, bool ascending)
            : this(items, filter, orderBy, ascending, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionView{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="filter">The filter of the view. </param>
        /// <param name="orderBy">The order key of the view. </param>
        /// <param name="ascending">The value indicating whether to sort ascending. </param>
        /// <param name="trackItemChanges">The value indicating whether to track items which implement <see cref="INotifyPropertyChanged"/></param>
        public ObservableCollectionView(IList<TItem> items, Func<TItem, bool> filter, Func<TItem, object> orderBy, bool ascending, bool trackItemChanges)
            : base(items, trackItemChanges)
        {
            Order = orderBy;
            Filter = filter;
            Ascending = ascending;
        }

        /// <summary>Gets or sets the filter. </summary>
        public Func<TItem, bool> Filter
        {
            get { return _filter; }
            set
            {
                if (_filter != value)
                {
                    _filter = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets the filter (a Func{TItem, bool} object). </summary>
        object IObservableCollectionView.Filter
        {
            get { return Filter; }
            set { Filter = (Func<TItem, bool>)value; }
        }

        /// <summary>Gets or sets the sorting/order function. </summary>
        public Func<TItem, object> Order
        {
            get { return _order; }
            set
            {
                if (_order != value)
                {
                    _order = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets the order. </summary>
        object IObservableCollectionView.Order
        {
            get { return Order; }
            set { Order = (Func<TItem, object>)value; }
        }

        /// <summary>Gets or sets the maximum number of items in the view. </summary>
        public int Limit
        {
            get { return _limit; }
            set
            {
                if (_limit != value)
                {
                    _limit = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets the offset from where the results a selected. </summary>
        public int Offset
        {
            get { return _offset; }
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets or sets a value indicating whether the sorting should be ascending; otherwise descending. </summary>
        public bool Ascending
        {
            get { return _ascending; }
            set
            {
                if (_ascending != value)
                {
                    _ascending = value;
                    Refresh();
                }
            }
        }

        /// <summary>Gets the list of items with the current order and filter.</summary>
        /// <returns>The items. </returns>
        protected override IList<TItem> GetItems()
        {
            List<TItem> list;

            if (Filter != null && Order != null && Ascending)
                list = Items.Where(Filter).OrderBy(Order).ToList();
            else if (Filter != null && Order != null && !Ascending)
                list = Items.Where(Filter).OrderByDescending(Order).ToList();
            else if (Filter == null && Order != null && Ascending)
                list = Items.OrderBy(Order).ToList();
            else if (Filter == null && Order != null && !Ascending)
                list = Items.OrderByDescending(Order).ToList();
            else if (Filter != null && Order == null)
                list = Items.Where(Filter).ToList();
            else if (Filter == null && Order == null)
                list = Items.ToList();
            else
                throw new Exception();

            if (Limit > 0 || Offset > 0)
                list = list.Skip(Offset).Take(Limit).ToList();

            return list;
        }
    }

    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
    public interface IObservableCollectionView : IList, INotifyCollectionChanged, INotifyPropertyChanged
    {

        /// <summary>Gets or sets the maximum number of items in the view. </summary>
        int Limit { get; set; }

        /// <summary>Gets or sets the offset from where the results a selected. </summary>
        int Offset { get; set; }

        /// <summary>Gets or sets a value indicating whether to sort ascending or descending. </summary>
        bool Ascending { get; set; }

        /// <summary>Gets or sets the filter (a Func{TItem, bool} object). </summary>
        object Filter { get; set; }

        /// <summary>Gets or sets the order (a Func{TItem, object} object). </summary>
        object Order { get; set; }

        /// <summary>Refreshes the view. </summary>
        void Refresh();
    }

    /// <summary>Provides a view for an <see cref="ObservableCollection{T}"/> with automatic sorting and filtering. </summary>
    /// <typeparam name="TItem">The type of an item. </typeparam>
    public abstract class ObservableCollectionViewBase<TItem> : IList<TItem>, IDisposable, IList, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private NotifyCollectionChangedEventHandler _itemsChangedHandler;
        private MtObservableCollection<TItem> _internalCollection = new MtObservableCollection<TItem>();

        private readonly object _syncRoot = new object();

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionViewBase{TItem}"/> class. </summary>
        protected ObservableCollectionViewBase()
            : this(new ObservableCollection<TItem>(), false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionViewBase{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        protected ObservableCollectionViewBase(IList<TItem> items)
            : this(items, false)
        {
        }

        /// <summary>Initializes a new instance of the <see cref="ObservableCollectionViewBase{TItem}"/> class. </summary>
        /// <param name="items">The source item list. </param>
        /// <param name="trackItemChanges">The value indicating whether to track items which implement <see cref="INotifyPropertyChanged"/></param>
        protected ObservableCollectionViewBase(IList<TItem> items, bool trackItemChanges)
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
        public void Update()
        {
            Refresh();
        }

        /// <summary>Refreshes the view. </summary>
        public void Refresh()
        {
            
            lock (SyncRoot)
            {
                var list = GetItems();
               // if (!_internalCollection.IsCopyOf(list))
                    _internalCollection.Initialize(list);
            }
        }

        /// <summary>Gets the list of items with the current order and filter.</summary>
        /// <returns>The items. </returns>
        protected abstract IList<TItem> GetItems();

        private void OnOriginalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            lock (SyncRoot)
            {
                Refresh();
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
        private List<T> _oldCollection = null;
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

        /// <summary>Значение означающее сохранять старую коллекцию при изменении коллекции
        /// Enabling this feature may have a performance impact as for each collection changed event a copy of the collection gets created. </summary>
        public bool ProvideOldCollection
        {
            get; set;
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

        /// <summary>Collection changed event with safe/always correct added items and removed items list. </summary>
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

        /// <summary>Raises the System.Collections.ObjectModel.ObservableCollection{T}.CollectionChanged event with the provided arguments. </summary>
        /// <param name="e">Arguments of the event being raised. </param>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            var copy = _extendedCollectionChanged;
            if (copy != null)
            {
                var oldCollection = ProvideOldCollection ? _oldCollection.ToList() : null;

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

                copy(this, new MtNotifyCollectionChangedEventArgs<T>(addedItems, removedItems, oldCollection));
            }
        }
    }

    public class MtNotifyCollectionChangedEventArgs<T> : PropertyChangedEventArgs
    {

        public MtNotifyCollectionChangedEventArgs(IList<T> addedItems, IList<T> removedItems, IList<T> oldCollection)
            : base(null)
        {
            AddedItems = addedItems;
            RemovedItems = removedItems;
            OldCollection = oldCollection;
        }

        /// <summary>
        /// Gets or sets the list of added items. 
        /// </summary>
        public IList<T> AddedItems { get; private set; }

        /// <summary>
        /// Gets or sets the list of removed items. 
        /// </summary>
        public IList<T> RemovedItems { get; private set; }

        /// <summary>
        /// Gets the previous collection (only provided when enabled in the <see cref="MtObservableCollection{T}"/> object). 
        /// </summary>
        public IList<T> OldCollection { get; private set; }

    }

}
