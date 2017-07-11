using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using D = System.Drawing;

namespace ProFrame
{
    public class PictureParameter
    {
        string _pathPicture;
        Microsoft.Office.Core.MsoTriState _linkToFile, _saveWithDocument;
        int _left, _top, _width, _height;
        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="nameOfExcel">Имя ячейки (пример:"D8")</param>
        /// <param name="value">Значение ячейки</param>
        public PictureParameter(string pathPicture, Microsoft.Office.Core.MsoTriState linkToFile, 
            Microsoft.Office.Core.MsoTriState saveWithDocument, int left, int top, int width, int height)
        {
            _pathPicture = pathPicture;
            _linkToFile = linkToFile;
            _saveWithDocument = saveWithDocument;
            _left = left;
            _top = top;
            _width = width;
            _height = height;
        }
        /// <summary>
        /// Путь к файлу картинки
        /// </summary>
        public string PathPicture
        {
            get
            {
                return _pathPicture;
            }
            set 
            {
                _pathPicture = value;
            }
        }
        /// <summary>
        /// Параметры вставки картинки (не знаю зачем...)
        /// </summary>
        public Microsoft.Office.Core.MsoTriState LinkToFile
        {
            get
            {
                return _linkToFile;
            }
            set
            {
                _linkToFile = value;
            }
        }

        /// <summary>
        /// Параметры вставки картинки (не знаю зачем...)
        /// </summary>
        public Microsoft.Office.Core.MsoTriState SaveWithDocument
        {
            get
            {
                return _saveWithDocument;
            }
            set 
            {
                _saveWithDocument = value;
            }
        }

        /// <summary>
        /// Левая граница
        /// </summary>
        public int Left
        {
            get
            {
                return _left;
            }
            set 
            {
                _left = value;
            }
        }

        /// <summary>
        /// Верхняя граница
        /// </summary>
        public int Top
        {
            get
            {
                return _top;
            }
            set 
            {
                _top = value;
            }
        }

        /// <summary>
        /// Ширина
        /// </summary>
        public int Width
        {
            get
            {
                return _width;
            }
            set 
            {
                _width = value;
            }
        }

        /// <summary>
        /// Высота
        /// </summary>
        public int Height
        {
            get
            {
                return _height;
            }
            set 
            {
                _height = value;
            }
        }
    }
}
