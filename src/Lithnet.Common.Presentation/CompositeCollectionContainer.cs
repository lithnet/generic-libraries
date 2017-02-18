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
    public class CompositeCollectionContainer : CompositeCollection
    {
        public void AddItem(params object[] items)
        {
            foreach (var item in items)
            {
                if (item == null)
                {
                    continue;
                }

                if (item is IEnumerable)
                {
                    this.Add(new CollectionContainer()
                    {
                        Collection = item as IEnumerable
                    });
                }
                else
                {
                    this.Add(item);
                }
            }
        }
    }
}
