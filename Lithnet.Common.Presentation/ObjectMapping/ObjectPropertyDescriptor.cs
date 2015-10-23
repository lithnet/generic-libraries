using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Common.Presentation
{
    /// <summary>
    /// A property descriptor which exposes an ICommand instance
    /// </summary>
    internal class CommandPropertyDescriptor : PropertyDescriptor
    {
        /// <summary>
        /// Store the command which will be executed
        /// </summary>
        private object command;

        /// <summary>
        /// Construct the descriptor
        /// </summary>
        /// <param name="command"></param>
        public CommandPropertyDescriptor(KeyValuePair<string, object> command)
            : base(command.Key, null)
        {
            this.command = command.Value;
        }

        /// <summary>
        /// Always read only in this case
        /// </summary>
        public override bool IsReadOnly
        {
            get { return true; }
        }

        /// <summary>
        /// Nope, it's read only
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool CanResetValue(object component)
        {
            return false;
        }

        /// <summary>
        /// Not needed
        /// </summary>
        public override Type ComponentType
        {
            get { throw new NotImplementedException(); }
        }

        /// <summary>
        /// Get the ICommand from the parent command map
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override object GetValue(object component)
        {
            ObjectMap map = component as ObjectMap;

            if (null == map)
                throw new ArgumentException("component is not a CommandMap instance", "component");

            return map[this.Name];
        }

        /// <summary>
        /// Get the type of the property
        /// </summary>
        public override Type PropertyType
        {
            get { return command.GetType(); }
        }

        /// <summary>
        /// Not needed
        /// </summary>
        /// <param name="component"></param>
        public override void ResetValue(object component)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not needed
        /// </summary>
        /// <param name="component"></param>
        /// <param name="value"></param>
        public override void SetValue(object component, object value)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Not needed
        /// </summary>
        /// <param name="component"></param>
        /// <returns></returns>
        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
    }
}
