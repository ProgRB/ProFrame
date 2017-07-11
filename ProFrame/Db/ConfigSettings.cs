using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace DbProviderConfiguration
{
    public class ProviderConfigSection : ConfigurationSection
    {
        [ConfigurationProperty("Providers")]
        [ConfigurationCollection(typeof(ProviderCollection), AddItemName = "ProviderElement")]
        public ProviderCollection ProvidersItems
        {
            get { return ((ProviderCollection)(base["Providers"])); }
        }
    }

    [ConfigurationCollection(typeof(ProviderElement))]
    public class ProviderCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new ProviderElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((ProviderElement)(element)).EnumName;
        }

        public ProviderElement this[string key]
        {
            get
            {
                return (ProviderElement)base.BaseGet(key);
            }
        }

        public ProviderElement this[int idx]
        {
            get { return (ProviderElement)BaseGet(idx); }
        }
    }

    public class ProviderElement : ConfigurationElement
    {

        /// <summary>
        /// Имя сборки откуда брать необходимые компоненты провайдера данных
        /// </summary>
        [ConfigurationProperty("enumName", DefaultValue = "", IsKey = true, IsRequired = true)]
        public string EnumName
        {
            get { return ((string)(base["enumName"])); }
        }

        /// <summary>
        /// Имя сборки откуда брать необходимые компоненты провайдера данных
        /// </summary>
        [ConfigurationProperty("assemblyName", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string AssemblyName
        {
            get { return ((string)(base["assemblyName"])); }
        }

        /// <summary>
        /// Пространство имен сборки откуда брать необходимые компоненты провайдера данных
        /// </summary>
        [ConfigurationProperty("assemblyNameSpace", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string AssemblyNameSpace
        {
            get { return ((string)(base["assemblyNameSpace"])); }
        }

        /// <summary>
        /// Имя класса для создания подключения IDbConnection
        /// </summary>
        [ConfigurationProperty("connectionClassName", DefaultValue = "OracleConnection", IsKey = false, IsRequired = true)]
        public string ConnectionClassName
        {
            get { return ((string)(base["connectionClassName"])); }
        }

        /// <summary>
        /// Имя класса для создания команды IDbCommand
        /// </summary>
        [ConfigurationProperty("commandClassName", DefaultValue = "OracleCommand", IsKey = false, IsRequired = true)]
        public string CommandClassName
        {
            get { return ((string)(base["commandClassName"])); }
        }

        /// <summary>
        /// Имя класса для создания транзакции IDbTransaction
        /// </summary>
        [ConfigurationProperty("transactionClassName", DefaultValue = "OracleTransaction", IsKey = false, IsRequired = true)]
        public string TransactionClassName
        {
            get { return ((string)(base["transactionClassName"])); }
        }

        /// <summary>
        /// Имя класса для создания чтения данных IDbDataReader
        /// </summary>
        [ConfigurationProperty("dataReaderClassName", DefaultValue = "OracleDataReader", IsKey = false, IsRequired = true)]
        public string DataReaderClassName
        {
            get { return ((string)(base["dataReaderClassName"])); }
        }

        /// <summary>
        /// Имя класса для создания чтения данных IDataAdapter
        /// </summary>
        [ConfigurationProperty("dataAdapterClassName", DefaultValue = "OracleDataAdapter", IsKey = false, IsRequired = true)]
        public string DataAdapterClassName
        {
            get { return ((string)(base["dataAdapterClassName"])); }
        }

        /// <summary>
        /// Имя класса для создания чтения данных IDataAdapter
        /// </summary>
        [ConfigurationProperty("connectionString", DefaultValue = "", IsKey = false, IsRequired = true)]
        public string ConnectionString
        {
            get { return ((string)(base["connectionString"])); }
        }

    }
}
