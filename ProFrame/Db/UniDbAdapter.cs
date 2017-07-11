using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ProFrame
{
    /// <summary>
    /// Адаптер данных. Планируется универсальность
    /// </summary>
    public class UniDbAdapter: DbDataAdapter
    {
        DbDataAdapter _adapter;

        /// <summary>
        /// Конструктор адаптера данных
        /// </summary>
        /// <param name="commandText">текст команды</param>
        /// <param name="connection"></param>
        public UniDbAdapter(string commandText, UniDbConnection connection):this()
        {
            this.SelectCommand = new UniDbCommand(commandText, connection);
        }

        public UniDbAdapter(UniDbCommand cmd):this()
        {
            SelectCommand = cmd;
        }

        public UniDbAdapter()
        {
            _adapter = ActivatorHelper.CreateAndUnwrap<DbDataAdapter>(ProviderSetting.ConfigurationProvider.DataAdapterClassName);
        }

        public override int Fill(DataSet dataSet)
        {
            return _adapter.Fill(dataSet);
        }
        public new int Fill(DataTable dataTable)
        {
            return _adapter.Fill(dataTable);
        }

        public new int Fill(DataSet dataSet, string srcTable)
        {
            return _adapter.Fill(dataSet, srcTable);
        }
        public new int Fill(int startRecord, int maxRecordCounts, params DataTable[] tables)
        {
            return _adapter.Fill(startRecord, maxRecordCounts, tables);
        }

        public new int Fill(DataSet dataSet, int startRecord, int maxRecordCounts, string srsTable)
        {
            return _adapter.Fill(dataSet, startRecord, maxRecordCounts, srsTable);
        }

        public override DataTable[] FillSchema(DataSet dataSet, SchemaType schemaType)
        {
            return base.FillSchema(dataSet, schemaType);
        }

        public new DataTableMappingCollection TableMappings
        {
            get
            {
                return _adapter.TableMappings;
            }
        }

        public override int Update(DataSet dataSet)
        {
            return _adapter.Update(dataSet);
        }

        public new int Update(DataRow[] rows)
        {
            return _adapter.Update(rows);
        }

        public new int Update(DataTable table)
        {
            return _adapter.Update(table);
        }

        public new int Update(DataSet dataSet, string tableName)
        {
            return this.Update(dataSet.Tables[tableName]);
        }

        UniDbCommand _selectCommand;
        /// <summary>
        /// Команда выбора данных адаптера
        /// </summary>
        public new UniDbCommand SelectCommand
        {
            get
            {
                return _selectCommand;
            }
            set
            {
                _selectCommand = value;
                _adapter.SelectCommand = value?.InternalCommand;
            }
        }

        UniDbCommand _insertCommand;
        /// <summary>
        /// Команда вставки данных в базу данных
        /// </summary>
        public new UniDbCommand InsertCommand
        {
            get
            {
                return _insertCommand;
            }
            set
            {
                _insertCommand = value;
                _adapter.InsertCommand = value?.InternalCommand;
            }
        }

        UniDbCommand _updateCommand;
        /// <summary>
        /// Команда обновления данных в базе данных
        /// </summary>
        public new UniDbCommand UpdateCommand
        {
            get
            {
                return _updateCommand;
            }
            set
            {
                _updateCommand = value;
                _adapter.UpdateCommand = value?.InternalCommand;
            }
        }

        UniDbCommand _deleteCommand;
        /// <summary>
        /// Команда обновления данных в базе данных
        /// </summary>
        public new UniDbCommand DeleteCommand
        {
            get
            {
                return _deleteCommand;
            }
            set
            {
                _deleteCommand = value;
                _adapter.DeleteCommand = value?.InternalCommand;
            }
        }
    }
}
