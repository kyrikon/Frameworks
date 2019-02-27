using Core.Converters;
using Core.Extensions;
using Core.Helpers;
using Core.Logging;
using Core.Models;
using Core.Network;
using DataInterface;
using DataSource;
using DevExpress.Mvvm;
using DevExpress.Xpf.Core;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Windows.Controls;
using System.Windows.Media;
using UI.WPF.Singletons;
using UI.WPF.Views;
using UI.WPF.Views.ConnectionBar;
using UI.WPF.Views.DataGrid;
using UI.WPF.Views.Settings;
using UI.WPF.Views.SimProject;
using HDynamicObject = DataInterface.HDynamicObject;
using DelegateCommand =DevExpress.Mvvm.DelegateCommand;

namespace UI.WPF
{
    public class MainWindowVM : NotifyPropertyChanged
    {

        #region Instance Variables / Events / Delegates
        private NavigationFrame _NFrame;
        private PasswordBox _Pbox;
        private Dictionary<Type, UserControl> _Views;
        private Dictionary<Guid, SimProjectVM> _Projects;
        private SolidColorBrush _BlackBrush = new SolidColorBrush(Colors.Black);
        private SolidColorBrush _RedBrush = new SolidColorBrush(Colors.Red);
        private NavigationFrame _ShellNFrame;
        private Guid _CurrentProject = Guid.Empty;      
        #endregion
        #region Constructors
        public MainWindowVM(NavigationFrame NF)
        {
            //Init Logging 
            GlobalLogging.Instance.AppLog.AppLogItems.CollectionChanged += AppLogItems_CollectionChanged;
            GlobalLogging.Instance.ProgressValChanged += Instance_ProgressValChanged;
            GlobalLogging.Instance.MaxProgressValChanged += Instance_MaxProgressValChanged;
            GlobalLogging.Instance.BarStyleChanged += Instance_BarStyleChanged;
            GlobalLogging.Instance.ShowProgressChanged += Instance_ShowProgressChanged; ;

            // set global Context to this

            GlobalSettings.Instance.ShellContext = this;
            // set the commands

            ConnectionChangedCmd = new DelegateCommand<ConnectionBarItem>((x) => ConnectionChanged(x));
            NewProjectCmd = new DelegateCommand(() => NewProject());
            NavigateProjectCmd = new DelegateCommand(() => NavigateProject());
            AppSettingsCmd = new DelegateCommand(() => AppSettings());
            SaveConfigCmd = new DelegateCommand(() => SaveConfig());
            CancelConfigCmd = new DelegateCommand(() => CancelConfig());

            //set up progress bar 
            BarStyleSetting = new ProgressBarStyleSettings();
            ShowProgress = false;

            _NFrame = NF;
            _Projects = new Dictionary<Guid, SimProjectVM>();

            GlobalLogging.AddLog(LogTypes.Status, "Load Settings");
           
            //load settings
            Task.Run(() => this.LoadSettings()).Wait();

            //initialise Shell
            GlobalLogging.AddLog(LogTypes.Status, "Initializing");
            this.Initialize();
            //Init views 

            GlobalLogging.AddLog(LogTypes.Status, "Initialization Complete");
        }

        
        #endregion
        #region Commands   

        public DelegateCommand<ConnectionBarItem> ConnectionChangedCmd
        {
            get;
            private set;
        } 
        public DelegateCommand NewProjectCmd
        {
            get;
            private set;
        }
        public DelegateCommand NavigateProjectCmd
        {
            get;
            private set;
        }
        public DelegateCommand AppSettingsCmd
        {
            get;
            private set;
        }
        public DelegateCommand CancelConfigCmd
        {
            get;
            private set;
        }
        public DelegateCommand SaveConfigCmd
        {
            get;
            private set;
        }
        
        #endregion
        #region Properties

        public SimProjectVM CurrentProject
        {
            get;set;
        }

