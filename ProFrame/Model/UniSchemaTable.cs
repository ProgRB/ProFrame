using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProFrame
{
    [Serializable]
    [XmlType("Table")]
    public class UniSchemaTable
    {
        public UniSchemaTable()
        {
            Columns = new List<UniSchemaColumn>();
        }
        /// <summary>
        /// Имя таблицы (сущности)
        /// </summary>
        [XmlAttribute(AttributeName ="FriendlyName")]
        public string TableName
        {
            get;set;
        }

        /// <summary>
        /// Имя таблицы в базе данных
        /// </summary>
        [XmlAttribute(AttributeName ="Name")]
        public string TableDbName
        {
            get; set;
        }

        /// <summary>
        /// Имя схемы в базе данных
        /// </summary>
        [XmlAttribute(AttributeName ="SchemaName")]
        public string SchemaName
        {
            get; set;
        }

        /// <summary>
        /// Комментарий к таблице
        /// </summary>
        [XmlAttribute(AttributeName = "Comment")]
        public string TableComment
        {
            get;set;
        }

        [XmlArray("Columns")]
        public List<UniSchemaColumn> Columns
        {
            get;set;
        }

        [NonSerialized]
        Dictionary<string, UniSchemaColumn> _columns;

        /// <summary>
        /// Получаем схему колонки по наименованию 
        /// </summary>
        /// <param name="columnName"></param>
        /// <returns></returns>
        [XmlIgnore]
        public UniSchemaColumn this[string columnName]
        {
            get
            {
                if (_columns == null)
                {
                    _columns = Columns.ToDictionary(r => r.DbColumnName, r => r, StringComparer.OrdinalIgnoreCase);
                }
                UniSchemaColumn p;
                if (_columns.TryGetValue(columnName, out p))
                    return p;
                else
                    return null;
            }
        }
    }
}
