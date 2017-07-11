using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WExcel = Microsoft.Office.Interop.Excel;
using System.Threading;
namespace ProFrame
{
    public class Excel
    {
        #region Функции преобразования  декартовых координат в экселевские  и обратно

        /// <summary>
        /// Перевод номера столбца в символьный эквивалент.
        /// </summary>
        public static string ParseColNum(int ColNum)
        {
            StringBuilder sb = new StringBuilder();
            if (ColNum <= 90) return Convert.ToChar(ColNum).ToString();
            sb.Append(Convert.ToChar(64 + (ColNum - 65) / 26));
            sb.Append(Convert.ToChar(65 + (ColNum - 65) % 26));
            return sb.ToString();
        }

        /// <summary>
        /// Выделяет из строки имя столбца и преобразует его в числовой вид
        /// </summary>
        /// <param name="st">Координат ячейки эксель в виде 'A10'</param>
        /// <returns>Номер столбца</returns>
        public static int ExNameToColNum(string st)
        {
            int i = 0, p = 1;
            st = st.Substring(0, st.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }));
            for (int j = st.Length - 1; j > -1; --j)
            {
                i += p * (st[j] - 64);
                p *= 26;
            }
            return i;
        }
        public static int ExNameToRowNum(string st)
        {
            return Convert.ToInt32(st.Substring(st.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' })));
        }
        /// <summary>
        /// Преобразует номер столбца в формат эксель(27->'AA')
        /// </summary>
        /// <param name="ColNum">Номер столбца(начиная с 1)</param>
        /// <returns>возвращает строку в формате столбца Excel</returns>
        public static string ColNumToEx(int ColNum)
        {
            string st = "";
            while (ColNum > 0)
            {
                --ColNum;
                st = Convert.ToChar(ColNum % 26 + 65) + st;
                ColNum = ColNum / 26;
            }
            return st;
        }
        /// <summary>
        /// Смещает адрес ячейки в формате Excel на К строк
        /// </summary>
        /// <param name="Cell">Адрес в формате Excel</param>
        /// <param name="k">требуемое смещение</param>
        /// <returns></returns>
        public static string AddRows(string Cell, int k)
        {
            string s = "";
            for (int i = 0; i < Cell.Length && Cell[i] > '9'; i++)
                s += Cell[i];
            s += (Convert.ToInt32(Cell.Substring(s.Length, Cell.Length - s.Length)) + k).ToString();
            return s;
        }
        public static string AddCols(string Cell, int k)
        {
            return ColNumToEx(ExNameToColNum(Cell) + k) + Cell.Substring(Cell.IndexOfAny(new char[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' }));
        }

        #endregion

        public static void PrintWithBorder(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters)
        {
            PrintWithBorder(false, nameOfTemplate, startExcel, tables, excelParameters, null, false);
        }

        public static void PrintWithBorder(bool ShowEditor, string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters)
        {
            PrintWithBorder(ShowEditor, nameOfTemplate, startExcel, tables, excelParameters, null, false);
        }

        public static void PrintWithBorder(bool ShowEditor, string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters, TotalRowsStyle[] totalRowStyles)
        {
            PrintWithBorder(ShowEditor, nameOfTemplate, startExcel, tables, excelParameters, totalRowStyles, false);
        }
        /// <summary>
        /// Печать с рамкой вокруг данных
        /// </summary>
        /// <param name="ShowEditor">Показывать ли отчет в режиме редактирования</param>
        /// <param name="nameOfTemplate">Шаблон</param>
        /// <param name="startExcel">Стартовый угол данных</param>
        /// <param name="tables">таблицы данных</param>
        /// <param name="excelParameters">Параметры</param>
        public static void PrintWithBorderMergedTable(bool ShowEditor, string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters, TotalRowsStyle[] totalRowStyles)
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {

                //Создание книги Excel
                WExcel._Worksheet m_Sheet;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                m_ExcelApp.Workbooks.Open(Application.StartupPath + @"\Reports\" + nameOfTemplate, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;
                
                //Заполняем отдельные параметры
                if (excelParameters != null)
                    foreach (ExcelParameter parameter in excelParameters)
                    {
                        WExcel.Range r2 = m_Sheet.get_Range(parameter.NameOfExcel, parameter.NameOfEndMergeExcel);
                        if (parameter.NameOfEndMergeExcel != parameter.NameOfExcel)
                        {
                            r2.Merge(false);
                            r2.HorizontalAlignment = parameter.TextAlign;
                        }
                        if (parameter.Borders != null)
                            foreach (WExcel.XlBordersIndex border in parameter.Borders)
                            {
                                r2.Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                            }
                        r2.Value2 = parameter.Value;
                        
                    }
                //Проверяем - есть ли столбцы означающие итоги.
                Dictionary<string,Dictionary<int,string> > hs = new Dictionary<string,Dictionary<int,string>>();
                if (totalRowStyles!=null)
                {
                    foreach (TotalRowsStyle c in totalRowStyles)
                    {
                        WExcel.Style my_style = m_ExcelApp.ActiveWorkbook.Styles.Add("style_"+c.TotalFlagColumnName+c.TotalFlagValue.GetHashCode().ToString(),Type.Missing);
                        my_style.Interior.Color= System.Drawing.ColorTranslator.ToOle(c.BackColor);
                        if (c.ForeColor!=Color.Empty) my_style.Font.Color= ColorTranslator.ToOle(c.ForeColor);
                        if (!hs.ContainsKey(c.TotalFlagColumnName.ToUpper()))
                            hs.Add(c.TotalFlagColumnName.ToUpper(),new Dictionary<int,string>());
                        hs[c.TotalFlagColumnName.ToUpper()].Add(c.TotalFlagValue.GetHashCode(),my_style.Name);   
                    }

                }

                //Заполняем массив данных
                //Перебираем все таблицы
                int sumCountRow = 0, sum = 0, max = 1, RowInStr;
                for (int i = 0; i < tables.Count(); i++)
                {
                    sum += tables[i].Rows.Count;
                    max = Math.Max(tables[i].Columns.Count, max);
                }
                sum = Math.Max(sum, 1);
                max=Math.Max(1,max-hs.Count);
                WExcel.Range range = m_Sheet.Range[startExcel];
                string st = startExcel;
                int[] ar_jump = new int[max];
                for (int i = 0; i < max; ++i)
                {
                    ar_jump[i] = ((bool)range.MergeCells ? range.MergeArea.Columns.Count : 1);
                    range=m_Sheet.Range[(st=Excel.AddCols(st,((bool)range.MergeCells?range.MergeArea.Columns.Count:1)))];
                }
                max=0;
                foreach (int i in ar_jump)
                    max+=i;
                max=Math.Max(max-hs.Count,1);
                //ставим границы в экселе
                WExcel.Range r = m_Sheet.get_Range(startExcel, Excel.AddCols(Excel.AddRows(startExcel, sum - 1), max-1));
                r.BorderAround(WExcel.XlLineStyle.xlContinuous, WExcel.XlBorderWeight.xlThin, WExcel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
                r.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                r.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                object[,] str = new object[sum, max];
                
                for (int i = 0; i < tables.Count(); i++)
                {
                    //Перебираем все колонки
                    int j=0;
                    int j1 = -1;
                    for (int column = 0; column < tables[i].Columns.Count; column++)
                    {
                        RowInStr = sumCountRow;
                        if (hs.ContainsKey(tables[i].Columns[column].ColumnName.ToUpper()))
                        {
                            Dictionary<int,string> t = hs[tables[i].Columns[column].ColumnName.ToUpper()];
                            for (int k=0;k<tables[i].Rows.Count;++k,RowInStr++)
                                if (t.ContainsKey(tables[i].Rows[k][column].GetHashCode()))
                                {
                                    WExcel.Range r_row = m_Sheet.get_Range(Excel.AddRows(startExcel,RowInStr),Excel.AddCols(Excel.AddRows(startExcel,RowInStr),max-1));
                                    r_row.Style = t[tables[i].Rows[k][column].GetHashCode()] ;
                                    r_row.HorizontalAlignment = WExcel.XlHAlign.xlHAlignCenter;
                                    r_row.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                                    r_row.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                                }
                        }
                        else 
                        {
                            ++j1;
                            if (tables[i].Columns[column].DataType == typeof(DateTime))
                                for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                    str[RowInStr, j] =(tables[i].Rows[row][column]==DBNull.Value?"":((DateTime)tables[i].Rows[row][column]).ToString("dd-MM-yyyy"));
                            else
                                for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                    str[RowInStr, j] = tables[i].Rows[row][column];
                            j += ar_jump[j1];

                        }
                    }
                    sumCountRow += tables[i].Rows.Count;
                }
                r.set_Value(Type.Missing, str);
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                if (!ShowEditor)
                {
                    m_Sheet.PrintPreview(true);
                    m_ExcelApp.Quit();
                }
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public static void PrintWithBorder(bool ShowEditor, string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters, TotalRowsStyle[] totalRowStyles,
            bool ShowLongDate)
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {

                //Создание книги Excel
                WExcel._Worksheet m_Sheet;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                m_ExcelApp.Workbooks.Open(Application.StartupPath + @"\Reports\" + nameOfTemplate, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;

                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationManual;
                m_ExcelApp.ScreenUpdating = false;
                //Заполняем отдельные параметры
                if (excelParameters != null)
                    foreach (ExcelParameter parameter in excelParameters)
                    {
                        WExcel.Range r2 = m_Sheet.get_Range(parameter.NameOfExcel, parameter.NameOfEndMergeExcel);
                        if (parameter.NameOfEndMergeExcel != parameter.NameOfExcel)
                        {
                            r2.Merge(false);
                            r2.HorizontalAlignment = parameter.TextAlign;
                        }
                        if (parameter.Borders != null)
                            foreach (WExcel.XlBordersIndex border in parameter.Borders)
                            {
                                r2.Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                            }
                        r2.Value2 = parameter.Value;

                    }
                //Проверяем - есть ли столбцы означающие итоги.
                Dictionary<string, Dictionary<int, string>> hs = new Dictionary<string, Dictionary<int, string>>();
                if (totalRowStyles != null)
                {
                    foreach (TotalRowsStyle c in totalRowStyles)
                    {
                        WExcel.Style my_style = m_ExcelApp.ActiveWorkbook.Styles.Add("style_" + c.TotalFlagColumnName + c.TotalFlagValue.GetHashCode().ToString(), Type.Missing);
                        my_style.Interior.Color = System.Drawing.ColorTranslator.ToOle(c.BackColor);
                        if (c.ForeColor != Color.Empty) my_style.Font.Color = ColorTranslator.ToOle(c.ForeColor);
                        if (!hs.ContainsKey(c.TotalFlagColumnName.ToUpper()))
                            hs.Add(c.TotalFlagColumnName.ToUpper(), new Dictionary<int, string>());
                        hs[c.TotalFlagColumnName.ToUpper()].Add(c.TotalFlagValue.GetHashCode(), my_style.Name);
                    }

                }

                //Заполняем массив данных
                //Перебираем все таблицы
                int sumCountRow = 0, sum = 0, max = 1, RowInStr;
                for (int i = 0; i < tables.Count(); i++)
                {
                    sum += tables[i].Rows.Count;
                    max = Math.Max(tables[i].Columns.Count, max);
                }
                sum = Math.Max(sum, 1);
                max = Math.Max(1, max - hs.Count);
                //ставим границы в экселе
                WExcel.Range r = m_Sheet.get_Range(startExcel, Excel.AddCols(Excel.AddRows(startExcel, sum - 1), max - 1));
                r.BorderAround(WExcel.XlLineStyle.xlContinuous, WExcel.XlBorderWeight.xlThin, WExcel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
                r.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                r.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                object[,] str = new object[sum, max];

                for (int i = 0; i < tables.Count(); i++)
                {
                    //Перебираем все колонки
                    int j = -1;
                    for (int column = 0; column < tables[i].Columns.Count; column++)
                    {
                        RowInStr = sumCountRow;
                        if (hs.ContainsKey(tables[i].Columns[column].ColumnName.ToUpper()))
                        {
                            Dictionary<int, string> t = hs[tables[i].Columns[column].ColumnName.ToUpper()];
                            for (int k = 0; k < tables[i].Rows.Count; ++k, RowInStr++)
                                if (t.ContainsKey(tables[i].Rows[k][column].GetHashCode()))
                                {
                                    WExcel.Range r_row = m_Sheet.get_Range(Excel.AddRows(startExcel, RowInStr), Excel.AddCols(Excel.AddRows(startExcel, RowInStr), max - 1));
                                    r_row.Style = t[tables[i].Rows[k][column].GetHashCode()];
                                    r_row.HorizontalAlignment = WExcel.XlHAlign.xlHAlignCenter;
                                    r_row.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                                    r_row.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                                }
                        }
                        else
                        {
                            ++j;
                            if (tables[i].Columns[column].DataType == typeof(DateTime))
                                for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                    str[RowInStr, j] = (tables[i].Rows[row][column] == DBNull.Value ? "" :
                                        ShowLongDate ? ((DateTime)tables[i].Rows[row][column]).ToString() : ((DateTime)tables[i].Rows[row][column]).ToShortDateString());
                            else
                                for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                    str[RowInStr, j] = tables[i].Rows[row][column];
                        }
                    }
                    sumCountRow += tables[i].Rows.Count;
                }
                r.set_Value(Type.Missing, str);
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.ScreenUpdating = true;
                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationAutomatic;
                m_ExcelApp.Visible = true;
                if (!ShowEditor)
                {
                    m_Sheet.PrintPreview(true);
                    m_ExcelApp.Quit();
                }
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public static void PrintWithBorder(bool ShowEditor, string nameOfTemplate, ExcelPrintTable[] tables, ExcelParameter[] excelParameters =null , TotalRowsStyle[] totalRowStyles =null, string DateFormat="dd.MM.yyyy")
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {

                //Создание книги Excel
                WExcel._Worksheet m_Sheet;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                m_ExcelApp.Workbooks.Open(Application.StartupPath + @"\Reports\" + nameOfTemplate, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;

                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationManual;
                m_ExcelApp.ScreenUpdating = false;
                //Заполняем отдельные параметры
                if (excelParameters != null)
                    foreach (ExcelParameter parameter in excelParameters)
                    {
                        WExcel.Range r2 = m_Sheet.get_Range(parameter.NameOfExcel, parameter.NameOfEndMergeExcel);
                        if (parameter.NameOfEndMergeExcel != parameter.NameOfExcel)
                        {
                            r2.Merge(false);
                            r2.HorizontalAlignment = parameter.TextAlign;
                        }
                        if (parameter.Borders != null)
                            foreach (WExcel.XlBordersIndex border in parameter.Borders)
                            {
                                r2.Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                            }
                        r2.Value2 = parameter.Value;

                    }
                //Проверяем - есть ли столбцы означающие итоги.
                Dictionary<string, Dictionary<int, string>> hs = new Dictionary<string, Dictionary<int, string>>();
                if (totalRowStyles != null)
                {
                    foreach (TotalRowsStyle c in totalRowStyles)
                    {
                        WExcel.Style my_style = m_ExcelApp.ActiveWorkbook.Styles.Add("style_" + c.TotalFlagColumnName + c.TotalFlagValue.GetHashCode().ToString(), Type.Missing);
                        my_style.Interior.Color = System.Drawing.ColorTranslator.ToOle(c.BackColor);
                        if (c.ForeColor != Color.Empty) my_style.Font.Color = ColorTranslator.ToOle(c.ForeColor);
                        if (!hs.ContainsKey(c.TotalFlagColumnName.ToUpper()))
                            hs.Add(c.TotalFlagColumnName.ToUpper(), new Dictionary<int, string>());
                        hs[c.TotalFlagColumnName.ToUpper()].Add(c.TotalFlagValue.GetHashCode(), my_style.Name);
                    }

                }                

                for (int i = 0; i < tables.Count(); i++)
                {
                    //Перебираем все колонки
                    int j = -1;
                    int p = 0;
                    foreach (DataColumn t in tables[i].Table.Columns)
                        if (hs.ContainsKey(t.ColumnName.ToUpper()))
                            p++;
                    int max = Math.Max(1, tables[i].Table.Columns.Count - p),
                        sum=Math.Max(1,tables[i].Table.Rows.Count+(tables[i].WithColumnHeader?1:0));
                    //Заполняем массив данных
                    //Перебираем все таблицы
                    //ставим границы в экселе
                    WExcel.Range r = m_Sheet.get_Range(tables[i].StartPrintCell, Excel.AddCols(Excel.AddRows(tables[i].StartPrintCell, sum - 1), max - 1));
                    r.BorderAround(WExcel.XlLineStyle.xlContinuous, WExcel.XlBorderWeight.xlThin, WExcel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
                    r.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                    r.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                    int StartRow=(tables[i].WithColumnHeader?1:0);
                    object[,] str = new object[sum, max];
                    if (tables[i].WithColumnHeader)
                    {
                        int d = 0;
                        for (int k = 0; k < tables[i].Table.Columns.Count; ++k)
                            if (!hs.ContainsKey(tables[i].Table.Columns[k].ColumnName.ToUpper()))
                            {
                                str[0, d] = tables[i].Table.Columns[k].ColumnName.ToString();
                                ++d;
                            }
                    }
                    for (int column = 0; column < tables[i].Table.Columns.Count; column++)
                    {
                        if (hs.ContainsKey(tables[i].Table.Columns[column].ColumnName.ToUpper()))
                        {
                            Dictionary<int, string> t = hs[tables[i].Table.Columns[column].ColumnName.ToUpper()];
                            for (int k = 0; k < tables[i].Table.Rows.Count; ++k)
                                if (t.ContainsKey(tables[i].Table.Rows[k][column].GetHashCode()))
                                {
                                    WExcel.Range r_row = m_Sheet.get_Range(Excel.AddRows(tables[i].StartPrintCell, k+StartRow), Excel.AddCols(Excel.AddRows(tables[i].StartPrintCell, k+StartRow), max - 1));
                                    r_row.Style = t[tables[i].Table.Rows[k][column].GetHashCode()];
                                    r_row.HorizontalAlignment = WExcel.XlHAlign.xlHAlignCenter;
                                    r_row.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                                    r_row.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                                }
                        }
                        else
                        {
                            ++j;
                            if (tables[i].Table.Columns[column].DataType == typeof(DateTime))
                                for (int k = 0; k < tables[i].Table.Rows.Count; k++)
                                    str[k+StartRow, j] = string.Format("{0:"+DateFormat+"}",tables[i].Table.Rows[k][column]);
                            else
                                for (int k = 0; k < tables[i].Table.Rows.Count; k++)
                                    str[k+StartRow, j] = tables[i].Table.Rows[k][column];
                        }
                    }
                    r.set_Value(Type.Missing, str);
                }
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.ScreenUpdating = true;
                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationAutomatic;
                m_ExcelApp.Visible = true;
                if (!ShowEditor)
                {
                    m_Sheet.PrintPreview(true);
                    m_ExcelApp.Quit();
                }
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public static void Print(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables)
        {
            Print(nameOfTemplate, startExcel, tables, null, null, true);
        }
        public static void Print(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters)
        {
            Print(nameOfTemplate, startExcel, tables, excelParameters, null, true);
        }
        public static void Print(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters, bool flagQuit)
        {
            Print(nameOfTemplate, startExcel, tables, excelParameters, null, flagQuit);
        }
        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="startExcel">Имя ячейки (пример:В8)</param>
        /// <param name="tables">Массивы данных в виде DataTable</param>
        /// <param name="excelParameters">Дополнительные параметры,
        /// с помощью этих параметров задаются значения отдельных ячеек Excel</param>
        public static void Print(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, ExcelParameter[] excelParameters, PictureParameter[] pictParams, bool flagQuit)
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {
                //Создание страницы книги Excel
                WExcel._Worksheet m_Sheet;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                string PathOfTemplate = Application.StartupPath + @"\Reports\" + nameOfTemplate;
                m_ExcelApp.Workbooks.Open(PathOfTemplate, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;
                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationManual;
                m_ExcelApp.ScreenUpdating = false;
                //Заполняем отдельные параметры
                if (excelParameters != null)
                    foreach (ExcelParameter parameter in excelParameters)
                    {
                        WExcel.Range r = m_Sheet.get_Range(parameter.NameOfExcel, parameter.NameOfEndMergeExcel);
                        r.Value2 = parameter.Value;
                        r.Merge(false);
                        if (parameter.Borders != null)
                        {
                            foreach (WExcel.XlBordersIndex border in parameter.Borders)
                            {
                                m_Sheet.get_Range(parameter.NameOfExcel, Type.Missing).Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                            }
                        }
                    }
                if (pictParams != null)
                    foreach (PictureParameter picture in pictParams)
                    {
                        try
                        {
                            m_Sheet.Shapes.AddPicture(picture.PathPicture, picture.LinkToFile, picture.SaveWithDocument,
                                picture.Left, picture.Top, picture.Width, picture.Height);
                        }
                        catch { }
                    }
                //Заполняем массив данных
                //Перебираем все таблицы
                int sumCountRow = 0, sum = 0, max = 1, RowInStr;
                if (tables != null)
                {
                    for (int i = 0; i < tables.Count(); i++)
                    {
                        sum += tables[i].Rows.Count;
                        max = Math.Max(tables[i].Columns.Count, max);
                    }
                    sum = Math.Max(sum, 1);
                    if (tables.Count() > 0)//если есть че выполнять - выделять.
                    {
                        WExcel.Range r = m_Sheet.get_Range(startExcel, Excel.AddCols(Excel.AddRows(startExcel, sum - 1), max - 1));
                        string[,] str = new string[sum, max];
                        for (int i = 0; i < tables.Count(); i++)
                        {
                            //Перебираем все колонки
                            for (int column = 0; column < tables[i].Columns.Count; column++)
                            {
                                RowInStr = sumCountRow;
                                if (tables[i].Columns[column].DataType == typeof(DateTime))
                                    for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                        str[RowInStr, column] = (tables[i].Rows[row][column] == DBNull.Value ? "" : ((DateTime)tables[i].Rows[row][column]).ToShortDateString());
                                else
                                    for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                        str[RowInStr, column] = tables[i].Rows[row][column].ToString();
                            }
                            sumCountRow += tables[i].Rows.Count;
                        }
                        r.set_Value(Type.Missing, str);
                        //заверщили заполнение.
                    }
                }
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationAutomatic;
                m_ExcelApp.ScreenUpdating = true;
                m_ExcelApp.Visible = true;
                if (flagQuit)
                {
                    m_Sheet.PrintPreview(true);
                    m_ExcelApp.Quit();
                }
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }


        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="startExcel">Имя ячейки (пример:В8)</param>
        /// <param name="tables">Массивы данных в виде DataTable</param>
        /// <param name="excelParameters">Дополнительные параметры,
        /// с помощью этих параметров задаются значения отдельных ячеек Excel</param>
        public static void PrintRepOtherType(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, int[] columnWidth, string[] ListFormat)
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {
                //Создание страницы книги Excel
                WExcel._Worksheet m_Sheet;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                string PathOfTemplate = Application.StartupPath + @"\Reports\" + nameOfTemplate;
                m_ExcelApp.Workbooks.Open(PathOfTemplate, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;
                m_ExcelApp.ScreenUpdating = false;
                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationManual;
                //Заполняем массив данных
                //Перебираем все таблицы
                int sumCountRow = 0, sum = 0, max = 1, RowInStr;
                for (int i = 0; i < tables.Count(); i++)
                {
                    sum += tables[i].Rows.Count;
                    max = Math.Max(tables[i].Columns.Count, max);
                }
                sum = Math.Max(sum, 1);

                WExcel.Range r = m_Sheet.get_Range(startExcel, Excel.AddRows(Excel.AddCols(startExcel, max - 1), sum - 1));
                string cur_column = startExcel;
                for (int i = 0; i < columnWidth.Count(); i++)
                {
                    WExcel.Range r1 = m_Sheet.get_Range(cur_column, Excel.AddRows(cur_column, sum - 1));
                    r1.ColumnWidth = columnWidth[i];
                    cur_column = Excel.AddCols(cur_column, 1);
                }
                cur_column = startExcel;
                for (int i = 0; i < ListFormat.Count(); i++)
                {
                    cur_column = Excel.AddCols(cur_column, 1);
                    if (ListFormat[i] != "")
                    {
                        WExcel.Range r1 = m_Sheet.get_Range(cur_column, Excel.AddRows(cur_column, sum - 1));
                        r1.NumberFormat = "0,00";
                    }
                }
                object[,] str = new object[sum, max];
                for (int i = 0; i < tables.Count(); i++)
                {
                    //Перебираем все колонки
                    for (int column = 0; column < tables[i].Columns.Count; column++)
                    {

                        RowInStr = sumCountRow;
                        if (tables[i].Columns[column].DataType == typeof(DateTime))
                            for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                str[RowInStr, column] = (tables[i].Rows[row][column] == DBNull.Value ? "" : ((DateTime)tables[i].Rows[row][column]).ToShortDateString());
                        else
                            for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                str[RowInStr, column] = tables[i].Rows[row][column].ToString();
                    }

                    sumCountRow += tables[i].Rows.Count;
                }
                r.set_Value(Type.Missing, str);
                //заверщили заполнение.
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.ScreenUpdating = true;
                m_ExcelApp.Calculation = WExcel.XlCalculation.xlCalculationAutomatic;
                m_ExcelApp.Visible = true;
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public static void PrintRepOtherType(string nameOfTemplate, string startExcel, System.Data.DataTable[] tables, int[] columnWidth)
        {
            PrintRepOtherType(nameOfTemplate, startExcel, tables, columnWidth, new string[] { });
        }

        public static void PrintTemplateForEachRow(string nameOfTemplate, string LeftUpCornerTemplate, string RightDownCornerTemplate, System.Data.DataTable table, ExcelParameter[] excelParameters)
        {
            PrintTemplateForEachRow(nameOfTemplate, LeftUpCornerTemplate, RightDownCornerTemplate, table, excelParameters, true);
        }

        /// <summary>
        /// Печатает шаблон для каждой строки результата запроса
        /// </summary>
        /// <param name="nameOfTemplate">Путь к шаблону эксель</param>
        /// <param name="LeftUpCornerTemplate">Координаты левого верхнего угла шаблона</param>
        /// <param name="RightDownCornerTemplate">Нижнего правого угла</param>
        /// <param name="table">Таблицы с строками</param>
        /// <param name="excelParameters">Эксель параметры с пом. которых указываются координаты ячеек для каждого столбца таблицы</param>
        public static void PrintTemplateForEachRow(string nameOfTemplate, string LeftUpCornerTemplate, string RightDownCornerTemplate, System.Data.DataTable table, ExcelParameter[] excelParameters, bool flagQuit)
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {
                WExcel._Worksheet m_Sheet;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                string PathOfTemplate = Application.StartupPath + @"\Reports\" + nameOfTemplate;
                m_ExcelApp.Workbooks.Open(PathOfTemplate, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;

                WExcel.Range r = m_Sheet.get_Range(LeftUpCornerTemplate, RightDownCornerTemplate);
                WExcel.Range newRange;
                int n = r.Rows.Count, m = r.Columns.Count;
                for (int i = 1; i < table.Rows.Count; i++)
                {
                    newRange = m_Sheet.get_Range(AddRows(LeftUpCornerTemplate, n * i), AddRows(RightDownCornerTemplate, n * i));
                    r.Copy(newRange);
                }
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    for (int j = 0; j < table.Columns.Count; j++)
                    {
                        if (j < excelParameters.Length)
                        {
                            string s1 = excelParameters[j].NameOfExcel,
                                s2 = excelParameters[j].NameOfEndMergeExcel;
                            r = m_Sheet.get_Range(Excel.AddRows(s1, n * i), Excel.AddRows(s2, n * i));
                            r.Merge(false);
                            r.Value2 = excelParameters[j].Value;
                            r.Value2 = table.Rows[i][j].ToString();
                            if (excelParameters[j].Borders != null)
                            {
                                foreach (WExcel.XlBordersIndex border in excelParameters[j].Borders)
                                {
                                    r.Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                                }
                            }
                        }
                    }
                }
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_Sheet.PrintPreview(true);
                if (flagQuit)
                {
                    m_ExcelApp.Quit();
                }
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }

        public static void PrintCellWithMerge(bool ShowEditior, string nameOfTemplate, ExcelParameter[] excelParameters)
        {
            WExcel.Application m_ExcelApp = new WExcel.Application();
            try
            {

                WExcel._Worksheet m_Sheet;
                WExcel.Workbook m_Book ;
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp.Visible = false;
                string PathOfTemplate = Application.StartupPath + @"\Reports\" + nameOfTemplate;
                m_Book=m_ExcelApp.Workbooks.Open(PathOfTemplate, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;
                //Заполняем отдельные параметры
                int i = 0;
                if (excelParameters != null)
                    foreach (ExcelParameter parameter in excelParameters)
                    {
                        //m_Sheet.get_Range(parameter.NameOfExcel, Type.Missing).Borders[WExcel.XlBordersIndex.xlDiagonalUp].Color = Color.Black;
                        //m_Sheet.get_Range(parameter.NameOfExcel, Type.Missing).BorderAround(WExcel.XlLineStyle.xlContinuous, WExcel.XlBorderWeight.xlThin, WExcel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
                        WExcel.Range r = m_Sheet.get_Range(parameter.NameOfExcel, parameter.NameOfEndMergeExcel);
                        m_Book.Styles.Add("MyStyle" + (++i).ToString(), m_Sheet.get_Range("A1", "A1"));
                        m_Book.Styles["MyStyle" + i.ToString()].HorizontalAlignment = parameter.TextAlign;
                        m_Book.Styles["MyStyle" + i.ToString()].Font.Size = parameter.Font.Size;
                        m_Book.Styles["MyStyle" + i.ToString()].Font.Bold = parameter.Font.Bold;
                        r.Style = m_Book.Styles["MyStyle" + i.ToString()];
                        r.Merge(false);
                        m_Sheet.get_Range(parameter.NameOfExcel, Type.Missing).Value2 = parameter.Value;
                        if (parameter.Borders != null)
                        {
                            foreach (WExcel.XlBordersIndex border in parameter.Borders)
                            {
                                r.Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                            }
                        }
                    }
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_Sheet.PrintPreview(true);
                if (!ShowEditior) m_ExcelApp.Quit();
            }
            catch
            {
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                m_ExcelApp.Quit();
                m_ExcelApp = null;
                throw;
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }


        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="startCell">Стартовая ячейка</param>
        /// <param name="tables">Массив типа DataTable</param>
        /// <param name="cellParameters">Параметры ячейки</param>
        public static void PrintR1C1(bool flagQuit, string nameOfTemplate, Cell startCell, System.Data.DataTable[] tables, CellParameter[] cellParameters)
        {

            try
            {
                WExcel.Application m_ExcelApp;
                //Создание книги Excel
                WExcel._Workbook m_Book;
                //Создание страницы книги Excel
                WExcel._Worksheet m_Sheet;
                //private Excel.Range Range;
                WExcel.Workbooks m_Books;
                int columnStart = 0;
                int rowStart = 0;
                int columnNow = 0;
                int rowNow = 0;
                if (startCell != null)
                {
                    columnStart = startCell.Column;
                    rowStart = startCell.Row;
                    columnNow = startCell.Column;
                    rowNow = startCell.Row;
                }
                object oMissing = System.Reflection.Missing.Value;
                m_ExcelApp = new WExcel.Application();
                m_ExcelApp.Visible = false;
                m_Books = m_ExcelApp.Workbooks;
                string PathOfTemplate = Application.StartupPath + @"\Reports\" + nameOfTemplate;
                m_Book = m_Books.Open(PathOfTemplate, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing,
                oMissing, oMissing, oMissing, oMissing, oMissing, oMissing);
                m_Sheet = (WExcel._Worksheet)m_ExcelApp.ActiveSheet;
                //Заполняем отдельные параметры
                if (cellParameters != null)
                    foreach (CellParameter parameter in cellParameters)
                    {
                        m_Sheet.Cells[parameter.Row, parameter.Column] = parameter.Value;
                        if (parameter.Borders != null)
                        {
                            foreach (WExcel.XlBordersIndex border in parameter.Borders)
                            {
                                m_Sheet.get_Range(m_Sheet.Cells[parameter.Row, parameter.Column], Type.Missing).Borders[border].LineStyle = WExcel.XlLineStyle.xlContinuous;
                            }
                        }
                    }
                if (tables != null)
                {
                    //Заполняем массив данных
                    //Перебираем все таблицы
                    int sumCountRow = 0, sum = 0, max = 0, RowInStr, l;
                    string s1;
                    for (int i = 0; i < tables.Count(); i++)
                    {
                        sum += tables[i].Rows.Count;
                        if (tables[i].Columns.Count > max) max = tables[i].Columns.Count;
                    }

                    WExcel.Range r = m_Sheet.get_Range(Excel.ColNumToEx(startCell.Column)+startCell.Row.ToString(),Excel.ColNumToEx(startCell.Column+max-1)+(startCell.Row+sum-1).ToString());
                    r.BorderAround(WExcel.XlLineStyle.xlContinuous, WExcel.XlBorderWeight.xlThin, WExcel.XlColorIndex.xlColorIndexAutomatic, Type.Missing);
                    r.Borders.LineStyle = WExcel.XlLineStyle.xlContinuous;
                    r.Borders.Weight = WExcel.XlBorderWeight.xlThin;
                    string[,] str = new string[sum, max];
                    for (int i = 0; i < tables.Count(); i++)
                    {
                        //Перебираем все колонки
                        for (int column = 0; column < tables[i].Columns.Count; column++)
                        {
                            RowInStr = sumCountRow;
                            if (tables[i].Columns[column].DataType == typeof(DateTime))
                                for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                {
                                    s1 = tables[i].Rows[row][column].ToString();
                                    if ((l = s1.IndexOf(' ')) > -1)
                                        s1 = s1.Substring(0, l);
                                    str[RowInStr, column] = s1;
                                }

                            else
                                for (int row = 0; row < tables[i].Rows.Count; row++, RowInStr++)
                                    str[RowInStr, column] = tables[i].Rows[row][column].ToString();
                        }

                        sumCountRow += tables[i].Rows.Count;
                    }
                    r.set_Value(Type.Missing, str);
                }
                //заверщили заполнение.
                m_ExcelApp.DisplayAlerts = false;
                m_ExcelApp.Visible = true;
                if (flagQuit)
                {
                    m_Sheet.PrintPreview(true);
                    m_ExcelApp.Quit();
                }
            }
            finally
            {
                //Что бы там ни было вызываем сборщик мусора
                GC.WaitForPendingFinalizers();
                GC.Collect();
            }
        }
        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="cellParameters">Параметры ячеек</param>
        public static void PrintR1C1(string nameOfTemplate, CellParameter[] cellParameters)
        {
            PrintR1C1(true, nameOfTemplate, (Cell)null, (System.Data.DataTable[])null, cellParameters);
        }

        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="cellParameters">Параметры ячеек</param>
        public static void PrintR1C1(bool flagQuit, string nameOfTemplate, CellParameter[] cellParameters)
        {
            PrintR1C1(flagQuit, nameOfTemplate, (Cell)null, (System.Data.DataTable[])null, cellParameters);
        }
        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="startCell">Стартовая ячейка</param>
        /// <param name="table">Наименование таблицы типа DataTable</param>
        /// <param name="cellParameters">Параметры ячеек</param>
        public static void PrintR1C1(string nameOfTemplate, Cell startCell, System.Data.DataTable table, CellParameter[] cellParameters)
        {
            PrintR1C1(true, nameOfTemplate, startCell, new System.Data.DataTable[] { table }, cellParameters);
        }
        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование шаблона с расширением</param>
        /// <param name="startCell">Стартовая ячейка</param>
        /// <param name="table">Наименование таблицы типа DataTable</param>
        public static void PrintR1C1(string nameOfTemplate, Cell startCell, System.Data.DataTable table)
        {
            PrintR1C1(nameOfTemplate, startCell, table, null);
        }
        /// <summary>
        /// Печать в Excel
        /// </summary>
        /// <param name="nameOfTemplate">Наименование таблицы типа DataTable</param>
        /// <param name="startCell">Стартовая ячейка</param>
        /// <param name="tables">Массив типа DataTable</param>
        public static void PrintR1C1(string nameOfTemplate, Cell startCell, System.Data.DataTable[] tables)
        {
            PrintR1C1(true, nameOfTemplate, startCell, tables, null);
        }
    }
        
}
