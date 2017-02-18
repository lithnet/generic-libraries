using System;
using System.Windows.Input;
using System.Collections.Generic;
using System.ComponentModel;
using Lithnet.Common.Presentation;

namespace Lithnet.Common.Presentation
{
    public class ToolTipMap : ObjectMap
    {
        /// <summary>
        /// Add a named command to the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        /// <param name="executeMethod">The method to execute</param>
        public void AddItem(string propertyName, string toolTipMessage)
        {
            this[propertyName] = toolTipMessage;
        }

        /// <summary>
        /// Remove a command from the command map
        /// </summary>
        /// <param name="commandName">The name of the command</param>
        public void RemoveItem(string propertyName)
        {
            this.Remove(propertyName);
        }
    }
}
