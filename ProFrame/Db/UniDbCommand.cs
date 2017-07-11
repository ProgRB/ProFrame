using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Data;
using System.Reflection;

namespace ProFrame
{
    public class UniDbCommand : DbCommand, IDisposable
    {
        DbCommand _command;

        /// <summary>
        /// Конструктор команды 
        /// </summary>
        /// <param name="commandText">Текст команды. Соединение используется текущее</param>
        public UniDbCommand(string commandText):this()
        {
            this.CommandText = commandText;
            this.Connection = UniDbConnection.Current.InternalConnection;
        }

        /// <summary>
        /// Конструктор команды
        /// </summary>
        /// <param name="commandText">текст Команды</param>
        /// <param name="connection">Соединение для команды</param>
        public UniDbCommand(string commandText, UniDbConnection connection):this()
        {
            this.CommandText = commandText;
            this.Connection = connection.InternalConnection;
        }


        public UniDbCommand()
        {
            _command = ActivatorHelper.CreateAndUnwrap<DbCommand>(ProviderSetting.ConfigurationProvider.CommandClassName);
            switch (ProviderSetting.CurrentDBProvider)
            {
                case DbProviderType.OracleOdpNetUnmanaged: SetBindByName(true);break;
                case DbProviderType.OracleOdpNetManaged: SetBindByName(true);break;
            }
        }


        /// <summary>
        /// Установка свойства BindByName=true для ораклКомманды
        /// </summary>
        /// <param name="value"></param>
        private void SetBindByName(bool value)
        {
            PropertyInfo prop = _command.GetType().GetProperty("BindByName");
            if (prop != null)
            {
                prop.SetValue(_command, value, null);
            }
        }

        internal DbCommand InternalCommand
        {
            get
            {
                return _command;
            }
        }

        #region IDbCommand 
        public override string CommandText
        {
            get
            {
                return _command.CommandText;
            }

            set
            {
                _command.CommandText = value;
            }
        }

        public override int CommandTimeout
        {
            get
            {
                return _command.CommandTimeout;
            }

            set
            {
                _command.CommandTimeout = value;
            }
        }

        public override CommandType CommandType
        {
            get
            {
                return _command.CommandType;
            }

            set
            {
                _command.CommandType = value;
            }
        }

        protected override DbConnection DbConnection
        {
            get
            {
                return _command.Connection;
            }

            set
            {
                _command.Connection = value;
            }
        }

        UniDbParameterCollection _params;

        /// <summary>
        /// Параметры команды запроса
        /// </summary>
        public new UniDbParameterCollection Parameters
        {
            get
            {
                if (_params == null)
                    _params = new UniDbParameterCollection(_command.Parameters);
                return _params;
            }
        }

        public override UpdateRowSource UpdatedRowSource
        {
            get
            {
                return _command.UpdatedRowSource;
            }

            set
            {
                _command.UpdatedRowSource = value;
            }
        }

        protected override DbParameterCollection DbParameterCollection
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override DbTransaction DbTransaction
        {
            get
            {
                return null;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool DesignTimeVisible
        {
            get
            {
                return false;
            }

            set
            {
                throw new NotImplementedException();
            }
        }

     
        public override void Cancel()
        {
            _command.Cancel();
        }

        public new IDbDataParameter CreateParameter()
        {
            return _command.CreateParameter();
        }

        public override int ExecuteNonQuery()
        {
            return _command.ExecuteNonQuery();
        }

        public new UniDbDataReader ExecuteReader()
        {
            return new UniDbDataReader(_command.ExecuteReader());
        }

        public new UniDbDataReader ExecuteReader(CommandBehavior behavior)
        {
            return new UniDbDataReader(_command.ExecuteReader(behavior));
        }

        public override object ExecuteScalar()
        {
            return _command.ExecuteScalar();
        }

        public override void Prepare()
        {
            _command.Prepare();
        }

        protected override DbParameter CreateDbParameter()
        {
            return null;
        }
        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UniDbCommand() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }



        #endregion


        #region Other class helpers
        /// <summary>
        /// Данная процедура автоматически проставляет команде значения из класса согласно мэппингу атрибутов UniParameterMapping
        /// </summary>
        /// <param name="sourceValue">Источник значений, каждое значение для параметра должно быть отмечено атрибутом [OracleParameterMapping]</param>
        public void SetParameters(object sourceValue)
        {
            foreach (UniParameter c in Parameters.Cast<UniParameter>().Where(r => r.Direction != System.Data.ParameterDirection.Output))
            {
                PropertyInfo p = sourceValue.GetType().GetProperties()
                    .Where(r => r.GetCustomAttributes(typeof(UniParameterMapping), true)
                                .Any(r1 => (r1 as UniParameterMapping).ParameterName.Equals(c.ParameterName, StringComparison.OrdinalIgnoreCase))
                                ).FirstOrDefault();
                if (p != null)
                {
                    c.Value = p.GetValue(sourceValue, null);
                    continue;
                }
                FieldInfo p1 = sourceValue.GetType().GetFields()
                    .Where(r => r.GetCustomAttributes(typeof(UniParameterMapping), true)
                                .Any(r1 => (r1 as UniParameterMapping).ParameterName.Equals(c.ParameterName, StringComparison.OrdinalIgnoreCase))
                                ).FirstOrDefault();
                if (p1 != null)
                {
                    c.Value = p1.GetValue(sourceValue);
                    continue;
                }

                MethodInfo p2 = sourceValue.GetType().GetMethods()
                    .Where(r => r.GetCustomAttributes(typeof(UniParameterMapping), true)
                                .Any(r1 => (r1 as UniParameterMapping).ParameterName.Equals(c.ParameterName, StringComparison.OrdinalIgnoreCase))
                                ).FirstOrDefault();
                if (p2 != null)
                {
                    c.Value = p2.Invoke(sourceValue, null);
                    continue;
                }
            }
        }

        /// <summary>
        /// Статичный метод получения запроса выбора всех данных таблицы
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="schemaName">Имя схемы</param>
        /// <returns>возвращает строку запроса типа select * from HR.EMPS </returns>
        public static string GetSelectString(string tableName, string schemaName)
        {
            switch (ProviderSetting.CurrentDBProvider)
            {
                case DbProviderType.OracleOdpNetManaged:
                case DbProviderType.OracleOdpNetUnmanaged:
                case DbProviderType.OracleMicrosoft: return $"select * from {schemaName}.{tableName}";break;
            }
            return string.Empty;
        }

        /// <summary>
        /// Метод создания команды выбора данных из таблицы
        /// </summary>
        /// <param name="tableName">наименование таблицы</param>
        /// <param name="schemaName">схема таблицы</param>
        /// <param name="filter">фильтр данных таблицы</param>
        /// <returns>Возвращает команду выбора данных по условию</returns>
        public static UniDbCommand GetSelectCommand(string tableName, string schemaName,  string filter)
        {
            UniDbCommand cmd = null;
            switch (ProviderSetting.CurrentDBProvider)
            {
                case DbProviderType.OracleOdpNetManaged:
                case DbProviderType.OracleOdpNetUnmanaged:
                case DbProviderType.OracleMicrosoft:
                    if (string.IsNullOrWhiteSpace(filter))
                        cmd = new UniDbCommand($"select * from {schemaName}.{tableName}");
                    else
                        cmd = new UniDbCommand($"select * from {schemaName}.{tableName} where {filter}");
                    return cmd;
                    break;
            }
            return cmd;
        }
        #endregion
    }
}
