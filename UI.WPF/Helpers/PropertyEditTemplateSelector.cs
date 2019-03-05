using DataInterface;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.WPF.Helpers
{
    public class PropertyEditTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate
            SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                PropertyItem PI = item as PropertyItem;
                DataInterface.ValueType ConItem = PI.Value.GetType().GetEditor();
                switch(ConItem)
                {
                    case DataInterface.ValueType.Integer:
                        return element.FindResource("IntEditTemplate") as DataTemplate;
                    case DataInterface.ValueType.Text:
                        return element.FindResource("StringEditTemplate") as DataTemplate;
                     default:
                        return   element.FindResource("StringEditTemplate") as DataTemplate;
                }
               
            }
            return null;
        }
    }
}
