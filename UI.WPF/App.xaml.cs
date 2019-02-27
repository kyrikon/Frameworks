using DevExpress.Xpf.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using DevExpress.Xpf.Core.Native;
using System.Windows.Navigation;
using UI.WPF.Singletons;

namespace UI.WPF
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            RegisterThemes();
            MainWindow = new MainWindow();
            MainWindowVM VM = new MainWindowVM(((MainWindow)MainWindow).NavFrame);           
            MainWindow.DataContext = VM;
            MainWindow.Show();
        }

        private void RegisterThemes()
        {
            Theme _OscuroTheme = new Theme("Oscuro", "DevExpress.Xpf.Themes.Oscuro.v17.2", "", new Uri("pack://application:,,,/UI.WPF;component/Artwork/16/ribbon_ui_blacktheme.png", UriKind.RelativeOrAbsolute), new Uri("pack://application:,,,/UI.WPF;component/Artwork/48/ribbon_ui_blacktheme.png", UriKind.Absolute));
            _OscuroTheme.AssemblyName = "DevExpress.Xpf.Themes.Oscuro.v17.2";
            Theme.RegisterTheme(_OscuroTheme);

            Theme _LigeroTheme = new Theme("Ligero", "DevExpress.Xpf.Themes.Ligero.v17.2", "", new Uri("pack://application:,,,/UI.WPF;component/Artwork/16/ribbon_ui_blacktheme.png", UriKind.RelativeOrAbsolute), new Uri("pack://application:,,,/UI.WPF;component/Artwork/48/ribbon_ui_blacktheme.png", UriKind.Absolute));
            _LigeroTheme.AssemblyName = "DevExpress.Xpf.Themes.Ligero.v17.2";
            Theme.RegisterTheme(_LigeroTheme);
            DXGridDataController.DisableThreadingProblemsDetection = true;

        }
        
        
    }
    
}
