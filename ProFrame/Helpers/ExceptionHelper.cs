using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ProFrame
{
    public static class ExceptionHelper
    {
        public static string GetFormattedException(this Exception ex)
        {
            if (ex.GetType().Name == "OracleException")
            {
                PropertyInfo prop = ex.GetType().GetProperty("Number");
                if (prop != null)
                {
                    int error_code = (int)ex.GetType().GetProperty("Number").GetValue(ex, null);
                    string message = ex.GetType().GetProperty("Message").GetValue(ex, null).ToString();
                    if (error_code > 19999 && error_code < 25000 && message.Length > 0)
                    {
                        return message.Substring(0, message.IndexOf("ORA-", 10, StringComparison.CurrentCultureIgnoreCase));
                    }
                    else
                    {
                        switch (error_code)
                        {
                            case 1013: return "Пользователь прервал операцию";
                            case 910: return "Строковый параметр превышает установленную длину";
                            case 1031: return "Не достаточно привилегий";
                            case 1033: return "Сервер базы данных находится в состоянии запуска или отключения";
                            case 1034: return "Сервер базы данных недоступен";
                            case 1062: return "Недостаточно памяти на сервере для создания буфера указанно размера";
                        }
                        return ex.Message;
                    }
                }
                else return "UnhandledOracleException: " + ex.Message;
            }
            else return ex.Message;
        }
    }
}
