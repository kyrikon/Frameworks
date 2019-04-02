using DataInterface;
using System;
using System.Windows;
using System.Windows.Controls;

namespace UI.WPF.Helpers
{
    public class PropertyEditTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                ValueTypes _Vtypes = new ValueTypes();
                PropertyItem PI = item as PropertyItem;
                if (PI != null)
                {
                    string ConItem = _Vtypes.GetValueType(PI.Value);
                    switch (ConItem)
                    {
                        case DataInterface.ValueTypes.Int:
                            return element.FindResource("IntEditTemplate") as DataTemplate;
                        case DataInterface.ValueTypes.Text:
                            return element.FindResource("StringEditTemplate") as DataTemplate;
                        default:
                            return element.FindResource("StringEditTemplate") as DataTemplate;
                    }
                }
            }
            return null;
        }
    }
    public class ObjectPageViewTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                HDynamicObject CurrObj = item as HDynamicObject;
                if (CurrObj.HID.IsRoot)
                {
                    return element.FindResource("RootTemplate") as DataTemplate;
                }
                if (CurrObj.IsContainer)
                {
                    return element.FindResource("ContainerTemplate") as DataTemplate;
                }

                return element.FindResource("ObjectTemplate") as DataTemplate;
            }
            return null;
        }
    }
}
