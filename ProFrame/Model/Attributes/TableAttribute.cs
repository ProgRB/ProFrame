using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    [AttributeUsageAttribute(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute: Attribute
    {
        public TableAttribute()
        {
        }

        public string Name
        {
            get;set;
        }

        public string SchemaName
        {
            get;set;
        }
    }
}
