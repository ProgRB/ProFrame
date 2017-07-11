using DbProviderConfiguration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace ProFrame
{
    public class ProviderSetting
    {
        private static DbProviderType _currentDBProvider = DbProviderType.OracleOdpNetManaged;
        /// <summary>
        /// Текущий провайдер для подключения к базе данных
        /// </summary>
        public static DbProviderType CurrentDBProvider
        {
            get
            {
                return _currentDBProvider;
            }
            set
            {
                if (_currentDBProvider != value)
                {
                    _currentDBProvider = value;
                    LoadProviderLibrary();
                }
            }
        }

        static string _schemaHelper;
        /// <summary>
        /// Схема в которой содержатся вспомогательные классы
        /// </summary>
        public static string DbHelperSchema
        {
            get
            {
                return _schemaHelper;
            }
            set
            {
                _schemaHelper = value;
            }
        }

        public static AppDomain ConnectorDomain
        {
            get;set;
        }

        /// <summary>
        /// Загрузка библиотеки приложения в домен для работы с ней
        /// </summary>
        public static void LoadProviderLibrary()
        {
            try
            {
               /* if (ConnectorDomain != null)
                {
                    Debug.WriteLine("Domain for provider has been unloaded");
                    AppDomain.Unload(ConnectorDomain);
                    ConnectorDomain = null;
                }
                ConnectorDomain = AppDomain.CreateDomain("ConnectorDomain");*/
                /*if (!string.IsNullOrEmpty(ConfigurationProvider.AssemblyName))
                    ConnectorDomain.Load(ConfigurationProvider.AssemblyName);*/
                Debug.WriteLine("Assembly "+ ConfigurationProvider.AssemblyName+" is loaded!");
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Ошибка работы с доменом соединения или библиотекой подключения: " + ex.Message);
            }
        }

        static ProviderElement _config;
        /// <summary>
        /// Конфигурация провайдера - подключения данных.
        /// </summary>
        public static ProviderElement ConfigurationProvider
        {
            get
            {
                if (_config == null)
                {
                    try
                    {
                        var section = (ProviderConfigSection) System.Configuration.ConfigurationManager.GetSection("ProviderSettings");
                        string currstring = CurrentDBProvider.ToString();
                        _config = section.ProvidersItems[currstring];
                        Debug.WriteLine("Выбран провайдер: " + _config.AssemblyName);
                        return _config;
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка получения настроек провайдера данных: " + ex.Message);
                        return null;
                    }
                }
                else
                    return _config;
            }
        }
    }

    public enum DbProviderType
    {
        OracleOdpNetUnmanaged = 1,
        OracleOdpNetManaged = 2,
        OracleMicrosoft =3
    }

}
