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
using System.Windows.Media.Imaging;

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
            CancelCmd = new DelegateCommand(() => GlobalSettings.Instance.ShellContext.NavBack());
            MoveUpCmd = new DelegateCommand<HDynamicObject>((x) => MoveUp(x));
            MoveDownCmd = new DelegateCommand<HDynamicObject>((x) => MoveDown(x));
            AddFldrCmd = new DelegateCommand(() => AddFldr());
            DelFldrCmd = new DelegateCommand(() => DelFldr());
            AddCustomListItemCmd = new DelegateCommand(() => AddCustomListItem());
            RemoveCustomListItemCmd = new DelegateCommand(() => RemoveCustomListItem());

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

        public DelegateCommand CancelCmd
        {
            get; private set;
        }
        public DelegateCommand AddItemsCmd
        {
            get; private set;
        }

        public DelegateCommand<HDynamicObject> MoveUpCmd
        {
            get; private set;
        }

        public DelegateCommand<HDynamicObject> MoveDownCmd
        {
            get; private set;
        }
        public DelegateCommand AddFldrCmd
        {
            get; private set;
        }
        public DelegateCommand DelFldrCmd
        {
            get; private set;
        }
        public DelegateCommand AddCustomListItemCmd
        {
            get; private set;
        }
        public DelegateCommand RemoveCustomListItemCmd
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
                        OnPropertyChanged("ObjectCnt");                        
                    }
                    OnPropertyChanged("HasSelectedNode");
                    OnPropertyChanged("SelectedNodeImage");
                   
                }                
            }
            
        }
        public ObservableCollection<HDynamicObject> Root
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

        public bool HasSelectedNode
        {
            get
            {
                return SelectedNode != null;
            }
        }
        public BitmapImage SelectedNodeImage
        {
            get
            {
                if (HasSelectedNode)
                {
                    if (SelectedNode.HID.IsRoot)
                    {
                        return App.Current.TryFindResource("RootFolderIconClosed") as BitmapImage;
                    }
                    else if(SelectedNode.IsContainer)
                    {
                        return App.Current.TryFindResource("FolderIconClosed") as BitmapImage;
                    }
                    else
                    {
                        return App.Current.TryFindResource("SaveIcon") as BitmapImage;
                    }
                }
                return  App.Current.TryFindResource("FavouritesIcon") as BitmapImage; 
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

        public int ObjectCnt
        {
            get
            {
                return DM.Objects?.Count ?? 0;
            }
        }

        public KeyValuePair<string, KeyObjectDictionary> SelectedCustomList
        {
            get
            {
                return GetPropertyValue<KeyValuePair<string, KeyObjectDictionary>>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        public string NewCustomListName
        {
            get
            {
                return GetPropertyValue<string>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        public KeyObjectDictionary ListItems
        {
            get
            {
                return GetPropertyValue<KeyObjectDictionary>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        public DataInterface.ValueType ListType
        {
            get
            {
                return GetPropertyValue<DataInterface.ValueType>();
            }
            set
            {
                SetPropertyValue(value);
            }
        }
        #endregion
        #region Methods     
        private void ClearItems()
        {
            if (DM.Root != null)
            {
                DM.Clear();
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
                DynamicObjectHierarchy DH = new DynamicObjectHierarchy();
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
            SelectedNode["hello"] = SelectedNode["hello"] == null ? 0 : ((int)SelectedNode["hello"])+1;
            SelectedNode["hello2"] = SelectedNode["hello2"] == null ? 0 : ((int)SelectedNode["hello2"]) + 1;
            SelectedNode["hello3"] = "Hello World";
        }
        private async void DM_ModelInitialized(object sender, EventArgs args)
        {            
            _SW1.Stop();
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Loading Complete", $"{DM.Objects.Count} Objects added in {_SW1.Elapsed.TotalSeconds} seconds");
            if (_IsNew)
            {
                await DM.Save();
            }
            _IsNew = false;
            Root = DM.Root;
            SelectedNode = Root.FirstOrDefault();
        }
        private void MoveDown(HDynamicObject CurrChild)
        {
            if (CurrChild != null)
            {
                int CurrIxd = CurrChild.Rank;
                HDynamicObject MoveUp = CurrChild.Parent.Children.FirstOrDefault(x => x.Rank == CurrIxd + 1);
                if (MoveUp != null)
                {
                    MoveUp.Rank = CurrIxd;
                    CurrChild.Rank = CurrIxd + 1;
                    CurrChild.Parent.NodeRankChange();
                }
            }
            else
            {
                int CurrIxd = SelectedNode.Rank;
                HDynamicObject MoveUp = SelectedNode.Parent.Children.FirstOrDefault(x => x.Rank == CurrIxd + 1);
                if (MoveUp != null)
                {
                    MoveUp.Rank = CurrIxd;
                    SelectedNode.Rank = CurrIxd + 1;
                    SelectedNode.Parent.NodeRankChange();
                }
            }
        }
        private void MoveUp(HDynamicObject CurrChild)
        {
            if (CurrChild != null)
            {
                int CurrIxd = CurrChild.Rank;
                HDynamicObject MoveDown = CurrChild.Parent.Children.FirstOrDefault(x => x.Rank == CurrIxd - 1);
                if (MoveDown != null)
                {
                    MoveDown.Rank = CurrIxd;
                    CurrChild.Rank = CurrIxd - 1;
                    CurrChild.Parent.NodeRankChange();
                }
            }
            else
            {
                int CurrIxd = SelectedNode.Rank;
                HDynamicObject MoveDown = SelectedNode.Parent.Children.FirstOrDefault(x => x.Rank == CurrIxd - 1);
                if (MoveDown != null)
                {
                    MoveDown.Rank = CurrIxd;
                    SelectedNode.Rank = CurrIxd - 1;
                    SelectedNode.Parent.NodeRankChange();
                }
            }
        }
        private void AddFldr()
        {
            HDynamicObject NewFolder = SelectedNode.NewFolder();           
            DM.Objects.TryAdd(NewFolder.HID, NewFolder);
            SelectedNode.IsExpanded = true;
            SelectedNode?.NodeRankChange();
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Folder Added", $"Key is {NewFolder.HID.StrKey}");
        }
        private void DelFldr()
        {
            HDynamicObject OldFolder = new HDynamicObject();
            if (SelectedNode.Children.Any())
            {
                GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Folder Must Be empty", $"Key - {SelectedNode.HID.StrKey}");
            }
            else
            {
                DM.Objects.TryRemove(SelectedNode.HID, out OldFolder);
                GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, "Folder removed", $"Key - {OldFolder.HID.StrKey}");
                SelectedNode = OldFolder.Parent;
            
            }           
        }
        private void AddCustomListItem()
        {         
           ((ObservableConcurrentDictionary<string, KeyObjectDictionary>)SelectedNode["CustomLists"]).TryAdd(NewCustomListName, new KeyObjectDictionary() { ValueType = ListType});
        }
        private void RemoveCustomListItem()
        {
            if (SelectedCustomList.Key != null)
            {
                KeyObjectDictionary Obj = new KeyObjectDictionary();
                ((ObservableConcurrentDictionary<string, KeyObjectDictionary>)SelectedNode["CustomLists"]).TryRemove(SelectedCustomList.Key, out Obj);
            }
        }
        
        #endregion
        #region Callbacks

        #endregion
    }

}
