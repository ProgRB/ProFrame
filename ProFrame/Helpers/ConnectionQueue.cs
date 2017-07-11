using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ProFrame
{
    public class ConnectionQueue : List<AbortableBackgroundWorker>
    {
        private UniDbConnection _connection;
        private bool _isBusy = false;

        public ConnectionQueue() : base()
        {
        }

        public UniDbConnection Connection
        {
            get
            {
                return _connection;
            }
            set
            {
                _connection = value;
            }
        }

        public UniDbConnection NewConnection()
        {
            Tuple<ConnectingResult, Exception, UniDbConnection> res = UniDbConnection.OpenNew();
            if (res.Item2 == null)
            {
                return res.Item3;
            }
            else
            {
                MessageBox.Show(res.Item2.Message);
                return null;
            }
        }

        public void Enqueue(AbortableBackgroundWorker item)
        {
            item.RunWorkerCompleted += Item_RunWorkerCompleted;
            item.TempStatus = "Ожидание в очереди операций...";
            base.Add(item);

            if (!IsBusy)
            {
                CheckConnection();
                RunAsyncNext();
            }
        }

        private void CheckConnection()
        {
            if (Connection == null)
            {
                Connection = NewConnection();
            }
        }

        private void Item_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            IsBusy = false;
            this.Dequeue();
            RunAsyncNext();
        }

        public AbortableBackgroundWorker Dequeue()
        {
            if (this.Count > 0)
            {
                var item = this[0];
                this.Remove(this[0]);
                return item;
            }
            else
                return null;
        }

        /// <summary>
        /// Запускает следующую операцию в очереди
        /// </summary>
        public void RunAsyncNext()
        {
            if (this.Count > 0)
            {
                IsBusy = true;
                AbortableBackgroundWorker bw = this.Peek();
               /* if (bw.ExecutingCommand != null)
                    bw.ExecutingCommand.Connection = Connection;*/
                bw.TempStatus = null;
                bw.RunWorkerAsync(bw.Argument);
            }
        }

        public AbortableBackgroundWorker Peek()
        {
            return this.Count > 0 ? this[0] : null;
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }
            set
            {
                _isBusy = value;
            }
        }

        protected new void Remove(AbortableBackgroundWorker item)
        {
            base.Remove(item);
            ListChanged?.Invoke(this, EventArgs.Empty);
        }

        protected new void Add(AbortableBackgroundWorker item)
        {
            base.Add(item);
            ListChanged?.Invoke(this, EventArgs.Empty);
        }

        public event EventHandler ListChanged;
    }
}
