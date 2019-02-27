using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using DataInterface;
using DevExpress.Data.Filtering.Helpers;

namespace UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for Blank.xaml
    /// </summary>
    public partial class Blank : UserControl
    {
        public Blank()
        {
            InitializeComponent();
        }

        private void DXgrdObjects_CustomRowFilter(object sender, DevExpress.Xpf.Grid.RowFilterEventArgs e)
        {
            ObservableCollection<HDynamicObject> Src = (ObservableCollection<HDynamicObject>)DXgrdObjects.ItemsSource;
            HDynamicObject row = Src[e.ListSourceRowIndex];
            ExpressionEvaluator EE = new ExpressionEvaluator(new EvaluatorContextDescriptorDefault(row.GetProperties()), DXgrdObjects.FilterCriteria);
           var rslt = EE.Evaluate(row);
        }
    }
}
