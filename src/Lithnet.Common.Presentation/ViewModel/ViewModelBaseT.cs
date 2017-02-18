using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Common.Presentation
{
    public abstract class ViewModelBase<TModel> : ViewModelBase where TModel : class
    {
        private ICollectionViewModel<TModel> parentCollection;

        public ViewModelBase()
        :base()
        {
            this.Commands.AddItem("MoveUp", t => this.MoveUp(), t => this.CanMoveUp());
            this.Commands.AddItem("MoveDown", t => this.MoveDown(), t => this.CanMoveDown());
        }

        public ViewModelBase(TModel model)
            : this()
        {
            this.SetUINotifyModel(model);
        }

        public TModel Model { get; set; }

        public ICollectionViewModel<TModel> ParentCollection
        {
            get
            {
                return this.parentCollection;
            }
            set
            {
                this.parentCollection = value;
                this.Parent = value as ViewModelBase;
            }
        }

        protected void SetUINotifyModel(TModel model)
        {
            this.Model = model;

            if (this.Model is UINotifyPropertyChanges)
            {
                this.SubscribeToErrors(model as UINotifyPropertyChanges);
            }
        }

        public void MoveUp()
        {
            if (this.ParentCollection != null)
            {
                this.ParentCollection.MoveUp(this.Model);
            }
        }

        protected virtual bool CanMoveUp()
        {
            return false;
        }

        protected virtual bool CanMoveDown()
        {
            return false;
        }

        public void MoveDown()
        {
            if (this.ParentCollection != null)
            {
                this.ParentCollection.MoveDown(this.Model);
            }
        }

        public override void Copy()
        {
            try
            {
                ClipboardManager.CopyToClipboard(this.Model, this.ClipBoardIdentifier);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Could not copy object. " + ex.Message);
            }
        }

        public override void Cut()
        {
            try
            {
                ClipboardManager.CopyToClipboard(this.Model, this.ClipBoardIdentifier);
                this.ParentCollection.Remove(this.Model);
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show("Could not cut object. " + ex.Message);
            }
        }

        public override bool CanCut()
        {
            if (this.ParentCollection != null)
            {
                return base.CanCut();
            }
            else
            {
                return false;
            }
        }

        public override ViewModelBase Find(object o)
        {
            if (o == this.Model)
            {
                return this;
            }
            else
            {
                //if (this.Model != null)
                //{
                //    System.Diagnostics.Debug.WriteLine("Not matched " + this.Model.ToString());
                //}

                return base.Find(o);
            }
        }
    }
}
