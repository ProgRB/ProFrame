using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace TestingProject
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        string _username, _password;
        protected override void OnStartup(StartupEventArgs e)
        {
            this.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
            this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            ProFrame.ProviderSetting.CurrentDBProvider = ProFrame.DbProviderType.OracleOdpNetUnmanaged;
            ProFrame.AppConstants.App_Name = "TestingProject";
            ProFrame.AppConstants.App_Name_ID = 13;
            ProFrame.AppConstants.Schema_Name_Handbook = "APSTAFF";
            base.OnStartup(e);

            // Если запуск запуск приложения должен осуществляться с формой Авторизации использовать следующий код
            //ParseArgs(e.Args);
            //ProFrame.ProviderSetting.CurrentDBProvider = ProFrame.DbProviderType.OracleOdpNetUnmanaged;
            //ProFrame.Windows.Autorization auto;
            //if (!string.IsNullOrEmpty(_username))
            //{
            //    auto = new ProFrame.Windows.Autorization(_username, _password);
            //}
            //else
            //{
            //    auto = new ProFrame.Windows.Autorization();
            //}
            //if (!(auto.ShowDialog() ?? false))
            //    this.Shutdown();
            //else
            //{
            //    this.StartupUri = new Uri("MainWindow.xaml", UriKind.Relative);
            //    base.OnStartup(e);
            //}
        }

        /// <summary>
        /// процедура обработки входных аргументов
        /// </summary>
        /// <param name="args"></param>
        public void ParseArgs(string[] args)
        {
            try
            {
                args = new string[] { "ffff" }.Concat(args).ToArray();
                int k = args.Select((p, i) => new { param = p, index = i }).FirstOrDefault(it => it.param.ToUpper() == "-USER").index;
                if ((k != default(int)) && args.Length > k + 1)
                {
                    _username = args[k + 1];
                }
                k = args.Select((p, i) => new { param = p, index = i }).FirstOrDefault(it => it.param.ToUpper() == "-PASS").index;
                if (k != default(int) && args.Length > k + 1)
                {
                    _password = args[k + 1];
                }
            }
            catch { };
        }
    }
}
