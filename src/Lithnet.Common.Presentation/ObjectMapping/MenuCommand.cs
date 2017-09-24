using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Lithnet.Common.Presentation
{
    public class MenuCommand : DelegateCommand
    {
        public MenuCommand(Action<object> executeMethod, Predicate<object> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
        }

        public MenuCommand(Action<object> executeMethod)
            : base(executeMethod)
        {
        }

        public MenuCommand(string text, Action<object> executeMethod)
            : base(executeMethod)
        {
            this.Text = text;
        }

        public MenuCommand(string text, Action<object> executeMethod, Predicate<object> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
            this.Text = text;
        }


        public MenuCommand(string text, KeyGesture gesture , Action<object> executeMethod)
            : base(executeMethod)
        {
            this.Text = text;
            this.Gesture = gesture;
        }

        public MenuCommand(string text, KeyGesture gesture, Action<object> executeMethod, Predicate<object> canExecuteMethod)
            : base(executeMethod, canExecuteMethod)
        {
            this.Text = text;
            this.Gesture = gesture;
        }

        public string Text { get; set; }

        public KeyGesture Gesture { get; set; }

        public string GestureText => this.Gesture?.GetDisplayStringForCulture(CultureInfo.CurrentCulture);
    }
}
