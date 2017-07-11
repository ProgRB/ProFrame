using System;
using System.Windows.Input;

namespace ProFrame
{
    public class DelegateCommand : ICommand
    {
        private readonly Action _command;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public DelegateCommand(Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public DelegateCommand(string name, string caption, Action command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
            Name= name;
            Text = caption;
        }

        public void Execute(object parameter)
        {
            _command();
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null || _canExecute()) && (string.IsNullOrEmpty(Name) 
                //|| ControlRoles.GetState(Name)
                );
        }

        public string Name
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action<object> _command;
        private readonly Func<bool> _canExecute;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RelayCommand(Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
        }

        public RelayCommand(string name, string caption, Action<object> command, Func<bool> canExecute = null)
        {
            if (command == null)
                throw new ArgumentNullException();
            _canExecute = canExecute;
            _command = command;
            Name = name;
            Text = caption;
        }

        public void Execute(object parameter)
        {
            _command(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return (_canExecute == null || _canExecute()) && (string.IsNullOrEmpty(Name) 
                //|| ControlRoles.GetState(Name)
                );
        }

        public string Name
        {
            get;
            set;
        }

        public string Text
        {
            get;
            set;
        }
    }
}