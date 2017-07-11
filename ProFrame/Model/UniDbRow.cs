using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace ProFrame
{
    public class UniDbRow: IDataErrorInfo, INotifyPropertyChanged
    {
        public UniDbRow()
        {
        }


        public UniDbRow(DataRow row)
        {
            DataRow = row;
        }


        bool isRowUpdating = false; //признак находится ли строка сейчас на стадии обновления
        string _schemaTable = string.Empty;
        /// <summary>
        /// Схема в которой содержится таблица БД
        /// </summary>
        public string SchemaTable
        {
            get
            {
                return _schemaTable;
            }
            set
            {
                _schemaTable = value;
            }
        }

        string _tableName;
        /// <summary>
        /// Имя таблицы в базе данных для получения или сохранения данных
        /// </summary>
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                RaisePropertyChanged("TableName");
            }
        }

        DataRow _dataRow;
        /// <summary>
        /// Строка содержащая данные модели
        /// </summary>
        public DataRow DataRow
        {
            get
            {
                return _dataRow;
            }
            set
            {
                _dataRow = value;
                RaisePropertyChanged("DataRow");
            }
        }

        DataTable _dataTable;
        /// <summary>
        /// Таблица в которой содержатся данные
        /// </summary>
        public DataTable DataTable
        {
            get
            {
                if (_dataRow == null)
                    return _dataTable;
                else
                    return _dataRow.Table;
            }
            set
            {
                if (_dataRow == null)
                    _dataTable = value;
                else
                    throw new Exception("Невозможно установить DataTable когда уже установлено свойство DataRow");
            }
        }

        /// <summary>
        /// Набор данных включающих данную модель
        /// </summary>
        public DataSet DataSet
        {
            get
            {
                return DataTable?.DataSet;
            }
        }


        #region UPdate Data
        /// <summary>
        /// Установка данных строки напрямую
        /// </summary>
        /// <typeparam name="T">Тип устанавливаемых данных</typeparam>
        /// <param name="propertyName">Имя свойства</param>
        /// <param name="value">значение</param>
        public void SetProperty<T>(string propertyName, object value)
        {
            isRowUpdating = true;
            DataRow[propertyName] = value?? DBNull.Value;
            isRowUpdating = false;
        }

        /// <summary>
        /// Установка данных строки. Требуется схема модели для установки (в коде или XML файле)
        /// </summary>
        /// <typeparam name="T">Тип</typeparam>
        /// <param name="expr">Устанавливаемое поле класса</param>
        /// <param name="value">Устанавливаемое значение</param>
        public void SetProperty<T>(Expression<Func<T>> expr, object value)
        {
            if (_dataRow?.RowState != DataRowState.Deleted)
            {
                object[] attrib_info = (expr.Body as MemberExpression).Member.GetCustomAttributes(true);
                {
                    ColumnAttribute dm = attrib_info.OfType<ColumnAttribute>().FirstOrDefault();
                    if (dm != null)
                    {
                        SetProperty<T>(dm.Name, value);
                        RaisePropertyChanged((expr.Body as MemberExpression).Member.Name);
                        RaisePropertyChanged(() => Error);
                    }
                    else
                        throw new Exception("Для свойства не установлен поле источник данных DataRow (Member Name)");
                }
            }
        }

        /// <summary>
        /// Получение данных строки
        /// </summary>
        /// <typeparam name="T">Тип получаемых данных</typeparam>
        /// <param name="propertyName">Получаемое имя свойства</param>
        /// <returns></returns>
        public T GetProperty<T>(string propertyName) 
        {
            if (DataRow[propertyName] == DBNull.Value)
                return default(T);
            else
                if (DataRow.RowState == DataRowState.Deleted)
                return (T)DataRow[propertyName, DataRowVersion.Original];
            else return (T)DataRow[propertyName];
        }

        /// <summary>
        /// Получение значения поля по наименованию свойства класса
        /// </summary>
        /// <typeparam name="T">Получаемый тип данных</typeparam>
        /// <param name="expr">Получаемое выражение</param>
        /// <returns></returns>
        public T GetProperty<T>(Expression<Func<T>> expr)
        {
            if (_dataRow?.RowState != DataRowState.Deleted)
            {
                object[] attrib_info = (expr.Body as MemberExpression).Member.GetCustomAttributes(true);
                ColumnAttribute dm = (expr.Body as MemberExpression).Member.GetCustomAttributes(true).OfType<ColumnAttribute>().FirstOrDefault();
                if (dm != null)
                {
                    return GetProperty<T>(dm.Name);
                }
                else
                    throw new Exception("Для свойства не установлен поле источник данных DataRow (DataMember)");
            }
            return default(T);
        }
        #endregion

        #region IDataError info
        /// <summary>
        /// Получает текст ошибки по имени поля
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public string this[string columnName]
        {
            get
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Получает текст ошибки для всей модели
        /// </summary>
        public string Error
        {
            get
            {
                return string.Empty;
            }
        }
        #endregion

        #region Property changed раздел
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            var propertyName = GetPropertyName(action);
            RaisePropertyChanged(propertyName);
        }

        /// <summary>
        /// Получает строковое значение поля для выражения 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="action"></param>
        /// <returns></returns>
        private static string GetPropertyName<T>(Expression<Func<T>> action)
        {
            var expression = (MemberExpression)action.Body;
            var propertyName = expression.Member.Name;
            return propertyName;
        }

        public void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
