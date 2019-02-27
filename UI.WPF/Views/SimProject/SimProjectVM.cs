using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using Core;
using Core.Helpers;
using DataInterface;
using DataSource;
using UI.WPF.Singletons;
using Core.Extensions;
using System.IO;
using System.Collections.Concurrent;
using UI.WPF.Models;
using System.Xml;
using System.Xml.Schema;
using System.Dynamic;
using UI.WPF.Helpers;
using DelegateCommand = DevExpress.Mvvm.DelegateCommand;

namespace UI.WPF.Views.SimProject
{
    public class SimProjectVM : NotifyPropertyChanged
    {

        #region Events / Delegates
        #endregion
        #region Fields 
        private Guid _UId;
        Stopwatch _SW1;
        private bool _IsNew = false;
        #endregion
        #region Constructors
        public SimProjectVM(IConnection Connection)
        {
            DM = new DataModel(DataConnectionFactory.CreateNewDataSource(Connection));
            DM.ModelInitialized += DM_ModelInitialized;
            _UId = Guid.NewGuid();
            NewDSType = DataSourceType.LocalFile;           
            ClearItemsCmd = new DelegateCommand(() => ClearItems());
            CreateCmd = new DelegateCommand(() => CreateProject().Wait());
            AddItemsCmd = new DelegateCommand(() => AddItems());
        }

        

        #endregion
        #region Commands           
        public DelegateCommand ClearItemsCmd
        {
            get; private set;
        }
        public DelegateCommand CreateCmd
        {
            get; private set;
        }        

        public DelegateCommand AddItemsCmd
        {
            get; private set;
        }
        #endregion
        #region Properties
        public DataModel DM
        {
            get;
            private set;

        }        
        public DataSourceType NewDSType
        {
            get
            {
                return GetPropertyValue<DataSourceType>();
            }
            set
            {
                if (GetPropertyValue<DataSourceType>() != value)
                {
                    SetPropertyValue<DataSourceType>(value);
                    DM.DataSource = DataConnectionFactory.CreateNewDataSource(DataConnectionFactory.CreateNewConnection(NewDSType));
                }
            }
        }
        public Guid UID
        {
            get
            {
                return _UId;
            }
        }
        public UIHKeyDictionary mylist
        {
            get
            {
                return GetPropertyValue<UIHKeyDictionary>();
            }
            private set
            {
                if (GetPropertyValue<UIHKeyDictionary>() != value)
                {
                    SetPropertyValue<UIHKeyDictionary>(value);
                }
            }

        }
        public ObservableCollection<HDynamicObject> MyRoot
        {
            get
            {
                return GetPropertyValue<ObservableCollection<HDynamicObject>>();
            }
            private set
            {
                if (GetPropertyValue<ObservableCollection<HDynamicObject>>() != value)
                {
                    SetPropertyValue<ObservableCollection<HDynamicObject>>(value);
                }
            }

        }
        public int Key
        {
            get; set;
        }
        public HDynamicObject SelectedNode
        {
            get
            {
                return GetPropertyValue<HDynamicObject>();
            }
             set
            {
                if (GetPropertyValue<HDynamicObject>() != value)
                {
                    SetPropertyValue<HDynamicObject>(value);
                    if (SelectedNode != null)
                    {
                        GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Change Selected node", $"{SelectedNode?.Name}");
                    }
                }
            }

        }
        
        public string Validation
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
        #endregion
        #region Methods     

        private void ClearItems()
        {
            if (DM.Root != null)
            {
                DM.Clear();
                MyRoot = new ObservableCollection<HDynamicObject>();
            }
        }        
        private async Task CreateProject()
        {
            _IsNew = true;
            StringBuilder ConnectionPath = new StringBuilder(GlobalSettings.Instance.ShellContext.Configuration.GetValue<string>("SavePath"));
            ConnectionPath.Append($"\\{ DM.DataSource.Connection.ConnectionName}\\{ DM.DataSource.Connection.ConnectionName}.{DM.DataSource.Connection.SaveFormat.ToString()}");
            DM.DataSource.Connection.ConnectionString = ConnectionPath.ToString();
            Tuple<bool,string> Result = DM.DataSource.ValidateNewConnection();
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Validate connection", Result.Item2);
            if (Result.Item1)
            {
                Validation = string.Empty;
                GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Creating Project");
                DataObjectHierarchy DH = new DataObjectHierarchy();
                HierarchyFactory.GenerateFinance(ref DH);                
                DM.CreateNewProject(DH);
                _SW1 = Stopwatch.StartNew();
                GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Creating New Project", $"{DM.DataSource.Connection.ConnectionName}");

                
                GlobalSettings.Instance.ShellContext.ConnectionBar.ClientConnectionGroups.FirstOrDefault(x => x.Name.Equals("Local")).ConnectionBarItems.Add(
                    new ConnectionBar.ConnectionBarItem()
                    {   GrpID = GlobalSettings.Instance.ShellContext.ConnectionBar.ClientConnectionGroups.FirstOrDefault(x => x.Name.Equals("Local")).ID,
                        ID = this.UID,
                        Connection = DM.DataSource.Connection,
                        IsFavourite = false,
                        IsSelected = true
                    });
                GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Project Ready");
                GlobalSettings.Instance.ShellContext.NavigateProjectCmd.Execute(null);
              
            }
            else
            {
                Validation = $"*{Result.Item2}";
            }
           
        }
        public async Task LoadProject()
        {
            _SW1 = Stopwatch.StartNew();            
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Loading", $"{DM.DataSource.Connection.ConnectionName}");
            if (DM != null)
            {
                ClearItems();
            }
            await DM.LoadProject();
           
        }
        private void AddItems()
        {
            // SelectedNode.IsExpanded = !SelectedNode?.IsExpanded ?? false;
            DM.Objects[new HKey(new int[] { 1, 1 })].IsExpanded = true;
            SelectedNode = DM.Objects[new HKey(new int[] { 1, 1 })];
        }
        private async void DM_ModelInitialized(object sender, EventArgs args)
        {
            MyRoot = new ObservableCollection<HDynamicObject>((ICollection<HDynamicObject>)DM.Root.Values);
            _SW1.Stop();
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Loading Complete", $"{DM.Objects.Count} Objects added in {_SW1.Elapsed.TotalSeconds} seconds");
            if (_IsNew)
            {
                await DM.Save();
            }
            _IsNew = false;            
            SelectedNode = MyRoot.FirstOrDefault();
        }
        #endregion
    }
   
}
