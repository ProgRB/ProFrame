using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ProFrame
{
    public class UniDbModel : UniDbRow
    {


        public UniDbModel(string tableName):this()
        {
            TableName = tableName;
        }

        public UniDbModel() : base()
        {
            _connection = UniDbConnection.Current;
            if (string.IsNullOrWhiteSpace(TableName))
                TableName = SchemaTableManager.GetDbTableName(this.GetType());
            if (string.IsNullOrWhiteSpace(SchemaTable))
                SchemaTable = SchemaTableManager.GetSchemaName(this.GetType());

        }

        #region Commands and connection region

        UniDbConnection _connection;
        /// <summary>
        /// Соединение необходимое для сохранения/получения данных
        /// </summary>
        public UniDbConnection Connection
        {
            get
            {
                return _connection;
            }
        }

        UniDbCommand _selectCommand;
        /// <summary>
        /// Команда выбора данных для модели
        /// </summary>
        public UniDbCommand SelectCommand
        {
            get
            {
                return _selectCommand;
            }
            set
            {
                _selectCommand = value;
                RaisePropertyChanged(()=>SelectCommand);
            }
        }

        public virtual void CreateSelectCommand()
        {
            if (string.IsNullOrWhiteSpace(TableName) || string.IsNullOrWhiteSpace(SchemaTable))
                return;
                //throw new Exception("Не установлено имя таблицы и схемы для получения данных")
            _selectCommand = UniDbCommand.GetSelectCommand(TableName, SchemaTable, $"{TableName}_id=:p_{TableName}_id");
            _selectCommand.Parameters.Add("p_" + TableName + "_id", UniDbType.Decimal, null);
        }

        /// <summary>
        /// Получаем строку базы данных в модель по первичному ключу (айдишнику) таблицы
        /// </summary>
        /// <param name="primaryKeyValue"></param>
        public void GetDbRowData(object primaryKeyValue)
        {
            if (DataTable == null)
            {
                DataTable = new DataTable(TableName);
            }
            if (SelectCommand == null)
            {
                CreateSelectCommand();
                if (SelectCommand == null)
                    throw new Exception("Не установлена команда получения данных таблицы");
            }
            if (SelectCommand.Parameters.Count > 0 && primaryKeyValue != null) // если есть параметры команды, то думаем что надо поставить параметр выбора
                SelectCommand.Parameters[0].Value = primaryKeyValue;
            UniDbDataReader reader = SelectCommand.ExecuteReader();
            DataTable.BeginLoadData();
            DataTable.Load(reader);
            DataTable.EndLoadData();
            reader.Close();
            if (DataTable.Rows.Count > 0)
                DataRow = DataTable.Rows[DataTable.Rows.Count - 1];
        }

        /// <summary>
        /// Получаем класс и данные (если существуют)
        /// </summary>
        /// <typeparam name="T">Тип создаваемых данных</typeparam>
        /// <param name="primaryKeyValue">Значение первичного ключа</param>
        /// <param name="ds">Набор данных если требуется</param>
        /// <returns>Возвращает элемент созданный</returns>
        public static T GetOrCreate<T>(object primaryKeyValue, DataSet ds = null) where T:UniDbModel, new()
        {
            if (ds == null)
            {
                ds = new DataSet();
            }
            string tableName = SchemaTableManager.GetDbTableName(typeof(T));
            string schemaName = SchemaTableManager.GetSchemaName(typeof(T));
            DataTable table = null;
            if (!ds.Tables.Contains(tableName)) // если таблицы нету, то надо создать с структурой указанной в модели таблицу.
            {
                table = SchemaTableManager.CreateTable(tableName, schemaName);
                ds.Tables.Add(table);
            }
            else
                table = ds.Tables[tableName];
            T res = new T();
            res.DataTable = table;
            res.GetDbRowData(primaryKeyValue);
            if (res.DataRow == null)
            {
                DataRow row = res.DataTable.NewRow();
                res.DataTable.Rows.Add(row);
                res.DataRow = row;
            }
            return res;
        }

        /// <summary>
        /// Инициализация адаптера сохранения данных
        /// </summary>
        public virtual void InitializeAdapter()
        {
            _dataAdapter = new UniDbAdapter();
            UniSchemaColumn[] cols = SchemaTableManager.GetUpdatedColumns(TableName, DataTable).ToArray();
            if (cols == null) return;
            
            _dataAdapter.AcceptChangesDuringUpdate = false;

            _dataAdapter.InsertCommand = UniCommandBuilder.GetInsertCommand(SchemaTable, TableName, cols);
            _dataAdapter.UpdateCommand = UniCommandBuilder.GetUpdateCommand(SchemaTable, TableName, cols);
            _dataAdapter.DeleteCommand = UniCommandBuilder.GetDeleteCommand(SchemaTable, TableName, cols);
            
        }

        /// <summary>
        /// Удаление модели данных командой
        /// </summary>
        /// <returns></returns>
        public virtual Exception Delete()
        {
            object keyValue = GetProperty<object>(SchemaTableManager.GetPrimaryKeyField(this.GetType()));
            if (_dataAdapter != null && _dataAdapter.DeleteCommand != null)
            {
                UniDbCommand deleteCmd = new UniDbCommand(_dataAdapter.DeleteCommand.CommandText, this.Connection);
                if (_dataAdapter.DeleteCommand.Parameters.Count > 0)
                {
                    UniParameter p = _dataAdapter.DeleteCommand.Parameters[0];
                    deleteCmd.Parameters.Add(p.ParameterName, p.UniDbType, keyValue);
                }
                else
                    deleteCmd.Parameters.Add("p_" + SchemaTableManager.GetPrimaryKeyField(this.GetType()), UniDbType.Decimal, keyValue);
                return ExecuteDeleteCommand(deleteCmd);
            }
            else
            {
                UniDbCommand deleteCmd = UniCommandBuilder.GetDeleteCommand(SchemaTable, TableName, SchemaTableManager.GetUpdatedColumns(TableName, DataTable));
                deleteCmd.Parameters[0].Value = keyValue;
                deleteCmd.Parameters[0].SourceColumn = string.Empty;
                return ExecuteDeleteCommand(deleteCmd);
            }
        }

        /// <summary>
        /// Удаление объекта модели
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectValue"></param>
        /// <returns></returns>
        public static Exception Delete<T>(T objectValue) where T:UniDbModel
        {
            return objectValue.Delete();
        }

        internal Exception ExecuteDeleteCommand(UniDbCommand cmd)
        {
            IDbTransaction transact = Connection.BeginTransaction();
            try
            {
                cmd.ExecuteNonQuery();
                DataRow.Delete();
                DataRow.AcceptChanges();
                transact.Commit();
                return null;
            }
            catch (Exception ex)
            {
                transact.Rollback();
                return ex;
            }
        }

        /// <summary>
        /// Сохранение данных модели
        /// </summary>
        /// <returns>возвращает ошибку возникшую при сохранении данных или null при успешном сохранении данных</returns>
        public virtual Exception Save()
        {
            return Save(null);
        }


        /// <summary>
        /// Сохранение данных модели с использованием открытой транзакции
        /// </summary>
        /// <param name="currentTransaction">текущая открытая транзакция для модели</param>
        /// <returns>возвращает ошибку возникшую при сохранении данных или null при успешном сохранении данных</returns>
        public virtual Exception Save(IDbTransaction currentTransaction)
        {
            if (DataAdapter == null)
                InitializeAdapter();
            if (DataAdapter == null)
                return new Exception("Не иницилизирован адаптер сохранения данных модели");
            IDbTransaction transact = currentTransaction ?? UniDbConnection.Current.BeginTransaction();
            try
            {
                DataAdapter.Update(new DataRow[] { this.DataRow });
                transact.Commit();
                if (currentTransaction == null)
                    DataRow.AcceptChanges();
                return null;
            }
            catch (Exception ex)
            {
                if (currentTransaction == null)
                    transact.Rollback();
                return ex;
            }
        }

       

        UniDbAdapter _dataAdapter;
        /// <summary>
        /// Адаптер данных
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
                RaisePropertyChanged(() => DataAdapter);
            }
        }

        #endregion
    }
}