        public User CurrUser
        {
            get
            {
                return GetPropertyValue<User>();
            }
            set
            {
                SetPropertyValue<User>(value);
            }
        }

        public string LogMsg
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                if (GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }

        public double ProgressVal
        {
            get
            {
                return GetPropertyValue<double>();
            }
            set
            {
                if(GetPropertyValue<double>() != value)
                {
                    SetPropertyValue<double>(value);
                }
            }
        }
        public double MaxProg
        {
            get
            {
                return GetPropertyValue<double>();
            }
            set
            {
                if(GetPropertyValue<double>() != value)
                {
                    SetPropertyValue<double>(value);
                }
            }
        }

        public bool ShowProgress
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if(GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }
        public BaseProgressBarStyleSettings BarStyleSetting
        {
            get
            {
                return GetPropertyValue<BaseProgressBarStyleSettings>();
            }
            set
            {
                if(GetPropertyValue<BaseProgressBarStyleSettings>() != value)
                {
                    SetPropertyValue<BaseProgressBarStyleSettings>(value);
                }
            }
        }

        public ConnectionBarVM ConnectionBar
        {
            get
            {
                return GetPropertyValue<ConnectionBarVM>();
            }
            set
            {
                if (GetPropertyValue<ConnectionBarVM>() != value)
                {
                    SetPropertyValue<ConnectionBarVM>(value);
                }
            }
        }
        public IConnection ClientConnection
        {
            get
            {
                return ConnectionBar.SelectedConnection;
            }
           
        }
        public DataObject Configuration
        {
            get;private set;
        }

