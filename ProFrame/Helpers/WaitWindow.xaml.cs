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
    /// Логика взаимодействия для WaitWindow.xaml
    /// </summary>
    public partial class WaitWindow : Window
    {
        //private string _contentText;
        public WaitWindow(AbortableBackgroundWorker related_worker = null)
        {
            BackWorker = related_worker;
            InitializeComponent();
            this.DataContext = related_worker;
            if (related_worker != null && related_worker.InQueue)
            {
                this.Loaded += WaitWindow_Loaded;
            }
        }

        private void WaitWindow_Loaded(object sender, RoutedEventArgs e)
        {
            SetPosition(GetQueuePosition(BackWorker));
        }
        
        public AbortableBackgroundWorker BackWorker
        {
            get;
            set;
        }

        public bool IsAbortable
        {
            get
            {
                return BackWorker != null;
            }
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (IsAbortable)
            {
                BackWorker.Abort();
            }
        }

        protected Point GetQueuePosition(AbortableBackgroundWorker item)
        {
            int k = AbortableBackgroundWorker.Queue.IndexOf(item);
            AbortableBackgroundWorker prev;
            if (k < 0)
                prev = AbortableBackgroundWorker.Queue.LastOrDefault();
            else
                prev = k > 0 ? AbortableBackgroundWorker.Queue[k] : null;
            Window mainWindow = this.Owner;
            if (prev != null)
            {
                if (prev.WaitWindow.Top - this.ActualHeight - 40 > 0)
                    return new Point(prev.WaitWindow.Left, prev.WaitWindow.Top - this.ActualHeight - 40);
                else
                    return new Point(prev.WaitWindow.Left - ActualWidth, Owner.ActualHeight - this.ActualHeight - 40);
            }
            return new Point(mainWindow.ActualWidth - this.ActualWidth, Math.Min(Owner.ActualHeight, System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height) - this.ActualHeight - 40);
        }

        public void SetPosition(Point p)
        {
            this.WindowStartupLocation = WindowStartupLocation.Manual;
            Left = p.X;
            Top = p.Y;
        }

        public static void RepositionAllWindows()
        {
            foreach (var item in AbortableBackgroundWorker.Queue)
            {
                item.WaitWindow.SetPosition(item.WaitWindow.GetQueuePosition(item));
            }

        }

    }
}
