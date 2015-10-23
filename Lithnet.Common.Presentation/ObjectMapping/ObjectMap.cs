using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Common.Presentation
{
    /// <summary>
    /// A map that exposes commands in a WPF binding friendly manner
    /// </summary>
    [TypeDescriptionProvider(typeof(ObjectMapDescriptionProvider))]
    public class ObjectMap : Dictionary<string, object>
    {
        /// <summary>
        /// Store the commands
        /// </summary>
        private Dictionary<string, object> commands;

        public ObjectMap()
        {
            this.commands = new Dictionary<string, object>();
        }
    }
}
