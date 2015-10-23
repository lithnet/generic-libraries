using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Common.Presentation
{
    public class TypeDescriptionWrapper
    {
        public TypeDescriptionWrapper(Type type)
        {
            this.Value = type;
            this.Description = type.GetTypeDescription();
        }

        public string Description { get; private set; }

        public Type Value { get; private set; }

        public override string ToString()
        {
            return this.Description;
        }
    }
}
