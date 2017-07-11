using Microsoft.Reporting.WinForms;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Forms;
using SPrint = System.Drawing.Printing;

namespace ProFrame
{
    /// <summary>
    /// Логика взаимодействия для ViewReportWindow.xaml
    /// </summary>
    public partial class ViewReportWindow : Window
    {
        public ViewReportWindow(string[] enabledExtensions = null)
        {
            this.EnabledExportExtension = enabledExtensions ?? new string[] { "WORD", "PDF", "EXCEL" };
            InitializeComponent();
            this.repViewer.ReportExport += new ExportEventHandler(repViewer_ReportExport);
        }

        void repViewer_ReportExport(object sender, ReportExportEventArgs e)
        {
            e.Cancel = true;
        }

        public string[] EnabledExportExtension
        {
            get;
            set;
        }
       
        public static void ShowReport(DependencyObject sender, string Title, string path, DataTable table, IEnumerable<ReportParameter> r, SPrint.Duplex duplex = SPrint.Duplex.Simplex,
            bool PreviewPrint = true)
        {

            ShowReport(sender, Title, path, null, new DataTable[] { table }.AsEnumerable(), r, duplex, PreviewPrint);
        }

        public static void ShowReport(DependencyObject sender, string Title, string path, DataTable table, IEnumerable<ReportParameter> r, string[] EnabledExport)
        {

            ShowReport(sender, Title, path, null, new DataTable[] { table }.AsEnumerable(), r, SPrint.Duplex.Default, true, null, EnabledExport);
        }

        public static void ShowReport(DependencyObject sender, string Title, string path, IEnumerable<DataTable> tables, IEnumerable<ReportParameter> r, SPrint.Duplex duplex = SPrint.Duplex.Simplex,
            bool PreviewPrint = true)
        {

            ShowReport(sender, Title, path, null, tables, r, duplex, PreviewPrint);
        }

