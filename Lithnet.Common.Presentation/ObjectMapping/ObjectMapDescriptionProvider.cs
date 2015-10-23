using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Lithnet.Common.Presentation
{
    /// <summary>
    /// Expose the dictionary entries of a CommandMap as properties
    /// </summary>
    internal class ObjectMapDescriptionProvider: TypeDescriptionProvider
    {
        /// <summary>
        /// Standard constructor
        /// </summary>
        public ObjectMapDescriptionProvider()
            : this(TypeDescriptor.GetProvider(typeof(ObjectMap)))
        {
        }

        /// <summary>
        /// Construct the provider based on a parent provider
        /// </summary>
        /// <param name="parent"></param>
        public ObjectMapDescriptionProvider(TypeDescriptionProvider parent)
            : base(parent)
        {
        }

        /// <summary>
        /// Get the type descriptor for a given object instance
        /// </summary>
        /// <param name="objectType">The type of object for which a type descriptor is requested</param>
        /// <param name="instance">The instance of the object</param>
        /// <returns>A custom type descriptor</returns>
        public override ICustomTypeDescriptor GetTypeDescriptor(Type objectType, object instance)
        {
            return new ObjectMapDescriptor(base.GetTypeDescriptor(objectType, instance), instance as ObjectMap);
        }
    }

}
