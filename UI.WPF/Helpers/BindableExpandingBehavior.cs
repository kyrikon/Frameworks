using DevExpress.Mvvm.UI.Interactivity;
using DevExpress.Xpf.Grid;
using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI.WPF.Helpers
{
    public class BindableExpandingBehavior : Behavior<TreeListControl>
    {

        public string ExpandingProperty
        {
            get { return (string)GetValue(ExpnaderProperty); }
            set { SetValue(ExpnaderProperty, value); }
        }

        public static readonly DependencyProperty ExpnaderProperty =
            DependencyProperty.Register("ExpandingProperty", typeof(string), typeof(BindableExpandingBehavior), new PropertyMetadata(string.Empty));

        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.Loaded += GridLoaded;
        }

        protected void AttachItems(TreeListNodeCollection nodes)
        {
            TreeListNodeIterator iterator = new TreeListNodeIterator(nodes);
            while (iterator.MoveNext())
            {
                this.SubscribeObject(iterator.Current.Content);
                if (iterator.Current.HasChildren)
                {
                    this.AttachItems(iterator.Current.Nodes);
                }
            }
        }

        protected void GridLoaded(object sender, RoutedEventArgs e)
        {
            if (!(this.AssociatedObject.View is TreeListView))
            {
                return;
            }

            var grid = this.AssociatedObject;
            this.AttachItems((grid.View as TreeListView).Nodes);

            INotifyCollectionChanged collection = (INotifyCollectionChanged)grid.ItemsSource;
            if (collection != null)
            {
                collection.CollectionChanged += collection_CollectionChanged;
            }

            var tree = (this.AssociatedObject.View as TreeListView);
            tree.NodeExpanded += GridNodeChanged;
            tree.NodeCollapsed += GridNodeChanged;
        }

        protected void SubscribeObject(object item)
        {
            var iPropChanged = (INotifyPropertyChanged)item;
            if (iPropChanged != null)
            {
                iPropChanged.PropertyChanged += this.PropertyChanged;
            }
        }

        void collection_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                case System.Collections.Specialized.NotifyCollectionChangedAction.Replace:
                    foreach (var item in e.NewItems)
                    {
                        this.SubscribeObject(item);
                        var node = this.FindNodeByValue(item, (this.AssociatedObject.View as TreeListView).Nodes);
                        if (node != null)
                        {
                            this.AttachItems(node.Nodes);
                        }
                    }

                    break;
                default:
                    break;
            }
        }

        protected TreeListNode FindNodeByValue(object obj, TreeListNodeCollection nodes)
        {
            TreeListNode result = null;
            TreeListNodeIterator iterator = new TreeListNodeIterator(nodes);
            while (iterator.MoveNext() && result == null)
            {
                if (iterator.Current.Content == obj)
                {
                    result = iterator.Current;
                }
                if (result == null && iterator.Current.HasChildren)
                {
                    result = this.FindNodeByValue(obj, iterator.Current.Nodes);
                }
            }

            return result;
        }

        public void PropertyChanged(object obj, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == this.ExpandingProperty)
            {
                var treeView = this.AssociatedObject.View as TreeListView;
                var node = this.FindNodeByValue(obj, treeView.Nodes);
                if (node != null)
                {
                    int rowHandle = node.RowHandle;
                    bool isExpanded = (bool)obj.GetType().GetProperty(args.PropertyName).GetValue(obj, null);
                    if (this.AssociatedObject.IsValidRowHandle(rowHandle))
                    {
                        node.IsExpanded = isExpanded;
                    }

                    while (isExpanded && node.ParentNode != null)
                    {
                        node.ParentNode.IsExpanded = isExpanded;
                        node = node.ParentNode;
                    }
                }
            }
        }

        void GridNodeChanged(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeEventArgs e)
        {
            var propInfo = e.Node.Content.GetType().GetProperty(this.ExpandingProperty);
            if (propInfo != null)
            {
                propInfo.SetValue(e.Node.Content, e.Node.IsExpanded, null);
            }
        }
    }
}
