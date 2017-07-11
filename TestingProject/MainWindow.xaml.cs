using ProFrame;
using System;
using System.Collections.Generic;
using System.Windows;

namespace TestingProject
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            App.Current.Shutdown();
        }

        
        private void TestConnect_Click(object sender, RoutedEventArgs e)
        {
            UniDbConnection connect = new UniDbConnection("knvtest", "3");
            connect.Open();
            MessageBox.Show("Успешное соединение");

            UniDbCommand cmd = new UniDbCommand("select * from apstaff.emp where emp_birth_date=:p_date and per_num=:p_per_num and perco_sync_id=:p_perco_sync_id  order by per_num", connect);
            cmd.Parameters.Add("p_per_num", UniDbType.String, "14534");
            cmd.Parameters.Add("p_date", UniDbType.DateTime, new DateTime(1989, 7, 8));
            cmd.Parameters.Add("p_perco_sync_id", UniDbType.Decimal, 9479);
            /*var p = cmd.ExecuteScalar();
            MessageBox.Show($"значение={p}");*/

            UniDbDataReader dr = cmd.ExecuteReader();
            List<string> ls = new List<string>();
            while (dr.Read())
            {
                ls.Add(dr["PER_NUM"].ToString());
            }
            MessageBox.Show($"Загружен список Reader ом, кол-во равно = {ls.Count}; last per_num={ls[ls.Count-1]}");
        }

        private void TestAutorization_Click(object sender, RoutedEventArgs e)
        {
            //Autorization auto = new Autorization("OLM05367", "456");

            // Тестирование формы авторизации
            /*Autorization auto = new Autorization();
            if (auto.ShowDialog() == true)
            {
                // Тестирование отчетности через OpenXml
                /*UniDbCommand cmd = new UniDbCommand(
                    "select EMP_LAST_NAME as \"Фамилия\", EMP_FIRST_NAME as \"Имя\", EMP_MIDDLE_NAME as \"Отчество\" from apstaff.emp where PER_NUM between '12714' and '18888'", 
                    UniDbConnection.Current);
                System.Data.DataTable table = new System.Data.DataTable();
                UniDbAdapter adapter = new UniDbAdapter(cmd.CommandText, UniDbConnection.Current);
                adapter.Fill(table);
                System.Data.DataSet ds = new System.Data.DataSet();
                ds.Tables.Add(table);

                adapter.TableMappings.Add("Table", "Сотрудники");
                adapter.Fill(ds);
                ExcelWithOpenXml.CreateExcelFile(AppLocalPath.CurrentAppPath + @"\1234.xlsx", ds);
                ExcelWithOpenXml.CreateExcelFile(ds);*/
            /*}
            else
            { }*/
            //TestMainWindow main = new TestMainWindow();
            //main.ShowDialog();

            Autorization auto = new Autorization("OLM05367", "456");
            auto.Show();
            auto.Close();
            TestMainWindow test = new TestMainWindow();
            test.ShowDialog();
        }
    }

    public class TestSourceString
    {
        static List<Object> _collection;
        public static List<object> Collection
        {
            get
            {
                if (_collection == null)
                    GenCollection(1000000);
                return _collection;
            }
        }

        private static void GenCollection(int count)
        {
            _collection = new List<object>();
            for (int i = 0; i < count; ++i)
            {
                _collection.Add(new Emp() { EmpLastName = "Aaabef" });
            }
        }
    }

    public class Emp
    {
        public string EmpLastName
        {
            get;set;
        }

        public string EmpFirstName
        {
            get;set;
        }

        public string EmpMiddleName
        {
            get;set;
        }
        public string FullFIO
        {
            get
            {
                return EmpLastName+ ' ' + EmpFirstName + ' ' + EmpMiddleName;
            }

        }
    }
}
