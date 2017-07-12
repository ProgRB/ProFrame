using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public class UniDbTable<T>: ObservableCollection<T>, IList<T>, IDisposable, IList, INotifyCollectionChanged, INotifyPropertyChanged where T: UniDbRow, new()
    {
        /// <summary>
        /// Конструктор коллекции с именем таблицы. Имя схемы указывается через точку (если требуется)
        /// </summary>
        /// <param name="tableName">Имя таблицы (и схемы если другая)</param>
        public UniDbTable(string tableName)
        {
            if (tableName.Contains("."))
            {
                SchemaName = tableName.Substring(1, tableName.IndexOf('.'));
                TableName = tableName.Substring(tableName.IndexOf('.') + 1);
            }
            else
            {
                TableName = tableName;
            }
           
        }

        public UniDbTable(DataTable table)
        {
            TableName = table.TableName;
            Table = table;
        }

        string _tableName, _schemaName = SchemaTableManager.DefaultSchemaName;
        /// <summary>
        /// Имя таблицы для данных
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
            private set
            {
                _tableName = value;
            }
        }

        /// <summary>
        /// Схема таблицы
        /// </summary>
        public string SchemaName
        {
            get
            {
                return _schemaName;
            }
            set
            {
                _schemaName = value;
            }
        }



        /// <summary>
        /// Реальное имя таблицы которую надо обновлять и откуда получать данные
        /// </summary>
        private string DbTableName
        {
            get
            {
                if (!string.IsNullOrEmpty(TableName))
                    return TableName;
                if (Table != null)
                    return Table.TableName;
                return string.Empty;
            }

        }

        DataTable _table;
        /// <summary>
        /// Таблица данных
        /// </summary>
        public DataTable Table
        {
            get
            {
                return _table;
            }
            set
            {
                _table = value;
            }
        }

        UniSchemaTable _schemaOfTable;
        /// <summary>
        /// Схема-структура представления таблицы
        /// </summary>
        public UniSchemaTable SchemaOfTable
        {
            get
            {
                if (_schemaOfTable==null)
                    _schemaOfTable = SchemaTableManager.GetTable(DbTableName);
                return _schemaOfTable;
            }
        }

        UniDbAdapter _dataAdapter;
        /// <summary>
        /// Адаптер данных для таблицы
        /// </summary>
        public UniDbAdapter DataAdapter
        {
            get
            {
                return _dataAdapter;
            }
            set
            {
                _dataAdapter = value;
            }
        }

        /// <summary>
        /// Инициализирует адаптер сохранения данных
        /// </summary>
        public virtual void InitializeAdapter()
        {
            _dataAdapter = new UniDbAdapter();
            _dataAdapter.AcceptChangesDuringUpdate = false;

            if (!string.IsNullOrEmpty(DbTableName))
            {
                _dataAdapter.SelectCommand = UniCommandBuilder.GetSelectCommand(SchemaName, TableName);
            }
            UniSchemaColumn[] cols = SchemaTableManager.GetUpdatedColumns(TableName).ToArray();
            if (cols == null) return;

            _dataAdapter.AcceptChangesDuringUpdate = false;

            _dataAdapter.InsertCommand = UniCommandBuilder.GetInsertCommand(SchemaName, TableName, cols);
            _dataAdapter.UpdateCommand = UniCommandBuilder.GetUpdateCommand(SchemaName, TableName, cols);
            _dataAdapter.DeleteCommand = UniCommandBuilder.GetDeleteCommand(SchemaName, TableName, cols);
        }

        UniDbConnection _connection = UniDbConnection.Current;
        /// <summary>
        /// Соединение используемое коллекцией
        /// </summary>
        public UniDbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        /// <summary>
        /// Сохранение данных с использованием внутренней транзакции
        /// </summary>
        /// <returns></returns>
        public virtual Exception Save()
        {
            if (DataAdapter == null)
                InitializeAdapter();

            IDbTransaction tr = Connection.BeginTransaction();
            try
            {
                string primaryColumn = SchemaOfTable.Columns.FirstOrDefault(r => r.IsPrimaryKey).DbColumnName;
                if (!string.IsNullOrEmpty(primaryColumn))
                    foreach (DataRow r in Table.Rows)
                        if (r.RowState == DataRowState.Added && r[primaryColumn] != DBNull.Value)
                            r[primaryColumn] = DBNull.Value;
                _dataAdapter.Update(Table);
                tr.Commit();
                Table.AcceptChanges();
                return null;
            }
            catch (Exception ex)
            {
                tr.Rollback();
                return ex;
            }
        }

        /// <summary>
        /// Обновление данных коллекци с помощью адаптера
        /// </summary>
        public void RefreshTableView()
        {
            if (DataAdapter == null)
                InitializeAdapter();
            if (Table == null)
                Table = new DataTable();
            DataAdapter.Fill(Table);
            ClearItems();
            foreach (var item in GetItems())
                Add(item);
            OnPropertyChanged(new PropertyChangedEventArgs("Count"));
        }

        public IEnumerable<T> GetItems()
        {
            var items = from p in Table.AsEnumerable()
                        where p.RowState != DataRowState.Deleted && p.RowState != DataRowState.Detached
                        select new T() { DataRow = p };
            return items;
        }

        
       
        public void Dispose()
        {
            Table.Dispose();
            DataAdapter.Dispose();
        }
        
        /// <summary>
        /// Метод помечает все записи в таблицы для удаления (при сохранении удалит из базы данных)
        /// </summary>
        public void RemoveAll()
        {
            for (int i = Count - 1; i > -1; --i)
            {
                RemoveItem(i);
            }
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        protected override void RemoveItem(int index)
        {
            T value = this[index];
            if (value.DataRow != null && value.DataRow.RowState != DataRowState.Deleted)
                value.DataRow.Delete();
            base.RemoveItem(index);
        }

        protected override void InsertItem(int index, T item)
        {
            if (item.DataRow == null)
                item.DataRow = Table.Rows.Add();
            base.InsertItem(index, item);
        }
   
        /// <summary>
        /// Имеются ли изменения в таблице данных
        /// </summary>
        public bool HasChanges
        {
            get
            {
                return Table?.GetChanges() != null;
            }
        }

        /// <summary>
        /// Перегруженный метод очищает и коллекцию,и связанные данные в таблице
        /// </summary>
        protected override void ClearItems()
        {
            for (int i = Count - 1; i > -1; --i)
            {
                Items[i].DataRow.Delete();
            }
            Table.AcceptChanges();
            base.ClearItems();
        }
    }
}
