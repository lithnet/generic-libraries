using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;
using System.Windows;
using System.ComponentModel;
using System.Windows.Data;
using Lithnet.Common.ObjectModel;

namespace Lithnet.Common.Presentation
{
    public class ListViewModel<TViewModel, TModel> : ViewModelBase,
        ICollectionViewModel<TModel>,
        IEnumerable,
        IEnumerable<TViewModel>,
        IList<TViewModel>,
        INotifyCollectionChanged
        where TModel : class
        where TViewModel : ViewModelBase<TModel>
    {
        protected ObservableCollection<TViewModel> ViewModels { get; set; }

        private Dictionary<TModel, TViewModel> viewModelCache;

        private IEditableCollectionView editableCollectionView { get; set; }

        public delegate void ListViewCollectionChangedEvent(ListViewModelChangedEventArgs args);

        public event ListViewCollectionChangedEvent OnModelRemoving;

        public event ListViewCollectionChangedEvent OnModelRemoved;

        public event ListViewCollectionChangedEvent OnModelAdding;

        public event ListViewCollectionChangedEvent OnModelAdded;

        protected IList Models;

        private Func<TModel, TViewModel> ViewModelResolver { get; set; }

        public ListViewModel()
            : base()
        {
            this.IgnorePropertyHasChanged.Add("ViewModels");
        }

        public ListViewModel(IList<TModel> collection, Func<TModel, TViewModel> resolver)
            : this()
        {
            this.SetCollectionViewModel(collection, resolver);
        }

        public ListViewModel(IList collection, Func<TModel, TViewModel> resolver)
            : this()
        {
            this.SetCollectionViewModel(collection, resolver);
        }

        public void SetCollectionViewModel(IList<TModel> collection, Func<TModel, TViewModel> resolver)
        {
            this.SetCollectionViewModel((IList)collection, resolver);
        }

        public void SetCollectionViewModel(IList collection, Func<TModel, TViewModel> resolver)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (resolver == null)
            {
                throw new ArgumentNullException(nameof(resolver));
            }

            this.ViewModelResolver = resolver;
            this.ViewModels = new ObservableCollection<TViewModel>();
            this.viewModelCache = new Dictionary<TModel, TViewModel>();

            this.ViewModels.CollectionChanged += ViewModels_CollectionChanged;

            this.Models = (IList)collection;
            this.PopulateViewModels();
        }

        private void ViewModels_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaiseViewModelChanged(this, new PropertyChangedEventArgs("ViewModels"));

