﻿using System;
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
        public void AddItem(string commandName, Action<object> executeMethod)
        {
            this[commandName] = new DelegateCommand(executeMethod);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public void AddItem(string commandName, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            this[commandName] = new DelegateCommand(executeMethod, canExecuteMethod);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public void AddItem(string commandName, string text, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            this[commandName] = new MenuCommand(text, executeMethod, canExecuteMethod);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="gesture">The input gesture that triggers the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public void AddItem(string commandName, string text, KeyGesture gesture, Action<object> executeMethod, Predicate<object> canExecuteMethod)
        {
            this[commandName] = new MenuCommand(text, gesture, executeMethod, canExecuteMethod);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public void AddItem(string commandName, string text, Action<object> executeMethod)
        {
            this[commandName] = new MenuCommand(text, executeMethod);
        }

        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="text">The display text for the command</param>
        /// <param name="gesture">The input gesture that triggers the command</param>
        /// <param name="executeMethod">The method to execute</param>
        /// <param name="canExecuteMethod">The method to execute to check if the command can be executed</param>
        public void AddItem(string commandName, string text, KeyGesture gesture, Action<object> executeMethod)
        {
            this[commandName] = new MenuCommand(text, gesture, executeMethod);
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
