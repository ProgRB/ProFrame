using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace ProFrame
{
    public class UniDbConnection : IDbConnection, IDisposable
    {
        #region Текущее соединение для использования
        static UniDbConnection _current;

        /// <summary>
        /// Текущее соединение сессии. Используется одно соединение для обеспечения производительности
        /// </summary>
        public static UniDbConnection Current
        {
            get
            {
                return _current;
            }
            private set
            {
                _current = value;
            }
        }
        #endregion

        /// <summary>
        /// внутренник класс использовать будут для хранения экземпляра соединения
        /// </summary>
        DbConnection _connect;

        /// <summary>
        /// Конструктор соединения с параметрами
        /// </summary>
        /// <param name="userName">имя пользователя для подключения</param>
        /// <param name="password">пароль для подключения</param>
        public UniDbConnection(string userName, string password)
        {
            
            _connect  = ActivatorHelper.CreateAndUnwrap<DbConnection>(ProviderSetting.ConfigurationProvider.ConnectionClassName);
            switch (ProviderSetting.CurrentDBProvider)
            {
                case DbProviderType.OracleOdpNetManaged:  _connect.ConnectionString = string.Format(ProviderSetting.ConfigurationProvider.ConnectionString, userName, "\""+password+"\"");break;
                case DbProviderType.OracleOdpNetUnmanaged:  _connect.ConnectionString = string.Format(ProviderSetting.ConfigurationProvider.ConnectionString, userName, "\"" + password + "\"");break;
                default:  _connect.ConnectionString = string.Format(ProviderSetting.ConfigurationProvider.ConnectionString, userName, password);break;
            }
        }

        static UniDbConnection()
        {
#if DEBUG
            AppDomain.CurrentDomain.AssemblyLoad += CurrentDomain_AssemblyLoad;
#endif
        }

        private static void CurrentDomain_AssemblyLoad(object sender, AssemblyLoadEventArgs args)
        {
            Debug.WriteLine(args.LoadedAssembly.FullName);
        }

        internal DbConnection InternalConnection
        {
            get
            {
                return _connect;
            }
        }

        #region Создание или попытка подключения соединения

        /// <summary>
        /// Попытка открытия текущего соединения
        /// </summary>
        /// <param name="userName">имя пользователя</param>
        /// <param name="password"> пароль</param>
        /// <returns>Возвращает пару Результат, Исключение. Если соединение открыто результат = Открыто, исключение не указано</returns>
        public static Tuple<ConnectingResult, Exception> OpenCurrentConnection(string userName, string password)
        {
            try
            {
                _current = new UniDbConnection(userName, password);
                _current.UserID = userName;
                _current.Password = password;
                _current.Open();
                return new Tuple<ConnectingResult, Exception>(ConnectingResult.Open, null);
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "OracleException")
                {
                    PropertyInfo prop = ex.GetType().GetProperty("Number");
                    if (prop != null)
                    {
                        int error_code = (int)ex.GetType().GetProperty("Number").GetValue(ex, null);
                        switch (error_code)
                        {
                            case 1017: return new Tuple<ConnectingResult, Exception>(ConnectingResult.InvalidPassword, new Exception("Неверное имя пользователя или пароль", ex));
                            case 28001: return new Tuple<ConnectingResult, Exception>(ConnectingResult.PasswordExpired, new Exception("Срок действия пароля истек. Требуется изменить пароль", ex));
                            case 28000: return new Tuple<ConnectingResult, Exception>(ConnectingResult.AccountLock, new Exception("Пользователь заблокирован", ex));
                            default: return new Tuple<ConnectingResult, Exception>(ConnectingResult.OtherError, ex);
                        }
                    }
                }
                return new Tuple<ConnectingResult, Exception>(ConnectingResult.OtherError, ex);
            }
        }

        /// <summary>
        /// Метод создает новое соединение, если уже есть текущее соедиение (корректный логин, пароль)
        /// </summary>
        /// <returns>Возвращает тройку Результат, Ошибка, Соединение </returns>
        public static Tuple<ConnectingResult, Exception, UniDbConnection> OpenNew()
        {
            try
            {
                UniDbConnection connect = new UniDbConnection(Current.UserID, Current.Password);
                connect.Open();
                return new Tuple<ConnectingResult, Exception, UniDbConnection>(ConnectingResult.Open, null, connect);
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "OracleException")
                {
                    PropertyInfo prop = ex.GetType().GetProperty("Number");
                    if (prop != null)
                    {
                        int error_code = (int)ex.GetType().GetProperty("Number").GetValue(ex, null);
                        switch (error_code)
                        {
                            case 1017: return new Tuple<ConnectingResult, Exception, UniDbConnection>(ConnectingResult.InvalidPassword, new Exception("Неверное имя пользователя или пароль", ex), null);
                            case 28001: return new Tuple<ConnectingResult, Exception, UniDbConnection>(ConnectingResult.PasswordExpired, new Exception("Срок действия пароля истек. Требуется изменить пароль", ex), null);
                            case 28000: return new Tuple<ConnectingResult, Exception, UniDbConnection>(ConnectingResult.AccountLock, new Exception("Пользователь заблокирован", ex), null);
                            default: return new Tuple<ConnectingResult, Exception, UniDbConnection>(ConnectingResult.OtherError, ex, null);
                        }
                    }
                }
                return new Tuple<ConnectingResult, Exception, UniDbConnection>(ConnectingResult.OtherError, ex, null);
            }
        }

        /// <summary>
        /// Процедура смены пароля пользователя
        /// </summary>
        /// <param name="new_password">новый пароль</param>
        /// <param name="passwordExpired">истина, если срок действия пароля истек</param>
        /// <returns></returns>
        public Tuple<string, Exception> ChangePassword(string new_password, bool passwordExpired)
        {
            Tuple<bool, string> res = CheckPasswordPattern(new_password, this.UserID);
            if (res.Item1)
            {
                switch (ProviderSetting.CurrentDBProvider)
                {
                    case DbProviderType.OracleOdpNetUnmanaged:
                    case DbProviderType.OracleOdpNetManaged:
                    case DbProviderType.OracleMicrosoft: var ex = ChangeOracleUserPassword(new_password, this, passwordExpired);
                                                         if (ex!=null) return new Tuple<string, Exception>("Ошибка изменения пароля", ex); break;
                    default: return new Tuple<string, Exception>("Данный метод не реализован для поставщика данных", new NotSupportedException());
                }
                return new Tuple<string, Exception>("Пароль успешно изменен", null);
            }
            else
                return new Tuple<string, Exception>(res.Item2, new Exception("Пароль не соответствует требованиям безопасности"));
            
        }

        /// <summary>
        /// Команда изменения пароля на новый.
        /// </summary>
        /// <param name="new_pass"></param>
        /// <param name="connect1"></param>
        private Exception ChangeOracleUserPassword(string new_pass, UniDbConnection connect1, bool isExpired)
        {
            try
            {
                if (isExpired)
                {
                    MethodInfo mi = connect1.InternalConnection.GetType().GetMethod("OpenWithNewPassword");
                    if (mi != null)
                    {
                        mi.Invoke(connect1.InternalConnection, new object[] { new_pass });
                    }
                    else
                        new UniDbCommand(string.Format("ALTER USER {0} IDENTIFIED BY \"{1}\"", UserID, new_pass), connect1).ExecuteNonQuery();
                }
                else
                    new UniDbCommand(string.Format("ALTER USER {0} IDENTIFIED BY \"{1}\"", UserID, new_pass), connect1).ExecuteNonQuery();
                return null;
            }
            catch (Exception ex)
            {
                return ex;
            }

        }

        /// <summary>
        /// Имя пользователя для подключения
        /// </summary>
        public string UserID
        {
            get;set;
        }

        protected static byte[] pass_data;
        /// <summary>
        /// Пароль пользователя храним в зашифрованном виде
        /// </summary>
        protected string Password
        {
            get
            {
                return ProtectString.Unprotect(pass_data);
            }
            private set
            {
                pass_data = ProtectString.Protect(value);
            }
        }

        /// <summary>
        /// Проверка пароля на соответствие правилам
        /// </summary>
        /// <param name="password"></param>
        /// <param name="user_name"></param>
        /// <returns></returns>
        public static Tuple<bool, string> CheckPasswordPattern(string password, string user_name)
        {
            if (Regex.IsMatch(user_name.ToUpper(), password.ToUpper())) // пароль не является подстрокой имени пользователя
                return new Tuple<bool, string>(false, "Пароль не должен быть частью имени пользователя");
            if (password.Contains('"')) return new Tuple<bool, string>(false, "Пароль не может содержать двойные кавычки"); // не содержит двойных кавычек (чтобы не было проблем)
            if (password.Length < 3) return new Tuple<bool, string>(false, "Длина пароля не может быть менее трех знаков"); // не менее 3х симоволов
            return new Tuple<bool, string>(true, "");
        }


        #endregion

        #region IConnection region
        /// <summary>
        /// Строка подключения
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return _connect.ConnectionString;
            }

            set
            {
                _connect.ConnectionString = value;
            }
        }

        public int ConnectionTimeout
        {
            get
            {
                return _connect.ConnectionTimeout;
            }
        }

        public string Database
        {
            get
            {
                return _connect.Database;
            }
        }

        public ConnectionState State
        {
            get
            {
                return _connect.State;
            }
        }

        public IDbTransaction BeginTransaction()
        {
            return _connect.BeginTransaction();
        }

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            return _connect.BeginTransaction(il);
        }

        public void ChangeDatabase(string databaseName)
        {
            _connect.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            _connect.Close();
        }

        public IDbCommand CreateCommand()
        {
            return _connect.CreateCommand();
        }

        public void Open()
        {
            _connect.Open();
        }
        #endregion


        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                _connect.Dispose();
                _connect = null;
                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~UniDbConnection() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            //GC.SuppressFinalize(this);
        }
        #endregion
    }

    public enum ConnectingResult
    {
        Open = 1,
        InvalidPassword = 2,
        PasswordExpired = 3,
        AccountLock = 4,
        ImpossibleNewPassword = 5,
        OtherError = 6
    }
}
