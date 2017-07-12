using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProFrame
{
    public partial class TableEditorViewModel<T> : NotificationObject, ITableEditor<T> where T : UniDbRow, new()
    {
        public TableEditorViewModel(string tableName, string editCommand = null)
        {
            _items = new UniDbTable<T>(tableName);
            EditCommand = string.IsNullOrEmpty(editCommand) ? _items.TableName + "_Edit" : editCommand;
        }

        UniDbTable<T> _items;
        public UniDbTable<T> Items
        {
            get
            {
                return _items;
            }
        }

        T _currentItem;
        /// <summary>
        /// Текущий элемент коллекции используемый для удаления 
        /// </summary>
        public T CurrentItem
        {
            get
            {
                return _currentItem;
            }

            set
            {
                _currentItem = value;
                RaisePropertyChanged(() => CurrentItem);
            }
        }

        public void AddItem(T item)
        {
            Items.Add(item);
        }

        public void DeleteItem(T item)
        {
            Items.Remove(item);
        }

        public Exception Save()
        {
            return Items.Save();
        }

        string _editCommand;
        public string EditCommand
        {
            get
            {
                return _editCommand;
            }
            set
            {
                _editCommand = value;
                RaisePropertyChanged(() => EditCommand);
            }
        }

        /// <summary>
        /// Обновление данных локальных таблицы
        /// </summary>
        public void RefreshView()
        {
            Items.RefreshTableView();
        }
    }

    public interface ITableEditor<T>
    {
        void AddItem(T item);
        void DeleteItem(T item);
        Exception Save();

        T CurrentItem { get; set; }
    }
}
