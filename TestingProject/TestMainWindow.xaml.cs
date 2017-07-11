using ProFrame;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace TestingProject
{
    /// <summary>
    /// Логика взаимодействия для TestMainWindow.xaml
    /// </summary>
    public partial class TestMainWindow : MainWindowBase
    {
        public static RoutedUICommand ComTest { get; private set; }

        public TestMainWindow()
        {
            OpenTabs.AddNewTab("Test Telerik", new UserControl());
            InitializeComponent();
        }

        private void Button2_Click(object sender, RoutedEventArgs e)
        {
            OpenTabs.AddNewTab("Test Telerik.GridView", new UserControl());
        }
        
        static TestMainWindow()
        {
            ComTest = new RoutedUICommand("TestCommand", "TestCommand", typeof(TestMainWindow));
        }

        private void Button_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            if (ControlRoles.GetState((e.Command as RoutedUICommand).Name))
                e.CanExecute = true;
        }

        private void Button_Executed(object sender, ExecutedRoutedEventArgs e)
        {
        }
    }
}
