using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestingProject;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProFrame;
using System.Data;

namespace TestingProject.Tests
{
    [TestClass()]
    public class UniDbTests
    {
        [TestMethod()]
        public void CheckConnectionTest()
        {
            ProviderSetting.CurrentDBProvider = DbProviderType.OracleOdpNetManaged;
            UniDbConnection connect = new UniDbConnection("knvtest", "3");
            connect.Open();
            Assert.AreEqual(connect.State, ConnectionState.Open);

            UniDbCommand cmd = new UniDbCommand("select * from apstaff.emp where emp_birth_date=:p_date and per_num=:p_per_num and perco_sync_id=:p_perco_sync_id  order by per_num", connect);
            cmd.Parameters.Add("p_per_num", UniDbType.String, "14534");
            cmd.Parameters.Add("p_date", UniDbType.DateTime, new DateTime(1989, 7, 8));
            cmd.Parameters.Add("p_perco_sync_id", UniDbType.Decimal, 9479);
            UniDbDataReader dr = cmd.ExecuteReader();
            List<string> ls = new List<string>();
            while (dr.Read())
            {
                ls.Add(dr["PER_NUM"].ToString());
            }
            Assert.AreEqual(ls.Count, 1);
            Assert.AreEqual(ls[ls.Count - 1], "14534");
            connect.Close();
            connect.Dispose();
        }

        [TestMethod()]
        public void CheckUniAdapterTest()
        {
            ProviderSetting.CurrentDBProvider = DbProviderType.OracleOdpNetManaged;
            UniDbConnection connect = new UniDbConnection("knvtest", "3");
            connect.Open();
            Assert.AreEqual(connect.State, ConnectionState.Open);

            UniDbAdapter a = new UniDbAdapter("select * from test_apstaff.temp_rep where per_num=:p_per_num", connect);
            a.TableMappings.Add("Table", "temp_rep");
            a.SelectCommand.Parameters.Add("p_per_num", UniDbType.String, "14534");
            DataSet ds = new DataSet();

            a.Fill(ds);
            a.Dispose();

            UniDbAdapter a1 = new UniDbAdapter("declare begin open :c for select * from test_apstaff.temp_rep where per_num=:p_per_num; end;", connect);
            a1.TableMappings.Add("Table", "temp_rep1");
            a1.SelectCommand.Parameters.Add("p_per_num", UniDbType.String, "14534");
            a1.SelectCommand.Parameters.Add("c", UniDbType.RefCursor);

            a1.Fill(ds);

            Assert.AreEqual(ds.Tables["temp_rep1"].Rows.Count, 2);

            UniDbAdapter a2 = new UniDbAdapter();
            UniDbCommand cm = new UniDbCommand("declare begin open :c for select * from test_apstaff.temp_rep where per_num=:p_per_num; end;", connect);
            a2.SelectCommand = cm;
            a2.TableMappings.Add("Table", "temp_rep2");
            a2.SelectCommand.Parameters.Add("p_per_num", UniDbType.String);
            a2.SelectCommand.Parameters.Add("c", UniDbType.RefCursor);
            a2.SelectCommand.SetParameters(new TestFilter());

            DataTable t = new DataTable();
            a2.Fill(ds);
            Assert.AreEqual(ds.Tables["temp_rep2"].Rows.Count, 2);
            Assert.IsTrue(ds.Tables["temp_rep2"].Rows.Cast<DataRow>().All(r => r["PER_NUM"].ToString() == "12714"));

            a2.SelectCommand.Parameters["p_per_num"].Value = "13772";
            a2.Fill(t);
            Assert.AreEqual(t.Rows.Count, 2);
            Assert.IsTrue(t.Rows.Cast<DataRow>().All(r => r["PER_NUM"].ToString() == "13772"));



            connect.Close();
            connect.Dispose();
        }

