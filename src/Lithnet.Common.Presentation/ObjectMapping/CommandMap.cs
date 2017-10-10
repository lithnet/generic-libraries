using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using Lithnet.Common.Presentation;

namespace Lithnet.Common.Presentation
{
    public class CommandMap : ObjectMap
    {
        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        public DelegateCommand AddItem(string commandName, Action<object> executeMethod)
        {
            DelegateCommand d = new DelegateCommand(executeMethod);
            this[commandName] = d;
            return d;
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public DelegateCommand AddItem(string commandName, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            DelegateCommand d = new DelegateCommand(executeMethod, canExecuteMethod);
            this[commandName] = d;
            return d;
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public MenuCommand AddItem(string commandName, string text, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            MenuCommand d = new MenuCommand(text, executeMethod, canExecuteMethod);
            this[commandName] = d;
            return d;
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="gesture">The input gesture that triggers the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public MenuCommand AddItem(string commandName, string text, KeyGesture gesture, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            MenuCommand d = new MenuCommand(text, gesture, executeMethod, canExecuteMethod);
            this[commandName] = d;
            return d;
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public MenuCommand AddItem(string commandName, string text, Action<object> executeMethod)
        {
            MenuCommand d = new MenuCommand(text, executeMethod);
            this[commandName] = d;
            return d;
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="gesture">The input gesture that triggers the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public MenuCommand AddItem(string commandName, string text, KeyGesture gesture, Action<object> executeMethod)
        {
            MenuCommand d = new MenuCommand(text, gesture, executeMethod);
            this[commandName] = d;
            return d;
        }

        /// <summary>
        /// Remove a command from the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        public void RemoveItem(string commandName)
        {
            this.Remove(commandName);
        }
    }
}
