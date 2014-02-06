/*
 * Copyright (c) 2014 Nokia Corporation. All rights reserved.
 *
 * Nokia and Nokia Connecting People are registered trademarks of Nokia Corporation.
 * Other product and company names mentioned herein may be trademarks
 * or trade names of their respective owners.
 *
 * See the license text file for license information.
 */

using System;
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
