using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    /// <summary>
    /// Класс "Ячейка"
    /// </summary>
    public class Cell
    {
        /// <summary>
        /// Констуктор
        /// </summary>
        /// <param name="row">Номер строки</param>
        /// <param name="column">Номер колонки</param>
        public Cell(int row,int column)
        {
            this.Column = column;
            this.Row = row;
        }
        /// <summary>
        /// Номер строки 
        /// </summary>
        public int Row { get; set; }
        /// <summary>
        /// Номер колонки
        /// </summary>
        public int Column { get; set; }
    }
    /// <summary>
    /// Параметризированная ячейка
    /// </summary>
    public class CellParameter
    {
        Cell _cell;
        string _value;
        Microsoft.Office.Interop.Excel.XlBordersIndex[] _borders;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="row">Номер строки</param>
        /// <param name="column">Номер колонки</param>
        /// <param name="value">Значение ячейки</param>
        public CellParameter(int row,int column,string value, Microsoft.Office.Interop.Excel.XlBordersIndex[] borders)
        {
            _cell = new Cell(row,column);
            _value = value;
            _borders = borders;
        }
        /// <summary>
        /// Номер строки ячейки
        /// </summary>
        public int Row
        {
            get
            {
                return _cell.Row;
            }
        }
        /// <summary>
        /// Номер столбца ячейки
        /// </summary>
        public int Column
        {
            get
            {
                return _cell.Column;
            }
        }
        /// <summary>
        /// Значение ячейки
        /// </summary>
        public string Value
        {
            get
            {
                return _value;
            }
        }

        /// <summary>
        /// Массив рамок
        /// </summary>
        public Microsoft.Office.Interop.Excel.XlBordersIndex[] Borders
        {
            get
            {
                return _borders;
            }
        }
    }
}
