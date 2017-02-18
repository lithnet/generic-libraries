using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Common.Presentation
{
    /// <summary>
    /// This class is responsible for providing custom properties to WPF - in this instance
    /// allowing you to bind to commands by name
    /// </summary>
    internal class ObjectMapDescriptor: CustomTypeDescriptor
    {
        private ObjectMap map;

        /// <summary>
        /// Store the command map for later
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="map"></param>
        public ObjectMapDescriptor(ICustomTypeDescriptor descriptor, ObjectMap map)
            : base(descriptor)
        {
            this.map = map;
        }

        /// <summary>
        /// Get the properties for this command map
        /// </summary>
        /// <returns>A collection of synthesized property descriptors</returns>
        public override PropertyDescriptorCollection GetProperties()
        {
            //TODO: See about caching these properties (need the _map to be observable so can respond to add/remove)
            PropertyDescriptor[] props = new PropertyDescriptor[this.map.Count];

            int pos = 0;

            foreach (KeyValuePair<string, object> command in this.map)
            {
                props[pos++] = new CommandPropertyDescriptor(command);
            }

            return new PropertyDescriptorCollection(props);
        }
    }
}
