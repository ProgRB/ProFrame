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
using System.Windows.Shapes;

namespace ProFrame
{
    /// <summary>
    /// Interaction logic for TableEditorWindow.xaml
    /// </summary>
    public partial class TableEditorWindow : Window
    {
        public TableEditorWindow(string tableName)
        {
            InitializeComponent();
            editor.TableName = tableName;
        }

        public TableEditorViewModel<UniDbRow> Model
        {
            get
            {
                return editor.Model;
            }
        }
    }
}
