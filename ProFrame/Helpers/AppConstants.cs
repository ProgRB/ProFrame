using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public static class AppConstants
    {
        private static string _app_Name = "";
        /// <summary>
        /// Имя приложения из справочника приложений
        /// </summary>
        public static string App_Name
        {
            get
            {
                return _app_Name;
            }

            set
            {
                _app_Name = value;
            }
        }

        private static int _app_Name_ID = 0;
        /// <summary>
        /// ID приложения из справочника приложений
        /// </summary>
        public static int App_Name_ID
        {
            get
            {
                return _app_Name_ID;
            }

            set
            {
                _app_Name_ID = value;
            }
        }

        private static string _schema_Name_Handbook = "";
        /// <summary>
        /// Схема, в которой хранятся таблицы доступа к функциям приложения, доступные подразделения и т.п.
        /// </summary>
        public static string Schema_Name_Handbook
        {
            get
            {
                return _schema_Name_Handbook;
            }

            set
            {
                _schema_Name_Handbook = value;
            }
        }
    }
}
