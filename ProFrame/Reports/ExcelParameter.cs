using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MExcel=Microsoft.Office.Interop.Excel;
using D=System.Drawing;
using System.Data;
namespace ProFrame
{
    public class ExcelParameter
    {
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="StartCell">Левый верхний угол объединения</param>
        /// <param name="EndCell">Правый нижний угло объединения(по умолчанию равен левому верхнему)</param>
        /// <param name="value">значение</param>
        /// <param name="Align">Выравнивание в ячейке</param>
        /// <param name="font_cell">Шрифт в ячейке</param>
        /// <param name="borders">Рамки</param>
        public ExcelParameter(string StartCell, string EndCell,string value,MExcel.XlHAlign Align, D.Font font_cell,  params MExcel.XlBordersIndex[] borders)
        {
            NameOfExcel=StartCell;
            NameOfEndMergeExcel=EndCell;
            Value=value;
            TextAlign = Align;
            if (font_cell!=null) Font=font_cell; else Font = new System.Drawing.Font("Arial",14);
            Borders = borders;
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nameOfExcel">Имя ячейки (пример:"D8")</param>
        /// <param name="value">Значение ячейки</param>
        public ExcelParameter(string nameOfExcel, string value, params MExcel.XlBordersIndex[] borders)
            :this(nameOfExcel,nameOfExcel,value,MExcel.XlHAlign.xlHAlignCenter,null)
        {            
        }
        public ExcelParameter(string nameOfExcel, string value)
            : this(nameOfExcel, value, null)
        {
        }

        public ExcelParameter(D.Font f,MExcel.XlHAlign Align,string StartCell, string EndCell, string value)
            :this(StartCell,EndCell,value,Align,f,null)
        {}
        /// <summary>
        /// конструктор с указанием координта ячеек в цифровом виде
        /// (нумерация с 1 начинается)
        /// </summary>
        /// <param name="f"></param>
        /// <param name="Align"></param>
        /// <param name="nameOfExcel"></param>
        /// <param name="nameOfEndMergeExcel"></param>
        /// <param name="value"></param>
        public ExcelParameter(MExcel.XlHAlign Align, D.Point nameOfExcel, string value)
            :this(Excel.ColNumToEx(nameOfExcel.X)+nameOfExcel.Y.ToString(), Excel.ColNumToEx(nameOfExcel.X)+nameOfExcel.Y.ToString(),
            value,Align,null,null)
        {}

        public ExcelParameter(D.Point nameOfExcel, string value)
            :this(MExcel.XlHAlign.xlHAlignCenter,nameOfExcel,value)
        {
        }

        public ExcelParameter(MExcel.XlHAlign Align, D.Point StartCell, D.Point EndCell, string value)
            : this(Excel.ColNumToEx(StartCell.X) + StartCell.Y.ToString(), Excel.ColNumToEx(EndCell.X) + EndCell.Y.ToString(),
            value,Align,null,null)
        {}

        public ExcelParameter(D.Point StartCell, string value, params Microsoft.Office.Interop.Excel.XlBordersIndex[] borders)
            : this( StartCell,StartCell, value, borders)
        { }

        public ExcelParameter( D.Point StartCell, D.Point EndCell, string value, params Microsoft.Office.Interop.Excel.XlBordersIndex[] borders)
            :this(Excel.ColNumToEx(StartCell.X) + StartCell.Y.ToString(), Excel.ColNumToEx(EndCell.X) + EndCell.Y.ToString(),value,MExcel.XlHAlign.xlHAlignCenter,null,borders)
        {}

        public ExcelParameter(System.Drawing.Font f, MExcel.XlHAlign Align, D.Point StartCell, D.Point EndCell, string value)
            :this(Excel.ColNumToEx(StartCell.X) + StartCell.Y.ToString(), Excel.ColNumToEx(EndCell.X) + EndCell.Y.ToString(),value,Align,f,null)
        { }

#region Свойства параметра экселя
        /// <summary>
        /// Наименование ячейки
        /// </summary>
        public string NameOfExcel
        {
            get;set;
        }
        /// <summary>
        /// Наименование ячейки заканчивающей объединение
        /// </summary>
        public string NameOfEndMergeExcel
        {
            get;
            set;
        }
        /// <summary>
        /// Значение ячейки
        /// </summary>
        public string Value
        {
           get;set;
        }

        /// <summary>
        /// Массив рамок
        /// </summary>
        public MExcel.XlBordersIndex[] Borders
        {
            get;set;
        }
        /// <summary>
        /// Выравнивание
        /// </summary>
        public MExcel.XlHAlign TextAlign
        {
            get;set;
        }
        /// <summary>
        /// Шрифт
        /// </summary>
        public System.Drawing.Font Font
        {
            get;set;
        }
#endregion
    }


    public class TotalRowsStyle
    {
        public TotalRowsStyle(string totalColumnName, System.Drawing.Color backColor, System.Drawing.Color foreColor, object totalFlagValue)
        {
            TotalFlagColumnName = totalColumnName;
            TotalFlagValue = totalFlagValue;
            BackColor = backColor;
            ForeColor = foreColor;
        }

        public string TotalFlagColumnName
        { get; set; }
        public object TotalFlagValue
        {
            get;
            set;
        }
        public System.Drawing.Color BackColor
        { get; set; }
        public System.Drawing.Color ForeColor
        { get; set; }
    }

    public class ExcelPrintTable
    {
        public ExcelPrintTable(DataTable t, string StartCell, bool PrintColumnHeader=false)
        {
            Table = t;
            StartPrintCell = StartCell;
            WithColumnHeader = PrintColumnHeader;
        }
        public DataTable Table
        {
            get;
            set;
        }
        public string StartPrintCell
        { get; set; }
        public bool WithColumnHeader
        {
            get;
            set;
        }
    }
}
