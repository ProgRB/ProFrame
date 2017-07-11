using System;
using System.Windows.Controls;
using System.Windows;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ProFrame
{
    public class ViewTabBase : NotificationObject
    {
        private static RoutedUICommand _closeTabCommand;
        public ViewTabBase(string headerText, UserControl contentData) :
            this(headerText, contentData, null)
        { }

        public ViewTabBase(string headerText, UserControl contentData, Uri uriIcon)
        {
            ContentData = contentData;
            HeaderText = headerText;
            if (uriIcon != null)
                HeaderIcon = new BitmapImage(uriIcon);
        }

        static ViewTabBase()
        {
            InputGestureCollection g = new InputGestureCollection();
            g.Add(new KeyGesture(Key.W, ModifierKeys.Control, "Ctrl+W"));
            _closeTabCommand = new RoutedUICommand("Закрыть вкладку", "closeTabCommand", typeof(ViewTabBase));
        }

        string _headerText;

        /// <summary>
        /// Заголовок текст вкладки
        /// </summary>
        public string HeaderText
        {
            get
            {
                return _headerText;
            }
            set
            {
                _headerText = value;
                RaisePropertyChanged(() => HeaderText);
            }
        }

        ImageSource _iconSource;
        /// <summary>
        /// Иконка для класса вкладки
        /// </summary>
        public ImageSource HeaderIcon
        {
            get
            {
                return _iconSource;
            }
            set
            {
                _iconSource = value;
                RaisePropertyChanged(() => HeaderIcon);
            }
        }

        public UserControl ContentData
        {
            get;
            set;
        }
        public event CancelEventHandler RequestClose;

        public void ValidateClose(CancelEventArgs e)
        {
            if (RequestClose != null)
            {
                RequestClose(this, e);
            }
        }

        public static RoutedCommand CloseTabCommand
        {
            get
            {
                return _closeTabCommand;
            }
        }
    }
}