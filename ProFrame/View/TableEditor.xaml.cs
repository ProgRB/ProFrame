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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Telerik.Windows.Controls;

namespace ProFrame
{
    /// <summary>
    /// Interaction logic for TableViewer.xaml
    /// </summary>
    public partial class TableEditor : UserControl
    {
        /// <summary>
        /// Конструктор формы для редактирование таблицы.
        /// </summary>
        /// <param name="TableName">Имя таблицы (и если требуется то схема) Пример: APSTAFF.EMP или EMP если задано <see cref="SchemaTableManager.DefaultSchemaName"/></param>
        public TableEditor(string tableName)
        {
            
            InitializeComponent();
            if (!string.IsNullOrEmpty(tableName)) TableName = tableName;
        }

        public TableEditor():this(string.Empty)
        {
        }

        string _tableName;
        public string TableName
        {
            get
            {
                return _tableName;
            }
            set
            {
                _tableName = value;
                _model = new TableEditorViewModel<UniDbRow>(value);
                CreateGridColumns();
                DataContext = Model;
            }
        }

        public virtual void CreateGridColumns()
        {
            grid.Columns.Clear();
            foreach (var p in Model.Items.SchemaOfTable.Columns)
            {
                if (p.IsVisible)
                {
                    string bindingPath = p.DbColumnName;
                    grid.Columns.Add(new GridViewDataColumn() { DataMemberBinding = new Binding(bindingPath) });
                }
            }
        }

        public static RoutedUICommand AddItem
        {
            get; set;
        } = new RoutedUICommand("Добавить запись", "AddNew", typeof(TableEditor));
        public static RoutedUICommand DeleteItem
        {
            get; set;
        } = new RoutedUICommand("Удалить запись", "DeleteItem", typeof(TableEditor));
        public static RoutedUICommand SaveItems
        {
            get; set;
        } = new RoutedUICommand("Сохранить измененные данные", "SaveItems", typeof(TableEditor));

        private void RefreshTable_Click(object sender, RoutedEventArgs e)
        {
            Model.RefreshView();
        }

        private void Add_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(Model?.EditCommand);
        }

        private void Add_executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (Model != null)
            {
                Model.AddItem(new UniDbRow());
            }
        }

        private void Delete_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(Model?.EditCommand) && Model?.CurrentItem!=null;
        }

        private void Delete_executed(object sender, ExecutedRoutedEventArgs e)
        {
            Model.DeleteItem(Model.CurrentItem);
        }

        private void Save_CanExecute(object sender, CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = ControlRoles.GetState(Model?.EditCommand) && Model?.Items.HasChanges==true;
        }

        private void Save_executed(object sender, ExecutedRoutedEventArgs e)
        {
            Exception ex = Model.Save();
            if (ex != null)
            {
                MessageBox.Show(Window.GetWindow(this), ex.GetFormattedException(), "Ошибка сохранения данных");
            }
        }

        TableEditorViewModel<UniDbRow> _model;
        /// <summary>
        /// Модель данных формы
        /// </summary>
        public TableEditorViewModel<UniDbRow> Model
        {
            get
            {
                return _model;
            }
            set
            {
                _model = value;
            }
        }
    }

    
}
