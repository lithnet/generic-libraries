﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using Lithnet.Common.ObjectModel;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Lithnet.Common.Presentation
{
    public abstract class ViewModelBase : UINotifyPropertyChanges
    {
        public static PropertyChangedEventArgs PropertyChangedEventHandler;

        public static event PropertyChangedEventHandler ViewModelChanged;

        public static event PropertyChangedEventHandler ViewModelIsDirtySet;

        public event PropertyChangedEventHandler IsDirtySet;
        
        private HashSet<string> isDirtyPropertyNames;

        private BitmapSource displayIcon;

        private bool isCutCopyEnabled = false;

        public bool IsDirty { get; set; }

        protected ViewModelBase()
        {
            this.isDirtyPropertyNames = new HashSet<string>(StringComparer.InvariantCultureIgnoreCase);

            this.IgnorePropertyHasChanged.Add("IsSelected");
            this.IgnorePropertyHasChanged.Add("IsExpanded");
            this.IgnorePropertyHasChanged.Add("PasteableTypes");
            this.IgnorePropertyHasChanged.Add("IconProvider");

            this.Commands.AddItem("Cut", t => this.Cut(), t => this.CanCut());
            this.Commands.AddItem("Copy", t => this.Copy(), t => this.CanCopy());
            this.Commands.AddItem("Paste", t => this.Paste(), t => this.CanPaste());

            this.SetHasChangedOnPropertyChange = true;
            this.PasteableTypes = new List<Type>();

            this.PropertyChanged += this.ViewModelBase_PropertyChanged;
        }

        private void ViewModelBase_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaiseViewModelChanged(sender, e);

            if (this.isDirtyPropertyNames.Contains(e.PropertyName))
            {
                if (!this.IsDirty)
                {
                    this.IsDirty = true;
                    ViewModelBase.ViewModelIsDirtySet?.Invoke(this, e);
                    this.IsDirtySet?.Invoke(this, e);
                }
            }
        }

        public void AddIsDirtyProperty(string propertyName)
        {
            this.isDirtyPropertyNames.Add(propertyName);
        }

        public void RemoveIsDirtyPropertyName(string propertyName)
        {
            this.isDirtyPropertyNames.Remove(propertyName);
        }

        protected void RaiseViewModelChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ViewModelBase.ViewModelChanged != null)
            {
                ViewModelBase.ViewModelChanged(sender, e);
            }
        }
       
        public static IIconProvider GlobalIconProvider { get; set; }

        public IIconProvider IconProvider { get; set; }

        public BitmapSource DisplayIcon
        {
            get
            {
                if (this.displayIcon != null)
                {
                    return this.displayIcon;
                }
                else if (this.IconProvider != null)
                {
                    return this.IconProvider.GetImageForItem(this);
                }
                else if (ViewModelBase.GlobalIconProvider != null)
                {
                    return ViewModelBase.GlobalIconProvider.GetImageForItem(this);
                }
                else
                {
                    return null;
                }
            }
            set => this.displayIcon = value;
        }

        public bool IsSelected { get; set; }

        public bool IsExpanded { get; set; }

        public List<Type> PasteableTypes { get; set; }

        public ViewModelBase Parent { get; set; }

        public CommandMap Commands { get; protected set; } = new CommandMap();

        public ToolTipMap ToolTips { get; } = new ToolTipMap();

        protected virtual string ClipBoardIdentifier => null;

        public virtual bool CanCopy()
        {
            return this.isCutCopyEnabled;
        }

        public virtual bool CanPaste()
        {
            return this.PasteableTypes.Any(t => Clipboard.ContainsData(t.FullName));
        }

        public virtual bool CanCut()
        {
            return this.isCutCopyEnabled;
        }

        public virtual void Copy()
        {
        }

        public virtual void Paste() { }

        public virtual void Cut() { }

        protected virtual void OnPasting(object obj)
        {
        }

        protected void EnableCutCopy()
        {
            this.Commands.AddItem("Copy", t => this.Copy(), t => this.CanCopy());
            this.Commands.AddItem("Cut", t => this.Cut(), t => this.CanCut());
            this.isCutCopyEnabled = true;
        }

        protected virtual void RegisterChildViewModel(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            ViewModelBase vm = obj as ViewModelBase;

            if (vm != null)
            {
                vm.Parent = this;
            }

            UINotifyPropertyChanges npc = obj as UINotifyPropertyChanges;

            if (npc != null)
            {
                this.SubscribeToErrors(npc);
            }

            this.RaisePropertyChanged("ChildNodes");
        }

        protected virtual void UnregisterChildViewModel(object obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            ViewModelBase vm = obj as ViewModelBase;

            if (vm != null)
            {
                if (vm.Parent == this)
                {
                    vm.Parent = null;
                }
            }

            UINotifyPropertyChanges npc = obj as UINotifyPropertyChanges;

            if (npc != null)
            {
                this.SubscribeToErrors(npc);
            }

            this.RaisePropertyChanged("ChildNodes");
        }

        public virtual IEnumerable<ViewModelBase> ChildNodes
        {
            get
            {
                yield break;
            }
        }

        public virtual ViewModelBase Find(object o)
        {
            if (o == this)
            {
                return this;
            }
            else
            {
                foreach (var child in this.ChildNodes)
                {
                    if (child == null)
                    {
                        continue;
                    }

                    ViewModelBase vm = child.Find(o);
                    if (vm != null)
                    {
                        return vm;
                    }
                }

                return null;
            }
        }
    }
}
