using DevExpress.Xpf.Editors;
using DevExpress.Xpf.Grid;
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

namespace UI.WPF.Views.DataGrid
{
    /// <summary>
    /// Interaction logic for DataGridTest.xaml
    /// </summary>
    public partial class DataGridTest : UserControl
    {
        public DataGridTest()
        {
            InitializeComponent();
         
            DataContext = new DataGridTestVM();
        }

        private void treeList_SelectedItemChanged(object sender, DevExpress.Xpf.Grid.SelectedItemChangedEventArgs e)
        {

        }

        private void view_ShowingEditor(object sender, DevExpress.Xpf.Grid.TreeList.TreeListShowingEditorEventArgs e)
        {
            DataInterface.DataObject itm = (DataInterface.DataObject)e.Node.Content;
            if(itm.IsReadOnly)
            {
                e.Cancel = true;
            }
        }
    }
    public class DemoChildSelector : IChildNodesSelector
    {
        IEnumerable IChildNodesSelector.SelectChildren(object item)
        {
            if(item is DataInterface.DataObject)
                return (item as DataInterface.HDynamicObject).ChildrenCol;
            return null;
        }
    }
}
