using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FilterExplorer.Commands
{
    public interface IDelegateCommand : ICommand
    {
        void RaiseCanExecuteChanged();
    }

    public class DelegateCommand : IDelegateCommand
    {
        private readonly Action<object> _action;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged;

        public DelegateCommand(Action<object> action)
            : this(action, null)
        {
        }

        public DelegateCommand(Action<object> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute();
        }

        public void Execute(object parameter)
        {
            if (CanExecute(parameter))
            {
                _action(parameter);
            }
        }

        public void RaiseCanExecuteChanged()
        {
            if (CanExecuteChanged != null)
            {
                CanExecuteChanged(this, EventArgs.Empty);
            }
        }
    }
}
