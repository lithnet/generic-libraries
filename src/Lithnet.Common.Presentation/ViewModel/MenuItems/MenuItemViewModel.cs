using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using PropertyChanged;

namespace Lithnet.Common.Presentation
{
    [ImplementPropertyChanged]
    public class MenuItemViewModel : MenuItemViewModelBase
    {
        public DelegateCommand Command { get; set; }

        public BitmapImage Icon { get; set; }

        public MenuItemViewModel()
        {
            this.MenuItems = new ObservableCollection<MenuItemViewModelBase>();
        }

        public string Header { get; set; }

        public ObservableCollection<MenuItemViewModelBase> MenuItems { get; private set; }

        public KeyGesture Gesture { get; set; }

        public string GestureText => this.Gesture?.GetDisplayStringForCulture(CultureInfo.CurrentUICulture);
    }
}
