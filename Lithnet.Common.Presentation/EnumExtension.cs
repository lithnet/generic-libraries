using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Markup;
using System.Windows.Controls;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Common.Presentation
{
    public class EnumExtension : MarkupExtension
    {
        private readonly Type enumType;

        public EnumExtension(Type enumType)
        {
            this.enumType = enumType;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var enumValues = Enum.GetValues(this.enumType);
            return (
               from object enumValue in enumValues
               select new EnumMember
               {
                   Value = enumValue,
                   Description = ((Enum)enumValue).GetEnumDescription()
               }).ToArray();
        }

        public class EnumMember
        {
            public string Description { get; set; }
            public object Value { get; set; }

            public override string ToString()
            {
                return this.Description;
            }
        }
    }
}
