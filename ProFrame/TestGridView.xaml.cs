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
using Telerik.Windows.Controls;

namespace ProFrame
{
    /// <summary>
    /// Логика взаимодействия для TestGridView.xaml
    /// </summary>
    public partial class TestGridView : UserControl
    {
        public TestGridView()
        {
            InitializeComponent(); 
            DataSet _ds = new DataSet();
            UniDbAdapter _da = new UniDbAdapter("SELECT PER_NUM FROM APSTAFF.EMP", UniDbConnection.Current);
            _da.Fill(_ds, "EMP");
            dgEmp.DataContext = _ds.Tables["EMP"].DefaultView;
        }
    }
}
