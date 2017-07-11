using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    /// <summary>
    /// Атрибут для класса, помогающий установить занчения для команды Оракл
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Method | AttributeTargets.Field)]
    public class UniParameterMapping : Attribute
    {
        /// <summary>
        /// Имя параметра, которому требуется присвоить искомое значение
        /// </summary>
        public string ParameterName
        {
            get;
            set;
        }
    }
}