        public ObservableCollection<HDynamicObject> DynObjects
        {
            get
            {
                return GetPropertyValue<ObservableCollection<HDynamicObject>>();
            }
            set
            {
                if (GetPropertyValue<ObservableCollection<HDynamicObject>>() != value)
                {
                    SetPropertyValue<ObservableCollection<HDynamicObject>>(value);
                }
            }
        }
        #endregion
        #region Methods   
        private async Task LoadSettings()
        {
            string AppSettingsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\CFSim";
            if (!File.Exists($"{AppSettingsPath}\\Settings.json"))
            {
                Configuration = new DataObject();
                Configuration["SavePath"] = $"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\\SimProjects";
                Configuration["Theme"] = ThemeName.Oscuro;
                await WriteConfig();
            }
            else
            {
                Configuration = await ReadConfig();
                Configuration.CastProps();
                SetTheme(Configuration["Theme"]);
            }

            Configuration.PropertyChanged += Configuration_PropertyChanged;
        }
        private void Initialize()
        {
            _Views = new Dictionary<Type, UserControl>();

            //blank view for navigation
            _Views.Add(typeof(UI.WPF.Views.Blank), new UI.WPF.Views.Blank() { DataContext = this });
            _Views.Add(typeof(AppSettings), new AppSettings() { DataContext = this });

            //setup Connection bar
            ConnectionBar = new ConnectionBarVM();

            //Register Project Views
            _Views.Add(typeof(CreateProject), new CreateProject());
            _Views.Add(typeof(ProjectMainView), new ProjectMainView());

            Shell SH = new Shell();
            _ShellNFrame = SH.ShellNavFrame;
            SH.DataContext = this;
            _NFrame.Navigate(SH);
            _ShellNFrame.Navigate(_Views[typeof(UI.WPF.Views.Blank)]);

        }            
        private async Task ConnectionChanged(ConnectionBarItem BarItem)
        {            
            _ShellNFrame.Navigate(_Views[typeof(ProjectMainView)]);
            if (BarItem.ID == Guid.Empty)
            {               
                SimProjectVM ProjVM = new SimProjectVM(BarItem.Connection);
                BarItem.ID = ProjVM.UID;
                _Projects.Add(ProjVM.UID, ProjVM);                               
            }
            if (!_CurrentProject.Equals(BarItem.ID))
            {
                _CurrentProject = BarItem.ID;
                _Views[typeof(ProjectMainView)].DataContext = _Projects[_CurrentProject];
                 await _Projects[_CurrentProject].LoadProject();
            }
        }
        private void NewProject()
        {
            SimProjectVM Proj = new SimProjectVM(DataConnectionFactory.CreateNewConnection(DataSourceType.LocalFile));           
            _Projects.Add(Proj.UID, Proj);
            _CurrentProject = Proj.UID;
            _Views[typeof(CreateProject)].DataContext = _Projects[_CurrentProject];
            _Views[typeof(ProjectMainView)].DataContext = _Projects[_CurrentProject];
            _ShellNFrame.Navigate(_Views[typeof(CreateProject)]);
        }
        private void NavigateProject()
        {
            _ShellNFrame.Navigate(_Views[typeof(ProjectMainView)]);
        }
        private void AppSettings()
        {            
            _ShellNFrame.Navigate(_Views[typeof(AppSettings)]);
        }
        private async void SaveConfig()
        {
            Configuration.CommitChanges();
            await WriteConfig();
            _ShellNFrame.GoBack();
        }        
        private  void CancelConfig()
        {
            Configuration.Reset();
            _ShellNFrame.GoBack();
        }        
        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Configuration.LastFieldEdited.Equals("Theme") && e.PropertyName.Equals("Item[]"))
            {
                SetTheme(Configuration["Theme"]);                              
            }
        }
        private void SetTheme(ThemeName Tn)
        {
            if (Tn == ThemeName.Oscuro)
            {
                ApplicationThemeHelper.ApplicationThemeName = "Oscuro";
            }
            else if (Tn == ThemeName.Ligero)
            {
                ApplicationThemeHelper.ApplicationThemeName = "Ligero";
            }
        }
        private async Task WriteConfig()
        {
            string AppSettingsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\CFSim";
            if (!Directory.Exists(AppSettingsPath))
            {
                Directory.CreateDirectory(AppSettingsPath);
            }          
            using (StreamWriter sw = File.CreateText($"{AppSettingsPath}\\Settings.json"))
            {
                await sw.WriteAsync(Configuration.ToJson());
                await sw.FlushAsync();
                sw.Close();
            }

        }
        private async Task<DataObject> ReadConfig()
        {
            string AppSettingsPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\CFSim";
            
            if (File.Exists($"{AppSettingsPath}\\Settings.json"))
            {               
                using (StreamReader sr = new StreamReader($"{AppSettingsPath}\\Settings.json"))
                {
                    string contents = await sr.ReadToEndAsync();
                    sr.Close();                    
                    return  Core.Extensions.Serialization.FromJson<DataObject>(contents);
                }
            }
            return new DataObject();
        }

        #endregion
        #region Event Callbacks        

        private void AppLogItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(AppLogItem item in e.NewItems)
            {
                LogMsg = $"{item.LogTimeStamp.ToString("HH:mm:ss")} - {item.LogType.ToString()} - {item.LogDescription} { (string.IsNullOrEmpty(item.LogDetails) ? string.Empty : "")} {item.LogDetails}" + Environment.NewLine + LogMsg;
            }                        
        }
        private void Instance_MaxProgressValChanged(object sender, EventArgs args)
        {
            MaxProg = GlobalLogging.Instance.MaxProgress;
        }

        private void Instance_ProgressValChanged(object sender, EventArgs args)
        {
            ProgressVal = GlobalLogging.Instance.ProgressVal;
        }
        private void Instance_BarStyleChanged(object sender, EventArgs args)
        {
            BarStyleSetting = GlobalLogging.Instance.BarStyleSetting;
        }
        private void Instance_ShowProgressChanged(object sender, EventArgs args)
        {
            ShowProgress = GlobalLogging.Instance.ShowProgress;
        }
        #endregion

    }
    [TypeConverter(typeof(EnumDescriptionTypeConverter))]
    public enum ThemeName
    {
        [Description("Oscuro")]
        Oscuro = 0,
        [Description("Ligero")]
        Ligero = 1

    }
}
