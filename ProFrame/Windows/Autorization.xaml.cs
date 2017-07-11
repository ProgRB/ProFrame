using System;
using System.ComponentModel;
using System.IO;
using System.Linq.Expressions;
using System.Windows;
using System.Windows.Input;

namespace ProFrame
{
    /// <summary>
    /// Interaction logic for Autorization.xaml
    /// </summary>
    public partial class Autorization : Window, INotifyPropertyChanged
    {
        public Autorization()
        {
            InitializeComponent();
            try
            {
                string st = AppDomain.CurrentDomain.FriendlyName;
                StreamReader r = new StreamReader(new FileStream(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                    $@"\{AppDomain.CurrentDomain.FriendlyName}Login.ini", FileMode.Open, FileAccess.Read));
                string s = r.ReadLine();
                r.Close();
                user_name.Text = s;
            }
            catch { }
        }

        public Autorization(string username, string password)
        {
            InitializeComponent();
            UserName = username;
            Password = password;  
            this.Loaded += new RoutedEventHandler(Autorization_Loaded);          
        }

        void Autorization_Loaded(object sender, RoutedEventArgs e)
        {
            this.btOk_Click(this, null);
        }

        public string UserName
        {
            get
            {
                return user_name.Text.Trim();
            }
            set
            {
                user_name.Text = value;
            }
        }
        public string Password
        {
            get
            {
                return pass.Password;
            }
            set
            {
                pass.Password = value;
            }
        }

        public string NewPass
        {
            get
            {
                return new_pass.Password;
            }
        }
        private void btOk_Click(object sender, RoutedEventArgs e)
        {
            Tuple<ConnectingResult, Exception> res = UniDbConnection.OpenCurrentConnection(UserName, Password);
            if (res.Item2 == null)
            {
                try
                {
                    File.WriteAllLines(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData) +
                         $@"\{AppDomain.CurrentDomain.FriendlyName}Login.ini", new string[] { UserName.ToUpper() });
                }
                catch { };
                DialogResult = true;
                //this.Close();
            }
            else
            {
                MessageBox.Show(res.Item2.Message);
                if (res.Item1 == ConnectingResult.PasswordExpired) PasswordChangingState = true;
            }
        }
        private void btOkChanges_Click(object sender, RoutedEventArgs e)
        {
            if (new_pass.Password != new_pass1.Password)
            {
                MessageBox.Show(this, "Подтверждение нового пароля не совпадает с новым паролем!", "Зарплата предприятия", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                // Делаем попытку создать соединение чтобы в дальнейшем использовать его для смены пароля
                Tuple<ConnectingResult, Exception> res = UniDbConnection.OpenCurrentConnection(UserName, Password);
                // Запускаем метод смены пароля
                Tuple<string, Exception> changePass = UniDbConnection.Current.ChangePassword(new_pass.Password, true);
                if (changePass.Item2 == null)
                {
                    MessageBox.Show("Поздравляю! Пароль успешно изменен. Не сообщайте пароль третьим лицам. Вся ответственность за действия под вашим пользователем ложится на Вас",
                        "Изменение пароля",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    Password = new_pass.Password;
                    // Открываем соединение уже с новым паролем и закрываем форму авторизации
                    btOk_Click(null, null);
                    //UniDbConnection.OpenCurrentConnection(UserName, Password);
                    //this.DialogResult = true;
                    //Close();
                }
                else
                {
                    MessageBox.Show(changePass.Item2.Message);
                }
            }
        }

        private void btExit_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                btOk_Click(null, null);
        }
        
        private void Window_Activated(object sender, EventArgs e)
        {
            if (user_name.Text != "")
            {
                pass.Focus();
            }
        }

        bool _isChangePass = false;
        public bool PasswordChangingState
        {
            get
            {
                return _isChangePass;
            }
            set
            {
                _isChangePass = value;
                OnPropertyChanged(() => PasswordChangingState);
            }
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            this.DragMove();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged<T>(Expression<Func<T>> mem)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs((mem.Body as MemberExpression).Member.Name));
            }
        }
    }
}
