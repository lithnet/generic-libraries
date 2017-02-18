using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Collections;
using System.Collections.Specialized;

namespace Lithnet.Common.Presentation
{
    public class CompositeCollectionConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            CompositeCollection collection = new CompositeCollection();

            foreach (var item in values)
            {
                if (item == null)
                {
                    continue;
                }
                
                if (item is IEnumerable)
                {
                    collection.Add(new CollectionContainer()
                    {
                        Collection = item as IEnumerable
                    });
                }
                else
                {
                    collection.Add(item);
                }
            }

            return collection;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
