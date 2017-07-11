using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public static class AppLocalPath
    {
        /// <summary>
        /// Возвращает папку где выполняется приложение. Корневой каталог
        /// </summary>
        public static string CurrentAppPath
        {
            get { return System.IO.Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName); }
        }
    }
}
