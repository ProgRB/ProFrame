using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ProFrame
{
    /// <summary>
    /// Логика взаимодействия для WindowsForms_Viewer.xaml
    /// </summary>
    public partial class WindowsForms_Viewer : UserControl, IWindowsForms_Viewer
    {
        public WindowsForms_Viewer(System.Windows.Forms.Control form)
        {
            InitializeComponent();
            windowsFormsHost.Child = form;
        }

        public System.Windows.Forms.Control ChildForm
        {
            get { return windowsFormsHost.Child; }
        }

        public Type TypeChildForm
        {
            get { return windowsFormsHost.Child.GetType(); }
        }
    }
}
