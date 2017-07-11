using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    [AttributeUsageAttribute(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute:Attribute
    {
        /// <summary>
        /// Может ли колонка принимать значение Null
        /// </summary>
        public bool CanBeNull
        {
            get;set;
        }

        /// <summary>
        /// Являетлся ли поле первичным ключом
        /// </summary>
        public bool IsPrimaryKey
        {
            get;set;
        }

        /// <summary>
        /// Имя столца соответствующего базе данных
        /// </summary>
        public string Name
        {
            get;set;
        }

        /// <summary>
        /// Комментарий к столбцу
        /// </summary>
        public string Comment
        {
            get;set;
        }

        /// <summary>
        /// Тип в базе данных (строка названия)
        /// </summary>
        public string DbType
        {
            get;set;
        }
    }
}
