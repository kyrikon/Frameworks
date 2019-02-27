using DataInterface;
using DevExpress.Xpf.Grid;
using System.Windows;
using System.Windows.Controls;

namespace UI.WPF.Helpers
{
    public class TreeItemTemplate : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && container  is LightweightCellEditor)
            {
                HDynamicObject TreeItem =  ((LightweightCellEditor)container).RowData.Row as HDynamicObject;

                if (TreeItem.IsContainer) //container objects
                {
                    if (TreeItem.IsExpanded && !TreeItem.Children.IsEmpty)
                    {
                        return element.FindResource("TreeItemExpandedTemplate") as DataTemplate;
                    }
                    else
                    {
                        return element.FindResource("TreeItemTemplate") as DataTemplate;
                    }
                }
                
               
               
            }
            return null;
        }
    }
}
