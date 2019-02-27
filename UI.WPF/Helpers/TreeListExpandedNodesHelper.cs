using DataInterface;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Layout.Core.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace UI.WPF.Helpers
{
    public static class TreeListExpandedNodesHelper
    {
        static TreeListView treeListView;
        public static readonly DependencyProperty SynchIsExpandedProperty = DependencyProperty.RegisterAttached("SynchIsExpanded", typeof(bool), typeof(TreeListExpandedNodesHelper), new UIPropertyMetadata(false, new PropertyChangedCallback(OnSynchIsExpandedChanged)));

        public static bool GetSynchIsExpanded(DependencyObject target)
        {
            return (bool)target.GetValue(SynchIsExpandedProperty);
        }
        public static void SetSynchIsExpanded(DependencyObject target, bool value)
        {
            target.SetValue(SynchIsExpandedProperty, value);
        }
        private static void OnSynchIsExpandedChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            OnSynchIsExpandedChanged(o, (bool)e.OldValue, (bool)e.NewValue);
        }
        private static void OnSynchIsExpandedChanged(DependencyObject o, bool oldValue, bool newValue)
        {
            treeListView = o as TreeListView;
            if (treeListView == null)
                return;
            if (newValue)
            {
                treeListView.NodeExpanded += View_NodeExpanded;
                treeListView.NodeCollapsed += View_NodeCollapsed;
            }
            else
            {
                treeListView.NodeExpanded -= View_NodeExpanded;
                treeListView.NodeCollapsed -= View_NodeCollapsed;
            }
        }

        static void View_NodeCollapsed(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeEventArgs e)
        {
            UpdateIsExpanded(e.Node);
        }

        static void View_NodeExpanded(object sender, DevExpress.Xpf.Grid.TreeList.TreeListNodeEventArgs e)
        {
            UpdateIsExpanded(e.Node);
        }
        private static void UpdateIsExpanded(TreeListNode node)
        {
            HDynamicObject obj = node.Content as HDynamicObject;
            obj.IsExpanded = node.IsExpanded;
        }

        public static void RegisterBaseObject(HDynamicObject obj)
        {
            obj.PropertyChanged += obj_PropertyChanged;
        }
        public static void DeRegisterBaseObject(HDynamicObject obj)
        {
            obj.PropertyChanged -= obj_PropertyChanged;
        }
        static void obj_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsExpanded")
            {
                if (treeListView != null)
                    treeListView.GetNodeByContent(sender).IsExpanded = (sender as HDynamicObject).IsExpanded;
            }

        }

    }
}
