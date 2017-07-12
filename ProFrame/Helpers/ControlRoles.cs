using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ProFrame
{
    /// <summary>
    /// Класс для просмотра доступных пользователю команд (компонентов) 
    /// </summary>
    public class ControlRoles
    {
        private static HashSet<string> _controlRoles;
        static ControlRoles()
        {
            _controlRoles = new HashSet<string>();
            LoadControlRoles();
        }
        /// <summary>
        /// Проверка доступности пользователю компонента 
        /// </summary>
        /// <param name="ControlName">Имя компонента</param>
        /// <returns></returns>
        public static bool GetState(string ControlName)
        {
            return _controlRoles.Contains((ControlName??string.Empty).ToUpper());
        }
        /// <summary>
        /// Проверка доступности пользователю команды 
        /// </summary>
        /// <param name="cmd">Название команды</param>
        /// <returns></returns>
        public static bool GetState(ICommand cmd)
        {
            if (cmd is RoutedUICommand)
                return GetState((cmd as RoutedUICommand).Name);
            else return false;
        }
        /// <summary>
        /// Загрузка справочника доступных пользователю команд (компонентов)
        /// </summary>
        public static void LoadControlRoles()
        {
            if (UniDbConnection.Current != null)
            {
                _controlRoles.Clear();
                DataTable t = new DataTable();
                UniDbCommand com = new UniDbCommand(
                    $@"select distinct COMPONENT_NAME from {AppConstants.Schema_Name_Handbook}.CONTROL_USER_VIEW 
                    where APP_NAME_ID = :p_APP_NAME_ID 
                    order by COMPONENT_NAME",
                    UniDbConnection.Current);
                com.Parameters.Add("p_APP_NAME_ID", UniDbType.Int, AppConstants.App_Name_ID);
                try
                {
                    new UniDbAdapter(com).Fill(t);
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                for (int i = 0; i < t.Rows.Count; ++i)
                    if (!_controlRoles.Contains(t.Rows[i]["Component_Name"].ToString().ToUpper()))
                    {
                        _controlRoles.Add(t.Rows[i]["Component_Name"].ToString().ToUpper());
                    }
            }
        }
    }
}
