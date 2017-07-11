using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Windows;

namespace ProFrame
{
    public class AbortableBackgroundWorker : BackgroundWorker, INotifyPropertyChanged
    {

        private Thread workerThread;
        private string _currentStatus;
        private string _tempStatus;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void RaisePropertyChanged<T>(Expression<Func<T>> action)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(((MemberExpression)action.Body).Member.Name));
        }


        protected override void OnDoWork(DoWorkEventArgs e)
        {
            workerThread = Thread.CurrentThread;
            try
            {
                base.OnDoWork(e);
            }
            catch (ThreadAbortException)
            {
                e.Cancel = true; //We must set Cancel property to true!
                Thread.ResetAbort(); //Prevents ThreadAbortException propagation
            }
            catch (Exception ex)
            {
                if (ex.GetType().Name == "OracleException")
                {
                    System.Reflection.PropertyInfo prop = ex.GetType().GetProperty("Number");
                    if (prop != null)
                    {
                        int error_code = (int)ex.GetType().GetProperty("Number").GetValue(ex, null);
                        if (error_code == 1013)
                        {
                            e.Cancel = true; //We must set Cancel property to true!
                        }
                        else
                            throw ex;
                    }
                }
            }
        }


        public void Abort()
        {
            if (this.IsBusy) // если задача уже работает то прерываем команду, и там возникнет событие заверщения задачи и удалит сама поток
            {
                if (ExecutingCommand != null)
                    ExecutingCommand.Cancel();
                else
                    if (workerThread != null)
                {
                    workerThread.Abort();
                    workerThread = null;
                }
            }
            else // иначе просто удаляем из очереди 
            {
                lock (Queue)
                {
                    if (this.waitDialog != null && waitDialog.IsLoaded)
                        waitDialog.Close();
                    Queue.Remove(this);
                }
                //WaitWindow.RepositionAllWindows();
            }

        }

        public UniDbCommand ExecutingCommand
        {
            get;
            set;
        }

        private WaitWindow waitDialog
        {
            get;
            set;
        }

        public WaitWindow WaitWindow
        {
            get
            {
                return waitDialog;
            }
        }

        public static void RunAsyncWithWaitDialog(DependencyObject sender, string caption, DoWorkEventHandler dowork,
            object argument, UniDbCommand executingCommand, RunWorkerCompletedEventHandler workComplete, bool inQueue = true)
        {
            AbortableBackgroundWorker bw = new AbortableBackgroundWorker();
            bw.WorkerSupportsCancellation = true;
            bw.WorkerReportsProgress = true;
            bw.DoWork += dowork;
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
            bw.RunWorkerCompleted += workComplete;
            bw.ProgressChanged += Bw_ProgressChanged;
            bw.ExecutingCommand = executingCommand;
            bw.Argument = argument;
            bw.InQueue = inQueue;
            bw.CurrentStatus = caption;
            WaitWindow f = new WaitWindow(bw);
            f.Owner = Window.GetWindow(sender);
            bw.waitDialog = f;
            f.Show();

            if (bw.InQueue)
            {
                Queue.Enqueue(bw);
            }
            else
                bw.RunWorkerAsync(argument);
        }

        static ConnectionQueue _queue;
        public static ConnectionQueue Queue
        {
            get
            {
                if (_queue == null)
                {
                    _queue = new ConnectionQueue();
                }
                return _queue;
            }
        }




        /// <summary>
        /// Строка  - надпись на экране что сейчас происходит
        /// </summary>
        public string CurrentStatus
        {
            get
            {
                return _currentStatus;
            }
            set
            {
                _currentStatus = value;
                RaisePropertyChanged(() => CurrentStatus);
            }
        }
        string firstStatus;
        private bool _inQueue = false;

        /// <summary>
        /// Временный статус для выполнения операции
        /// </summary>
        public string TempStatus
        {
            get
            {
                return _tempStatus;
            }
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    CurrentStatus = firstStatus;
                }
                else
                {
                    _tempStatus = value;
                    firstStatus = CurrentStatus;
                    CurrentStatus = value;
                }
            }

        }

        public object Argument
        {
            get; set;
        }

        private static void Bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {

        }

        static void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            (sender as AbortableBackgroundWorker).waitDialog.Close();
        }
        /// <summary>
        /// Асинхронно выполняет загрузку данных и при ошибках показывает сообщение, иначе если все успешно прошло, выполняет делегат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="caption"></param>
        /// <param name="argument"></param>
        /// <param name="execCommand"></param>
        /// <param name="workSuccessEnded"></param>
        public static void RunAsyncWithWaitDialog(DependencyObject sender, string caption, object argument, UniDbCommand execCommand, RunWorkerCompletedEventHandler workSuccessEnded)
        {
            AbortableBackgroundWorker.RunAsyncWithWaitDialog(sender, caption,
                (p, pw) =>
                {
                    UniDbAdapter a = pw.Argument as UniDbAdapter;
                    DataSet ds = new DataSet();
                    a.Fill(ds);
                    pw.Result = ds;
                }, argument, execCommand,
                (p, pw) =>
                {
                    if (pw.Cancelled) return;
                    else if (pw.Error != null) MessageBox.Show(pw.Error.GetFormattedException(), "Ошибка получения данных");
                    else
                        workSuccessEnded(p, pw);
                });
        }


        /// <summary>
        /// Асинхронно выполняет команду и при ошибках показывает сообщение, иначе если все успешно прошло, выполняет делегат
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="caption"></param>
        /// <param name="argument"></param>
        /// <param name="execCommand"></param>
        /// <param name="workSuccessEnded"></param>
        public static void RunAsyncWithWaitDialog(DependencyObject sender, string caption, UniDbCommand execCommand, RunWorkerCompletedEventHandler workSuccessEnded)
        {
            AbortableBackgroundWorker.RunAsyncWithWaitDialog(sender, caption,
                (p, pw) =>
                {
                    execCommand.ExecuteNonQuery();
                }, null, execCommand,
                (p, pw) =>
                {
                    if (pw.Cancelled) return;
                    else if (pw.Error != null) MessageBox.Show(pw.Error.GetFormattedException(), "Ошибка получения данных");
                    else
                        workSuccessEnded(p, pw);
                });
        }

        /// <summary>
        /// Находится ли в очереди задача
        /// </summary>
        public bool InQueue
        {
            get
            {
                return _inQueue;
            }
            set
            {
                _inQueue = value;
                RaisePropertyChanged(() => InQueue);
            }
        }

        public Point GetWaitwindowPosition()
        {
            if (waitDialog != null)
                return new Point(waitDialog.Left, waitDialog.Top);
            return new Point(0, 0);
        }
    }
}