            if (this.CollectionChanged != null)
            {
                this.CollectionChanged(this, e);
            }
        }

        public override ViewModelBase Find(object o)
        {
            if (this == o)
            {
                return this;
            }

            foreach (var viewModel in this.ViewModels)
            {
                ViewModelBase vm = viewModel.Find(o);
                
                if (vm != null)
                {
                    return vm;
                }
                //else
               // {
                //    System.Diagnostics.Debug.WriteLine("Not matched " + viewModel.ToString());
               // }
            }
            
            return null;
        }

        public bool Remove(TViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.RemoveViewModel(item);
            return true;
        }

        public void Remove(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            this.RemoveModel(model);
        }

        public virtual void Add(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            this.AddViewModel(-1, model);
        }

        public void Add(TModel model, bool select)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            this.Add(model);

            if (select)
            {
                this.IsExpanded = true;
                TViewModel vm = this.GetViewModel(model);
                vm.IsExpanded = true;
                vm.IsSelected = true;
            }
        }

        public void Add(TViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            this.AddViewModel(item);
        }

        public void MoveUp(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            TViewModel vm = this.GetViewModel(model);
            this.ViewModels.MoveUp(vm);
            this.Models.MoveUp(model);
        }

        public void MoveDown(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            TViewModel vm = this.GetViewModel(model);
            this.ViewModels.MoveDown(vm);
            this.Models.MoveDown(model);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.ViewModels.GetEnumerator();
        }

        IEnumerator<TViewModel> IEnumerable<TViewModel>.GetEnumerator()
        {
            return this.ViewModels.GetEnumerator();
        }

        public int IndexOf(TViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.ViewModels.IndexOf(item);
        }

        public int IndexOf(TModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return this.Models.IndexOf(item);
        }

        public void Insert(int index, TViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("model");
            }

            this.AddViewModel(index, item);
        }

        public void RemoveAt(int index)
        {
            TViewModel vm = this.ViewModels[index];

            if (!this.Models[index].Equals(vm.Model))
            {
                throw new InvalidOperationException("The model collection is no longer consistent with the view model");
            }

            this.RemoveViewModel(vm);
        }

        public TViewModel this[int index]
        {
            get
            {
                return this.ViewModels[index];
            }
            set
            {
                this.ViewModels[index] = value;
            }
        }

        public void Clear()
        {
            this.Models.Clear();
            this.ViewModels.Clear();
            this.viewModelCache.Clear();
        }

        public bool Contains(TViewModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("model");
            }

            return this.ViewModels.Contains(item);
        }

        public bool Contains(TModel item)
        {
            if (item == null)
            {
                throw new ArgumentNullException("model");
            }

            return this.Models.Contains(item);
        }

        public void CopyTo(TViewModel[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            this.ViewModels.CopyTo(array, arrayIndex);
        }

        public void CopyTo(TModel[] array, int arrayIndex)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            this.Models.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get
            {
                return this.ViewModels.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public ListCollectionView BindingList
        {
            get
            {
                return new ListCollectionView(this.ViewModels);
            }
        }

        public ObservableCollection<TViewModel> GetNewBindingList()
        {
            ObservableCollection<TViewModel> bindingList = new ObservableCollection<TViewModel>(this);
            bindingList.CollectionChanged += bindingList_CollectionChanged;
            return bindingList;
        }

        private void bindingList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (TViewModel vm in e.NewItems)
                    {
                        this.AddViewModel(vm);
                    }

                    break;

                case NotifyCollectionChangedAction.Move:

                    TViewModel viewModel = this[e.OldStartingIndex];
                    if (viewModel != null)
                    {
                        this.Remove(viewModel);
                        this.AddViewModel(e.NewStartingIndex, viewModel);
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (TViewModel vm in e.OldItems)
                    {
                        this.RemoveViewModel(vm);
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    foreach (TViewModel vm in e.OldItems)
                    {
                        this.RemoveViewModel(vm);
                    }

                    foreach (TViewModel vm in e.NewItems)
                    {
                        this.AddViewModel(vm);
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    ObservableCollection<TViewModel> bindingList = (ObservableCollection<TViewModel>)sender;
                    this.Clear();
                    foreach (TViewModel vm in bindingList)
                    {
                        this.AddViewModel(vm);
                    }

                    break;

                default:
                    break;
            }
        }

        private void AddOrUpdateViewModelCache(TViewModel vm, TModel model)
        {
            if (viewModelCache.ContainsKey(model))
            {
                viewModelCache[model] = vm;
            }
            else
            {
                this.viewModelCache.Add(model, vm);
            }
        }

        private void PopulateViewModels()
        {
            for (int i = 0; i < this.Models.Count; i++)
            {
                this.AddViewModel(i, (TModel)this.Models[i]);
            }
        }

        private TViewModel GetViewModel(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            TViewModel viewModel;

            if (this.viewModelCache.ContainsKey(model))
            {
                viewModel = this.viewModelCache[model];
            }
            else
            {
                viewModel = this.ViewModelResolver(model);
                this.viewModelCache.Add(model, viewModel);
            }

            return viewModel;
        }

        private void AddViewModel(int index, TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            if (index == -1)
            {
                index = this.ViewModels.Count;
            }

            if (index > this.Models.Count)
            {
                index = this.Models.Count;
            }

            if (!this.Models.Contains(model))
            {
                if (this.OnModelAdding != null)
                {
                    this.OnModelAdding(new ListViewModelChangedEventArgs() { Model = model });
                }

                this.Models.Insert(index, model);

                if (this.OnModelAdded != null)
                {
                    this.OnModelAdded(new ListViewModelChangedEventArgs() { Model = model });
                }
            }

            TViewModel viewModel = this.GetViewModel(model);
            this.AddViewModel(index, viewModel);
        }

        private void AddViewModel(TViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            this.AddViewModel(-1, viewModel);
        }

        private void AddViewModel(int index, TViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (viewModel.Model == null)
            {
                throw new InvalidOperationException("The view model contained a null model");
            }

            if (index == -1)
            {
                index = this.ViewModels.Count;
            }

            this.ViewModels.Insert(index, viewModel);

            if (!this.Models.Contains(viewModel.Model))
            {
                if (this.OnModelAdding != null)
                {
                    this.OnModelAdding(new ListViewModelChangedEventArgs() { Model = viewModel.Model });
                }

                this.Models.Insert(index, viewModel.Model);

                if (this.OnModelAdded != null)
                {
                    this.OnModelAdded(new ListViewModelChangedEventArgs() { Model = viewModel.Model });
                }
            }
            this.AddOrUpdateViewModelCache(viewModel, viewModel.Model);
            this.ViewModelAdded(viewModel);
            this.SubscribeToErrors(viewModel);
            this.RaiseErrorChanged(null);
        }

        private void RemoveModel(TModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            TViewModel viewModel = this.GetViewModel(model);
            this.RemoveViewModel(viewModel);
        }

        private void RemoveViewModel(TViewModel viewModel)
        {
            if (viewModel == null)
            {
                throw new ArgumentNullException(nameof(viewModel));
            }

            if (this.Models.Contains(viewModel.Model))
            {
                if (this.OnModelRemoving != null)
                {
                    this.OnModelRemoving(new ListViewModelChangedEventArgs() { Model = viewModel.Model });
                }

                this.Models.Remove(viewModel.Model);

                if (this.OnModelRemoved != null)
                {
                    this.OnModelRemoved(new ListViewModelChangedEventArgs() { Model = viewModel.Model });
                }
            }

            this.ViewModels.Remove(viewModel);
            this.viewModelCache.Remove(viewModel.Model);
            this.ViewModelRemoved(viewModel);
            //this.UnsubscribeFromHasChanged(viewModel);
            this.UnsubscribeFromErrors(viewModel);
            this.RaiseErrorChanged(null);
        }

        protected virtual void ViewModelAdded(TViewModel viewModel)
        {
            viewModel.ParentCollection = this;
        }

        protected virtual void ViewModelRemoved(TViewModel viewModel)
        {
            viewModel.ParentCollection = null;
        }

        public override void Paste()
        {
            try
            {
                TModel clipboardModel = ClipboardManager.GetObjectFromClipBoard(this.PasteableTypes) as TModel;

                if (clipboardModel != null)
                {
                    this.OnPasting(clipboardModel);
                    this.Add(clipboardModel, true);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Could not paste the item. " + ex.Message);
            }
        }
    }
}