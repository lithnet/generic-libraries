using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Controls;
using System.Reflection;

namespace Lithnet.Common.Presentation
{
    public class TypeMarkupExtension : MarkupExtension
    {
        private readonly Type type;

        public TypeMarkupExtension(Type type)
        {
            this.type = type;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return GetSubclassDescriptors(this.type).ToArray();
        }

        public static IEnumerable<Type> GetSubclasses(Type type)
        {
            return Assembly.GetAssembly(type).GetTypes().Where(t => t.IsSubclassOf(type)).OrderBy(t => t.Name).ToList();
        }

        public static IEnumerable<TypeDescriptionWrapper> GetSubclassDescriptors(Type type)
        {
            IEnumerable<Type> types = TypeMarkupExtension.GetSubclasses(type);

            return types.Select(t => new TypeDescriptionWrapper(t)).OrderBy(t => t.Description);
        }
    }
}
