using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public class UniParameter : DbParameter
    {
        DbParameter _parameter;

        /// <summary>
        /// Конструктор параметра без 
        /// </summary>
        public UniParameter()
        {
            switch (ProviderSetting.CurrentDBProvider)
            {
                case DbProviderType.OracleOdpNetUnmanaged:
                case DbProviderType.OracleOdpNetManaged: _parameter = ActivatorHelper.CreateAndUnwrap<DbParameter>("OracleParameter"); break;
                case DbProviderType.OracleMicrosoft: _parameter = ActivatorHelper.CreateAndUnwrap<DbParameter>("OracleParameter"); break;
                default: throw new Exception("Для провайдера не установлен конструктор параметров");
            }
        }

        public UniParameter(string name, UniDbType paramType, object value):this()
        {
            ParameterName = name;
            UniDbType = paramType;
            Value = value;
        }

        internal UniParameter(DbParameter value)
        {
            _parameter = value;
        }

        public DbParameter InternalParameter
        {
            get
            {
                return _parameter;
            }
        }

        /// <summary>
        /// Унаследованный тип. Не используется
        /// </summary>
        public override DbType DbType
        {
            get
            {
                return _parameter.DbType;
            }

            set
            {
                _parameter.DbType = value;
            }
        }

        UniDbType _uniType;
        /// <summary>
        /// Тип параметра в базе данных. Используйте его для установки параметров
        /// </summary>
        public UniDbType UniDbType
        {
            get
            {
                return _uniType;
            }
            set
            {
                switch (ProviderSetting.CurrentDBProvider)
                {
                    case DbProviderType.OracleOdpNetUnmanaged:
                    case DbProviderType.OracleOdpNetManaged: MapOracleDbTypes(value); break;
                    default: throw new Exception("Нет сопоставления типов параметров и Net платформы");
                }
                _uniType = value;
            }
        }

        #region Static initializer map types

        static Dictionary<DbProviderType, Dictionary<UniDbType, string>> dictionaryMapType;
        static UniParameter()
        {
            dictionaryMapType = new Dictionary<DbProviderType, Dictionary<UniDbType, string>>();
            dictionaryMapType.Add(DbProviderType.OracleOdpNetManaged,
                new Dictionary<UniDbType, string>
                {
                    { UniDbType.Decimal, "Decimal" },
                    { UniDbType.DateTime, "Date" },
                    { UniDbType.String, "Varchar2" },
                    { UniDbType.RefCursor, "RefCursor" },
                    { UniDbType.Blob, "Blob" },
                    { UniDbType.Clob, "Clob" },
                    { UniDbType.ArrayNumber, "Varchar2" }
                }
                );
            dictionaryMapType.Add(DbProviderType.OracleOdpNetUnmanaged,
                new Dictionary<UniDbType, string>
                {
                    { UniDbType.Decimal, "Decimal" },
                    { UniDbType.DateTime, "Date" },
                    { UniDbType.String, "Varchar2" },
                    { UniDbType.RefCursor, "RefCursor" },
                    { UniDbType.Blob, "Blob" },
                    { UniDbType.Clob, "Clob" },
                    { UniDbType.ArrayNumber, "Array" }
                }
                );
        }
        #endregion

        /// <summary>
        /// Сопоставляем тип универсальный и типы оракла
        /// </summary>
        /// <param name="uniType"></param>
        private void MapOracleDbTypes(UniDbType uniType)
        {
            Dictionary<UniDbType, string> p;
            if (dictionaryMapType.TryGetValue(ProviderSetting.CurrentDBProvider, out p))
            {
                if (p.ContainsKey(uniType))
                {
                    string enumString = p[uniType];
                    // получаем  перечисление оракловое type
                    var enumType = Type.GetType($"{ProviderSetting.ConfigurationProvider.AssemblyNameSpace}.OracleDbType, {ProviderSetting.ConfigurationProvider.AssemblyName}");
                    // получаем оракловое значение
                    var enumValue = enumType.GetField(enumString).GetValue(null);
                    _parameter.GetType().GetProperty("OracleDbType").SetValue(_parameter, enumValue, null);
                }
            }
        }

        /// <summary>
        /// Направление параметра
        /// </summary>
        public override ParameterDirection Direction
        {
            get
            {
                return _parameter.Direction;
            }

            set
            {
                _parameter.Direction = value;
            }
        }

        public override bool IsNullable
        {
            get
            {
                return _parameter.IsNullable;
            }

            set
            {
                _parameter.IsNullable = value;
            }
        }

        public override string ParameterName
        {
            get
            {
                return _parameter.ParameterName;
            }

            set
            {
                _parameter.ParameterName = value;
            }
        }

        public override int Size
        {
            get
            {
                return _parameter.Size;
            }

            set
            {
                _parameter.Size = value;
            }
        }

        public override string SourceColumn
        {
            get
            {
                return _parameter.SourceColumn;
            }

            set
            {
                _parameter.SourceColumn = value;
            }
        }

        public override bool SourceColumnNullMapping
        {
            get
            {
                return _parameter.SourceColumnNullMapping;
            }

            set
            {
                _parameter.SourceColumnNullMapping = value;
            }
        }

        public override DataRowVersion SourceVersion
        {
            get
            {
                return _parameter.SourceVersion;
            }

            set
            {
                _parameter.SourceVersion = value;
            }
        }

        /// <summary>
        /// Значение параметра. Не всегда совпадает с тем что передается в запрос. Для запроса используется InternalValue
        /// </summary>
        public override object Value
        {
            get
            {
                return _internalValue;
            }

            set
            {
                _internalValue = value;
                _parameter.Value = ConvertUniToValue(value);
            }
        }

        object _internalValue;
        /// <summary>
        /// Внутреннее значение для подстановки в параметры
        /// </summary>
        public object InternalValue
        {
            get
            {
                return _internalValue;
            }
        }

        private object ConvertUniToValue(object value)
        {
            if (UniDbType == UniDbType.ArrayNumber)
            {
                if (ProviderSetting.CurrentDBProvider == DbProviderType.OracleOdpNetManaged)
                {
                    if (value is IEnumerable)
                    {
                        var array = value as IEnumerable;
                        return $"{ProviderSetting.DbHelperSchema}.NUMBER_COLLECTION_TYPE({string.Join(",", array)})";
                    }
                }
            }
            return value;
        }

        public override void ResetDbType()
        {
            _parameter.ResetDbType();
        }
    }
}
