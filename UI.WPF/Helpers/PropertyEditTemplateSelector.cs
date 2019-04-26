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
    public class CustomListTypeTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                ValueTypes _Vtypes = new ValueTypes();
                CustomList CL = item as CustomList;
                if (CL != null)
                {
                    switch (CL.ValueType.Name)
                    {
                        case DataInterface.ValueTypes.Int:
                            return element.FindResource("IntEditTemplate") as DataTemplate;
                        case DataInterface.ValueTypes.Boolean:
                            return element.FindResource("BoolEditTemplate") as DataTemplate;
                        case DataInterface.ValueTypes.Decimal:
                            return element.FindResource("DecEditTemplate") as DataTemplate;
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
    public class DynamicFieldTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null)
            {
                ValueTypes _Vtypes = new ValueTypes();
                DynamicField DF = item as DynamicField;
                if (DF != null)
                {
                    switch (DF.FieldTemplate.ValueType.Name)
                    {
                        case DataInterface.ValueTypes.Int:
                            return element.FindResource("DfltIntEditTemplate") as DataTemplate;
                        case DataInterface.ValueTypes.Boolean:
                            return element.FindResource("DfltBoolEditTemplate") as DataTemplate;
                        case DataInterface.ValueTypes.Decimal:
                            return element.FindResource("DfltDecEditTemplate") as DataTemplate;
                        case DataInterface.ValueTypes.Text:
                            return element.FindResource("DfltStringEditTemplate") as DataTemplate;
                        default:
                            return element.FindResource("DfltStringEditTemplate") as DataTemplate;
                    }
                }
            }
            return null;
        }
    }
}
