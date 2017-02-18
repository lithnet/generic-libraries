using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;

namespace Lithnet.Common.Presentation
{
    /// <summary>
    /// Implements ICommand in a delegate friendly way
    /// </summary>
    public class DelegateCommand : ICommand
    {
        private Predicate<object> canExecuteMethod;
        private Action<object> executeMethod;
        
        /// <summary>
        /// Create a command that can always be executed
        /// </summary>
        /// <param name="executeMethod">The method to execute when the command is called</param>
        public DelegateCommand(Action<object> executeMethod) : this(executeMethod, null) { }

        /// <summary>
        /// Create a delegate command which executes the canExecuteMethod before executing the executeMethod
        /// </summary>
        /// <param name="executeMethod"></param>
        /// <param name="canExecuteMethod"></param>
        public DelegateCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            if (null == executeMethod)
                throw new ArgumentNullException(nameof(executeMethod));

            this.executeMethod = executeMethod;
            this.canExecuteMethod = canExecuteMethod;
        }

        public bool CanExecute(object parameter)
        {
            return (null == canExecuteMethod) ? true : canExecuteMethod(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public void Execute(object parameter)
        {
            this.executeMethod(parameter);
        }
    }
}
