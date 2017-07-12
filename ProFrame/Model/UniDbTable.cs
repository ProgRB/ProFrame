using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace ProFrame.Model
{
    public class UniDbTable<T>:ObservableCollectionViewBase<T> where T: UniDbRow
    {
        public UniDbTable(string tableName, string schemaName = null)
        {
            TableName = tableName;
            if (!string.IsNullOrEmpty(schemaName))
                SchemaName = schemaName;
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
                _dataAdapter.SelectCommand = UniCommandBuilder.GetSelectCommand(SchemaName, DbTableName);
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

        public void RefreshTableView()
        {
            if (DataAdapter == null)
                InitializeAdapter();
            if (Table == null)
                Table = new DataTable();
            DataAdapter.Fill(Table);
            
        }
                
    }
}
