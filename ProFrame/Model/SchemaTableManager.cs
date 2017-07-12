using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace ProFrame
{
    /// <summary>
    /// Класс управляет загрузкой схемы данных таблиц или представлений
    /// </summary>
    public class SchemaTableManager
    {
        /// <summary>
        /// Имя схемы используемой по умолчанию в приложении
        /// </summary>
        public static string DefaultSchemaName
        {
            get;set;
        }
        static string settingFileName = AppLocalPath.CurrentAppPath + @"\"+"AppSchema.xml";
        static UniSchemaTable[] _tables;
        /// <summary>
        /// Список таблиц
        /// </summary>
        public static UniSchemaTable[] Tables
        {
            get
            {
                if (_tables == null)
                    LoadSchemaTables();
                return _tables;
            }
        }

        /// <summary>
        /// Загрузка данных по схемам таблиц
        /// </summary>
        public static void LoadSchemaTables()
        {
            if (File.Exists(settingFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(UniSchemaTable), new XmlRootAttribute("Tables"));
                FileStream f = File.Open(settingFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                object v=xs.Deserialize(f);
                _tables = (UniSchemaTable[])v; 
            }
            else
                _tables = new UniSchemaTable[]{ };
        }

        static Dictionary<string, UniSchemaTable> _dbdictionary;
        static Dictionary<string, UniSchemaTable> _dictionary;
        /// <summary>
        /// Получаем схему таблицы по имени
        /// </summary>
        /// <param name="tableName">Имя таблицы</param>
        /// <param name="byDbName">искать по имени таблицы как в базе данных или как нормализованное имя для С#</param>
        /// <returns>Возвращает схему таблицу</returns>
        public static UniSchemaTable GetTable(string tableName, bool byDbName = true)
        {
            if (byDbName)
            {
                if (_dbdictionary == null)
                {
                    _dbdictionary = Tables.ToDictionary(r => r.TableDbName, r => r, StringComparer.OrdinalIgnoreCase);
                }
                UniSchemaTable t = null;
                if (_dbdictionary.TryGetValue(tableName, out t))
                    return t;
                else
                    return null;
            }
            else
            {
                if (_dictionary == null)
                {
                    _dictionary = Tables.ToDictionary(r => r.TableName, r => r, StringComparer.OrdinalIgnoreCase);
                }
                UniSchemaTable t = null;
                if (_dictionary.TryGetValue(tableName, out t))
                    return t;
                else
                    return null;
            }
        }

        /// <summary>
        /// Получаем имя таблицы в базе данных по типу данных
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public static string GetDbTableName(Type t)
        {
            //Получаем сначала атрибут класса. Он первичный для определения имени
            object[] prop = t.GetCustomAttributes(typeof( TableAttribute), true);
            if (prop != null && prop.Length > 0)
            {
                return (prop[0] as TableAttribute).Name;
            }
            else
            {
                UniSchemaTable st = GetTable(t.Name, false);
                if (st!=null)
                    return st.TableDbName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Имя схемы таблицы в базе даннных
        /// </summary>
        /// <param name="type">тип данных для проверки схемы</param>
        /// <returns>Имя схемы</returns>
        public static string GetSchemaName(Type type)
        {
            //Получаем сначала атрибут класса. Он первичный для определения имени схемы
            object[] prop = type.GetCustomAttributes(typeof(TableAttribute), true);
            if (prop != null && prop.Length > 0)
            {
                return (prop[0] as TableAttribute).SchemaName;
            }
            else
            {
                UniSchemaTable st = GetTable(type.Name, false);
                if (st != null)
                    return st.SchemaName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Получаем имя поля первичного ключа по типу класса
        /// </summary>
        /// <param name="type">Тип класса у которого требуется получить первичный ключ</param>
        /// <returns>имя первичного ключа или пусто</returns>
        public static string GetPrimaryKeyField(Type type)
        {
            //Получаем сначала атрибут класса. Он первичный для определения имени схемы
            PropertyInfo prop = type.GetProperties().Where(r=>r.GetCustomAttributes(typeof(ColumnAttribute), true).Any(t=>(t as ColumnAttribute).IsPrimaryKey))
               .FirstOrDefault();
            // если существует поле с признаком первичного ключа, то берем его название
            if (prop != null)
            {
                ColumnAttribute ca = prop.GetCustomAttributes(typeof(ColumnAttribute), true).First() as ColumnAttribute;
                return ca.Name;
            }
            else
            {
                UniSchemaTable st = GetTable(type.Name, false);
                if (st != null)
                    return st.Columns.Where(r=>r.IsPrimaryKey).FirstOrDefault()?.DbColumnName;
            }
            return string.Empty;
        }

        /// <summary>
        /// Создание структуры таблицы по имени таблицы в БД
        /// </summary>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static DataTable CreateTable(string tableName, string schemaTable = null)
        {
            if (string.IsNullOrWhiteSpace(tableName))
                throw new Exception("Ошибка получения схемы. Не задано имя таблицы");
            UniSchemaTable st = GetTable(tableName);
            if (st != null)
            {
                DataTable table = new DataTable(tableName);
                foreach (var item in st.Columns)
                {
                    table.Columns.Add(item.DbColumnName, item.ColumnType);
                }
                table.DisplayExpression = st.TableComment;
                return table;
            }
            else
            {
                DataTable t = new DataTable(tableName);
                UniDbAdapter oda = new UniDbAdapter(UniDbCommand.GetSelectCommand(tableName, schemaTable, "1=2"));
                oda.Fill(t);
                return t;
            }
        }

        /// <summary>
        /// Метод получения столбцов для обновления или добавления данных
        /// </summary>
        /// <returns>Возвращает список столбцов которые терубется обновлять в базе данных</returns>
        public static IEnumerable<UniSchemaColumn> GetUpdatedColumns(string tableName, DataTable table = null)
        {
            UniSchemaTable stable = SchemaTableManager.GetTable(tableName); //здесь требуется реализовать чтение схемы таблицы из текстового файла Реализую позже кажется реализовал

            if (stable != null)
                return stable.Columns;
            else
            {
                return table.Columns.Cast<DataColumn>().Select(r => new UniSchemaColumn()
                {
                    ColumnName = r.ColumnName,
                    DbColumnName = r.ColumnName,
                    ColumnType = r.DataType,
                    DbColumnType = UniDbTypeHelper.GetUniDbType(r.DataType),
                    IsPrimaryKey = (r.ColumnName).Equals(tableName + "_ID", StringComparison.OrdinalIgnoreCase)
                });
            }
        }

    }
}
