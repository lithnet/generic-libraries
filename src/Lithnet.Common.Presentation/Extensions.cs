using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows.Controls;

namespace Lithnet.Common.Presentation
{
    public static class Extensions
    {
        public static void Invoke(this Control c, Action a)
        {
            c.Dispatcher.Invoke(DispatcherPriority.Normal, a);
        }

        public static void InvokeClose(this Window w)
        {
            w.Dispatcher.Invoke(DispatcherPriority.Normal, new Action(delegate()
            {
                w.Close();
            }));
        }

        public static T FindParent<T>(this Control t, DependencyObject child) where T : class
        {
            DependencyObject parent = VisualTreeHelper.GetParent(child);

            // Check if this is the end of the tree
            if (parent == null)
            {
                return null;
            }

            T expectedParent = parent as T;

            if (expectedParent != null)
            {
                return expectedParent;
            }
            else
            {
                //use recursion until it reaches a Window
                return t.FindParent<T>(parent as DependencyObject);
            }
        }
    }
}
