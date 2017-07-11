using OfficeOpenXml;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProFrame
{
    public static class ExcelWithOpenXml
    {
        /// <summary>
        /// Создание файла Excel для записи данных из DataSet (для каждой таблицы создается отдельный лист)
        /// </summary>
        /// <param name="pathFile">Путь сохранения файла (если файл существует, он будет перезаписан)</param>
        /// <param name="dataSet">DataSet с данными</param>
        public static void CreateExcelFile(string pathFile, DataSet dataSet)
        {
            using (ExcelPackage objExcelPackage = new ExcelPackage())
            {
                foreach (DataTable dtSrc in dataSet.Tables)
                {
                    //Create the worksheet    
                    ExcelWorksheet objWorksheet = objExcelPackage.Workbook.Worksheets.Add(dtSrc.TableName);
                    //Load the datatable into the sheet, starting from cell A1. Print the column names on row 1    
                    objWorksheet.Cells["A1"].LoadFromDataTable(dtSrc, true);
                    objWorksheet.Cells.Style.Font.SetFromFont(new Font("Calibri", 10));
                    objWorksheet.Cells.AutoFitColumns();
                    //Format the header    
                    using (ExcelRange objRange = objWorksheet.Cells["A1:XFD1"])
                    {
                        objRange.Style.Font.Bold = true;
                        objRange.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        objRange.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                        objRange.Style.Fill.PatternType = ExcelFillStyle.Solid;
                        objRange.Style.Fill.BackgroundColor.SetColor(Color.FromArgb(12632256));
                    }
                }

                //Write it back to the client    
                if (File.Exists(pathFile))
                    File.Delete(pathFile);

                //Create excel file on physical disk    
                FileStream objFileStrm = File.Create(pathFile);
                objFileStrm.Close();

                //Write content to excel file    
                File.WriteAllBytes(pathFile, objExcelPackage.GetAsByteArray());
            }
        }

        /// <summary>
        /// Создание файла Excel с выбором пути сохранения для записи данных из DataSet (для каждой таблицы создается отдельный лист)
        /// </summary>
        /// <param name="dataSet">DataSet с данными</param>
        public static void CreateExcelFile(DataSet dataSet)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.OverwritePrompt = true;
            sf.Filter = "Excel |*.xlsx";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                CreateExcelFile(sf.FileName, dataSet);
            }
        }
    }
}
