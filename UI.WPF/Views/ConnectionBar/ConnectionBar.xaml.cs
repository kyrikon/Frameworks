using UI.WPF.Views.ConnectionBar;
using System;
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

namespace UI.WPF.Views
{
    /// <summary>
    /// Interaction logic for StatusBar.xaml
    /// </summary>
    public partial class ShellConnectionBar : UserControl
    {
        private ConnectionBarVM _VM;
        public ShellConnectionBar()
        {
            InitializeComponent();
        }

        private void ListView_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            
            
        }

        private void Accordion_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
           
        }

        private void Image_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _VM.DelGrpCmd.Execute((ConnectionBarGroup)((FrameworkElement)sender).DataContext);
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            _VM = (ConnectionBarVM)this.DataContext;
        }

        private void Image_MouseDown_1(object sender, MouseButtonEventArgs e)
        {
            _VM.DelCmd.Execute((ConnectionBarItem)((FrameworkElement)sender).DataContext);
        }

        private void TextBlock_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if(e.ClickCount == 2)
            {
                Singletons.GlobalSettings.Instance.ShellContext.ConnectionChangedCmd.Execute(((ConnectionBarVM)DataContext).SelectedConnectionItem);
            }
        }
    }
}
