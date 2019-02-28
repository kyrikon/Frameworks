using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
using DevExpress.Xpf.Grid.Themes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace UI.WPF.Views.SimProject
{
    /// <summary>
    /// Interaction logic for DataGridTest.xaml
    /// </summary>
    public partial class ProjectMainView : UserControl
    {
        public ProjectMainView()
        {
            InitializeComponent();                   
        }

        private void treeList_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {
          
        }

        private void view_ShowingEditor(object sender, DevExpress.Xpf.Grid.TreeList.TreeListShowingEditorEventArgs e)
        {
            
        }

        private void View_Loaded(object sender, RoutedEventArgs e)
        {
            view.AutoFilterRowCellStyle = GetFilterStyle();
        }
        private Style GetFilterStyle()
        {
            Style s = new Style();
            try
            {
                GridRowThemeKeyExtension themeKey = new GridRowThemeKeyExtension();


                themeKey.ResourceKey = GridRowThemeKeys.LightweightCellStyle;
                themeKey.ThemeName = ThemeManager.GetThemeName(Application.Current.MainWindow);


             //   s.BasedOn = (Style)FindResource(themeKey);
                s.TargetType = typeof(FilterCellContentPresenter);
                s.Setters.Add(new Setter(FilterCellContentPresenter.BackgroundProperty, new SolidColorBrush(Colors.White) { Opacity = 0.1 }));
                s.Setters.Add(new Setter(FilterCellContentPresenter.BorderThicknessProperty, new Thickness(0)));
                s.Setters.Add(new Setter(FilterCellContentPresenter.BorderBrushProperty, new SolidColorBrush(Colors.Transparent)));
                s.Setters.Add(new Setter(FilterCellContentPresenter.MarginProperty, new Thickness(0,2,0,5)));
                //  s.Setters.Add(new Setter(LightweightCellEditor.ForegroundProperty, New Binding("RowData.Row.values[" & index.ToString() & "].TemplateColor")));
            }
            catch (System.Windows.ResourceReferenceKeyNotFoundException)
            {

            }
            return s;        
        }
    }
    public class TreeChildSelector : IChildNodesSelector
    {
        IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            if(item is DataInterface.HDynamicObject)
                return (item as DataInterface.HDynamicObject).ChildrenCol;
            return null;
        }
    }
}
