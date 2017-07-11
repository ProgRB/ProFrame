using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    /// <summary>
    /// Класс для просмотра предоставленных пользователю ролей
    /// </summary>
    public class GrantedRoles
    {
        private static HashSet<string> _grantedRoles;
        static GrantedRoles()
        {
            _grantedRoles = new HashSet<string>(StringComparer.CurrentCultureIgnoreCase);
            LoadGrantedRoles();
        }
        /// <summary>
        /// Загрузка справочника предоставленных ролей
        /// </summary>
        public static void LoadGrantedRoles()
        {
            _grantedRoles.Clear();
            if (UniDbConnection.Current != null)
            {
                UniDbDataReader drGrantedRoles = new UniDbCommand(string.Format(
                    $@"select COLUMN_VALUE as GRANTED_ROLE from TABLE({AppConstants.Schema_Name_Handbook}.GET_GRANTED_ROLES)"), UniDbConnection.Current).ExecuteReader();
                while (drGrantedRoles.Read())
                {
                    _grantedRoles.Add(drGrantedRoles["GRANTED_ROLE"].ToString());
                }
            }
        }
        /// <summary>
        /// Проверка предоставлена ли роль пользователю
        /// </summary>
        /// <param name="RoleName">Имя роли для проверки</param>
        /// <returns></returns>
        public static bool CheckRole(string RoleName)
        {
            return _grantedRoles.Contains(RoleName);
        }
        /// <summary>
        /// Количество предоставленных пользователю ролей
        /// </summary>
        public static int CountGrantedRoles
        {
            get { return _grantedRoles.Count(); }
        }
    }
}
