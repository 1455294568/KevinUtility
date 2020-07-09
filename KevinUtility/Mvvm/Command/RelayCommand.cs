using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace KevinUtility.Mvvm
{
    /// <summary>
    /// RelayCommand
    /// </summary>
    public class RelayCommand : ICommand
    {
        Func<bool> _runable;
        Action _delegatation;

        public RelayCommand(Func<bool> runable, Action delegatation)
        {
            _runable = runable;
            _delegatation = delegatation;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            return _runable != null ? _runable.Invoke() : true;
        }

        public void Execute(object parameter)
        {
            _delegatation?.Invoke();
        }
    }

    /// <summary>
    /// RelayCommand Generic version
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class RelayCommand<T> : ICommand
    {
        Predicate<T> _runable;
        Action<T> _delegatation;

        public RelayCommand(Predicate<T> runable, Action<T> delegatation)
        {
            _runable = runable;
            _delegatation = delegatation;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        public bool CanExecute(object parameter)
        {
            if (_runable == null)
            {
                return true;
            }

            if (parameter == null
                    && typeof(T).GetTypeInfo().IsValueType)
            {
                return _runable.Invoke(default(T));
            }

            if (parameter == null || parameter is T)
            {
                return (_runable.Invoke((T)parameter));
            }
            return false;
        }

        public void Execute(object parameter)
        {
            if (parameter == null
                    && typeof(T).GetTypeInfo().IsValueType)
            {
                _delegatation?.Invoke(default(T));
            }

            if (parameter == null || parameter is T)
            {
                _delegatation?.Invoke((T)parameter);
            }
        }
    }
}
