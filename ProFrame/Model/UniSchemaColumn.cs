using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProFrame
{
    /// <summary>
    /// Класс представляющий аналог колонки столбца в модели данных
    /// </summary>
    [Serializable]
    [XmlType("Column")]
    public class UniSchemaColumn
    {
        public UniSchemaColumn()
        {
        }
        /// <summary>
        /// Наименование в модели
        /// </summary>
        [XmlAttribute(AttributeName ="FriendlyName")]
        public string ColumnName
        {
            get;set;
        }

        /// <summary>
        /// Тип колонки в модели
        /// </summary>
        [XmlAttribute(AttributeName ="Type")]
        public string ColumnTypeString
        {
            get;set;
        }

        [XmlIgnore]
        public Type ColumnType
        {
            get
            {
                if (ColumnTypeString == null)
                    return null;
                else
                   return  Type.GetType(ColumnTypeString);
            }
            set
            {
                ColumnTypeString = value.FullName;
            }
        }

        /// <summary>
        /// Наименование столбца в базе данных
        /// </summary>
        [XmlAttribute(AttributeName = "Name")]
        public string DbColumnName
        {
            get;set;
        }

        /// <summary>
        /// Тип значения в базе данных (для команды обновления)
        /// </summary>
        [XmlAttribute(AttributeName = "DBType")]
        public UniDbType DbColumnType
        {
            get; set;
        } = UniDbType.String;

        [XmlAttribute(AttributeName ="IsUpdatable")]
        public bool IsUpdatable
        {
            get; set;
        } = true;

        [XmlAttribute(AttributeName= "IsPrimaryKey")]
        public bool IsPrimaryKey
        {
            get; set;
        } = false;

        [XmlAttribute(AttributeName = "Comment")]
        public string CommentText
        {
            get;set;
        }

        [XmlAttribute(AttributeName = "ToolTipText")]
        public string ToolTipText
        {
            get;set;
        }

        [XmlAttribute(AttributeName = "IsVisible")]
        public bool IsVisible
        {
            get;set;
        }
    }
}
