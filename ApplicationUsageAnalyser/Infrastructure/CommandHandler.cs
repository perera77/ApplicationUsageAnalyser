using System;
using System.Windows.Input;

namespace ApplicationUsageAnalyser.Infrastructure
{
    public class CommandHandler : ICommand
    {
        private Action<object> _action;
        private Func<bool> _canExecute;
        public CommandHandler(Action<object> action, Func<bool> canExecute)
        {
            _action = action;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute();
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _action(parameter);
        }
    }
}