        /// <summary>
        /// Формирует отчет согласно заданным критериям
        /// </summary>
        /// <param name="sender"> Владелец окна отчета</param>
        /// <param name="Title">Надпись окна отчета</param>
        /// <param name="path">Путь к основном отчету</param>
        /// <param name="subreports">Подотчеты</param>
        /// <param name="tables">Таблицы данных отчета</param>
        /// <param name="r">Параметры отчета</param>
        /// <param name="duplex">Режим печати отчета</param>
        /// <param name="PreviewPrint">Показывать ли предварительный просмотр</param>
        /// <param name="EnabledExport">Доступные параметры для экспорта</param>
        public static void ShowReport(DependencyObject sender, string Title, string path, IEnumerable<SubReport> subreports, IEnumerable<DataTable> tables, IEnumerable<ReportParameter> r, SPrint.Duplex duplex = SPrint.Duplex.Simplex,
            bool PreviewPrint = true, DateTime? YearVersion = null, string[] EnabledExport = null)
        {
            if (!ExistsReport(path, YearVersion))
            {
                System.Windows.MessageBox.Show(Window.GetWindow(sender), "Ошибка формирования. Макет отчета не найден в заданной директории", "Зарплата предприятия");
                return;
            }
            ViewReportWindow f = new ViewReportWindow(EnabledExport);
            f.repViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
            f.repViewer.LocalReport.LoadReportDefinition(File.OpenRead(string.Format(@"{0}\Reports\{1}{2}", AppLocalPath.CurrentAppPath, YearVersion.HasValue ? YearVersion.Value.Year.ToString() + @"\" : string.Empty, path)));

            if (subreports != null)
            {
                CurrentSubReports = subreports;
                foreach (SubReport st in subreports)
                {
                    f.repViewer.LocalReport.LoadSubreportDefinition(st.ReportName, File.OpenRead(string.Format(@"{0}\Reports\{1}{2}", AppLocalPath.CurrentAppPath, YearVersion.HasValue ? YearVersion.Value.Year.ToString() + @"\" : string.Empty, st.ReportPath)));
                }
            }
            else
                CurrentSubReports = null;
            f.repViewer.PrinterSettings.Duplex = duplex;
            f.repViewer.ZoomPercent = 110;
            f.repViewer.LocalReport.DataSources.Clear();
            if (tables != null)
            {
                int i = 1;
                foreach (DataTable t in tables)
                    f.repViewer.LocalReport.DataSources.Add(new ReportDataSource(string.Format("DataSet{0}", i++), t));
            }
            if (r != null)
                f.repViewer.LocalReport.SetParameters(r);
            f.repViewer.RefreshReport();
            if (PreviewPrint) f.repViewer.SetDisplayMode(DisplayMode.PrintLayout);
            if (sender != null)
                f.Owner = Window.GetWindow(sender);
            f.Title += "    " + Title;
            f.Show();
        }

        public static Window ShowReport(string Title, string path, IEnumerable<DataTable> tables, IEnumerable<ReportParameter> r)
        {
            ViewReportWindow f = new ViewReportWindow();
            f.repViewer.LocalReport.SubreportProcessing += new SubreportProcessingEventHandler(LocalReport_SubreportProcessing);
            f.repViewer.LocalReport.LoadReportDefinition(File.OpenRead(string.Format(@"{0}\Reports\{1}{2}", AppLocalPath.CurrentAppPath, string.Empty, path)));

            /*if (subreports != null)
            {
                CurrentSubReports = subreports;
                foreach (SubReport st in subreports)
                {
                    f.repViewer.LocalReport.LoadSubreportDefinition(st.ReportName, File.OpenRead(string.Format(@"{0}\Reports\{1}{2}", Connect.CurrentAppPath, string.Empty, st.ReportPath)));
                }
            }
            else*/
            CurrentSubReports = null;
            f.repViewer.ZoomPercent = 100;
            f.repViewer.LocalReport.DataSources.Clear();
            if (tables != null)
            {
                int i = 1;
                foreach (DataTable t in tables)
                    f.repViewer.LocalReport.DataSources.Add(new ReportDataSource(string.Format("DataSet{0}", i++), t));
            }
            if (r != null)
                f.repViewer.LocalReport.SetParameters(r);
            f.repViewer.RefreshReport();
            f.repViewer.SetDisplayMode(DisplayMode.PrintLayout);
            f.Title += "    " + Title;
            f.Show();
            return f;
        }

        /// <summary>
        /// Проверяет, есть ли заданный отчет в папке отчетов
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ExistsReport(string path, DateTime? YearVersion)
        {
            if (YearVersion == null)
                return File.Exists(AppLocalPath.CurrentAppPath + @"\Reports\" + path);// если версия без года, то берем версию без года, иначе в папке с годом
            else
                return File.Exists(AppLocalPath.CurrentAppPath + @"\Reports\" + YearVersion.Value.Year.ToString() + @"\" + path);
        }

        /// <summary>
        /// Проверяет, есть ли заданный отчет в папке отчетов
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool ExistsReport(string path)
        {
            return ExistsReport(path, null);
        }

        private static IEnumerable<SubReport> CurrentSubReports
        {
            get;
            set;
        }

        static void LocalReport_SubreportProcessing(object sender, SubreportProcessingEventArgs e)
        {
            if (CurrentSubReports != null)
            {
                e.DataSources.Clear();
                e.DataSources.Add(new ReportDataSource("DataSet1", CurrentSubReports.Where(t => t.ReportName == e.ReportPath).Select(t => t.DataSource).FirstOrDefault()));
            }
        }

        public static void ShowReport(DependencyObject sender, string Title, string path, IEnumerable<SubReport> subreports, DataTable table, IEnumerable<ReportParameter> r)
        {
            ShowReport(sender, Title, path, subreports, new DataTable[] { table }, r);
        }

        /// <summary>
        /// Выгружает рдлс репорт в эксель используя метод Render
        /// </summary>
        /// <param name="sender">окно владелец</param>
        /// <param name="path">путь к отчету RDLC</param>
        /// <param name="table">таблицы для отчета</param>
        /// <param name="r">список параметров для отчета</param>
        public static void RenderToExcel(DependencyObject sender, string path, DataTable table, IEnumerable<ReportParameter> r)
        {
            RenderToExcel(sender, path, "", string.Empty, new DataTable[] { table }, r);
        }

        /// <summary>
        /// Выгружает рдлс репорт в эксель используя метод Render
        /// </summary>
        /// <param name="sender">окно владелец</param>
        /// <param name="path">путь к отчету RDLC</param>
        /// <param name="SaveFileName">Имя для очета по умолчанию</param>
        /// <param name="table">таблицы для отчета</param>
        /// <param name="r">список параметров для отчета</param>
        public static void RenderToExcel(DependencyObject sender, string path, string SaveFileName, DataTable table, IEnumerable<ReportParameter> r)
        {
            RenderToExcel(sender, path, SaveFileName, string.Empty, new DataTable[] { table }, r);
        }

        /// <summary>
        /// Выгружает рдлс репорт в эксель используя метод Render
        /// </summary>
        /// <param name="sender">окно владелец</param>
        /// <param name="path">путь к отчету RDLC</param>
        /// <param name="SaveFileName">Имя для очета по умолчанию</param>
        /// <param name="table">таблицы для отчета</param>
        /// <param name="r">список параметров для отчета</param>
        public static void RenderToExcel(DependencyObject sender, string path, string SaveFileName, string initDirectory, DataTable table, IEnumerable<ReportParameter> r)
        {
            RenderToExcel(sender, path, SaveFileName, initDirectory, new DataTable[] { table }, r);
        }

        /// <summary>
        /// Выгружает рдлс репорт в эксель используя метод Render
        /// </summary>
        /// <param name="sender">окно владелец</param>
        /// <param name="path">путь к отчету RDLC</param>
        /// <param name="SaveFileName">имя для сохранения по умолчанию</param>
        /// <param name="tables"> таблицы для отчета</param>
        /// <param name="r">список параметров для отчета</param>
        public static void RenderToExcel(DependencyObject sender, string path, string SaveFileName, string initDirectory, DataTable[] tables, IEnumerable<ReportParameter> r)
        {
            AbortableBackgroundWorker.RunAsyncWithWaitDialog(sender, "Формирование отчета",
            (bwk, e) =>
            {
                LocalReport f = new LocalReport();
                f.LoadReportDefinition(File.OpenRead(AppLocalPath.CurrentAppPath + @"\Reports\" + path));
                f.DataSources.Clear();
                tables = e.Argument as DataTable[];
                if (tables != null)
                    for (int i = 1; i <= tables.Length; ++i)
                        f.DataSources.Add(new ReportDataSource(string.Format("DataSet{0}", i), tables[i - 1]));
                if (r != null)
                    f.SetParameters(r);
                e.Result = f.Render("Excel");
            },
            tables, null,
            (bwk, e) =>
            {
                if (e.Cancelled) return;
                else
                    if (e.Error != null)
                    System.Windows.MessageBox.Show(e.Error.Message, "Ошибка формирования отчета");
                else
                {
                    try
                    {

                        SaveFileDialog sf = new SaveFileDialog();
                        sf.FileName = SaveFileName;
                        sf.Filter = "Файлы Excel|*.xls";
                        sf.InitialDirectory = initDirectory;
                        if (sf.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                        {
                            File.WriteAllBytes(sf.FileName, (byte[])e.Result);
                            System.Diagnostics.Process.Start(sf.FileName);
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Windows.MessageBox.Show("Ошибка формирования", ex.Message);
                    }
                }
            });
        }

    }

    public class SubReport
    {
        public SubReport(string reportName, string reportPath, DataTable dataSource)
        {
            ReportName = reportName;
            ReportPath = reportPath;
            DataSource = dataSource;
        }
        public string ReportName
        {
            get;
            set;
        }
        public string ReportPath
        {
            get;
            set;
        }

        public DataTable DataSource
        {
            get;
            set;
        }
    }
}
