using System;
using System.Windows.Input;

namespace LanderNet.UI.Util
{
    public class DelegateCommand<T> : ICommand
    {
        private readonly Predicate<T> _canExecute;
        private readonly Action<T> _execute;

        public DelegateCommand(Action<T> execute)
            : this(execute, x => true)
        {
        }

        public DelegateCommand(Action<T> execute, Predicate<T> canExecute)
        {
            if (canExecute == null) throw new ArgumentNullException("canExecute");
            if (execute == null) throw new ArgumentNullException("execute");

            _execute = execute;
            _canExecute = canExecute;
        }

        public void Execute(object parameter = null)
        {
            _execute((T) parameter);
        }

        public bool CanExecute(object parameter = null)
        {
            return _canExecute((T) parameter);
        }

        public event EventHandler CanExecuteChanged;

        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public class DelegateCommand : DelegateCommand<object>
    {
        public DelegateCommand(Action execute)
            : base(execute != null ? x => execute() : (Action<object>) null)
        {
        }

        public DelegateCommand(Action execute, Func<bool> canExecute)
            : base(execute != null ? x => execute() : (Action<object>) null,
                canExecute != null ? x => canExecute() : (Predicate<object>) null)
        {
        }
    }
}