        [TestMethod]
        public void TestTableModel_TestAdd()
        {
            ProviderSetting.CurrentDBProvider = DbProviderType.OracleOdpNetManaged;
            UniDbConnection.OpenCurrentConnection("knvtest", "3");
            Assert.AreEqual(UniDbConnection.Current.State, ConnectionState.Open);
            DataSet ds = new DataSet();
            TestTableModel v = UniDbModel.GetOrCreate<TestTableModel>(null, ds);
            v.SetProperty<string>("NAME_T", "Новое название 1");
            v.SetProperty<string>("CODE_T", "9999Ж");
            v.SetProperty<decimal>("VALUE_T", 1042.21);
            v.SetProperty<decimal>("PER_NUM", "14534");
            v.Save();

            TestTableModel v1 = UniDbModel.GetOrCreate<TestTableModel>(v.GetProperty<decimal?>("TEST_TABLE_ID"));
            Assert.AreEqual(v.GetProperty<string>("NAME_T"), v1.GetProperty<string>("NAME_T"));
            Assert.AreEqual(v.GetProperty<decimal?>("VALUE_T"), v1.GetProperty<decimal?>("VALUE_T"));
            Assert.AreEqual(v.GetProperty<string>("PER_NUM"), v1.GetProperty<string>("PER_NUM"));
        }

        [TestMethod]
        public void TestTableModel_TestUpdate()
        {
            ProviderSetting.CurrentDBProvider = DbProviderType.OracleOdpNetManaged;
            UniDbConnection.OpenCurrentConnection("knvtest", "3");
            Assert.AreEqual(UniDbConnection.Current.State, ConnectionState.Open);
            DataSet ds = new DataSet();
            TestTableModel v = UniDbModel.GetOrCreate<TestTableModel>(2, ds);
            v.SetProperty<decimal>("VALUE_T", 2054.21);
            v.SetProperty<string>("PER_NUM", "12714");
            v.Save();

            TestTableModel v1 = UniDbModel.GetOrCreate<TestTableModel>(v.GetProperty<decimal?>("TEST_TABLE_ID"));
            Assert.AreEqual(v.GetProperty<string>("NAME_T"), v1.GetProperty<string>("NAME_T"));
            Assert.AreEqual<decimal?>(v.GetProperty<decimal?>("VALUE_T"), 2054.21m);
            Assert.AreEqual<string>(v.GetProperty<string>("PER_NUM"), "12714");
        }

        [TestMethod]
        public void TestTable_Delete()
        {
            ProviderSetting.CurrentDBProvider = DbProviderType.OracleOdpNetManaged;
            UniDbConnection.OpenCurrentConnection("knvtest", "3");
            Assert.AreEqual(UniDbConnection.Current.State, ConnectionState.Open);
            DataSet ds = new DataSet();
            TestTableModel v = UniDbModel.GetOrCreate<TestTableModel>(3, ds);
            v.SetProperty<decimal>("VALUE_T", 2054.21);
            v.Delete();

            v = UniDbModel.GetOrCreate<TestTableModel>(2, ds);
            Assert.AreEqual(v.GetProperty<string>("NAME_T"), null);
            Assert.AreEqual<decimal?>(v.GetProperty<decimal?>("VALUE_T"), null);
        }

        
        public class TestFilter
        {
            [UniParameterMapping(ParameterName = "p_per_num")]
            public string PerNum
            {
                get
                {
                    return "12714";
                }
            }
        }
    }

    [Table(Name = "TEST_TABLE", SchemaName = "TEST_APSTAFF")]
    public class TestTableModel : UniDbModel
    {
        public TestTableModel()
        {
        }

        [Column(Name = "TEST_TABLE_ID", IsPrimaryKey =true)]
        public decimal? TestTableID
        {
            get
            {
                return GetProperty<decimal?>(()=>TestTableID);
            }
            set
            {
                SetProperty<decimal?>(() => TestTableID, value);
            }
        }

        [Column(Name = "PER_NUM")]
        public string PerNum
        {
            get
            {
                return GetProperty<string>("PER_NUM");
            }
            set
            {
                SetProperty<string>("PER_NUM", value);
            }
        }
    }

}