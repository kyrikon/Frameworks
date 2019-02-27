using DataInterface;
using System.Windows;
using System.Windows.Controls;

namespace UI.WPF.Helpers
{
    public class ConnectionTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is IConnection)
            {
                IConnection ConItem = item as IConnection;
                switch(ConItem.DSType)
                {
                    case DataSourceType.LocalFile:
                        return element.FindResource("LocalFileTemplate") as DataTemplate;
                    case DataSourceType.MSSQL:
                        return element.FindResource("MSSQLTemplate") as DataTemplate;                       
                }
               
            }
            return null;
        }
    }
}
