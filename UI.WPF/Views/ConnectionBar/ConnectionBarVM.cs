using Core.Extensions;
using DataInterface;
using DataSource;
using DevExpress.Mvvm;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UI.WPF.Singletons;

namespace UI.WPF.Views.ConnectionBar
{
    public class ConnectionBarVM : Core.Helpers.NotifyPropertyChanged
    {
        private string _SavePath;
        #region Constructor
        public ConnectionBarVM()
        {
           // 
            AddGrpCmd =  new DelegateCommand(() => AddGrp());
            DelGrpCmd = new DelegateCommand<ConnectionBarGroup>((x) => DelGrp(x));
            DelCmd = new DelegateCommand<ConnectionBarItem>((x) => Del(x));
            NewProjectCmd = new DelegateCommand(() => NewProject());
             _SavePath = $"{GlobalSettings.Instance.ShellContext.Configuration["SavePath"]}";
            Task.Run(() => this.LoadGroups()).Wait();
        }
      

        #endregion
        #region Commands   
        public DelegateCommand AddGrpCmd
        {
            get;
            private set;
        }
        public DelegateCommand<ConnectionBarGroup> DelGrpCmd
        {
            get;
            private set;
        }
        public DelegateCommand<ConnectionBarItem> DelCmd
        {
            get;
            private set;
        }
        public DelegateCommand NewProjectCmd
        {
            get;
            private set;
        }

        #endregion
        #region Properties
        public ObservableCollection<ConnectionBarGroup> ClientConnectionGroups
        {
            get
            {
                return GetPropertyValue<ObservableCollection<ConnectionBarGroup>>();
            }
            private set
            {
                if (GetPropertyValue<ObservableCollection<ConnectionBarGroup>>() != value)
                {
                    SetPropertyValue<ObservableCollection<ConnectionBarGroup>>(value);
                }
            }
        }
        public ConnectionBarItem SelectedConnectionItem
        {
            get
            {
                return GetPropertyValue<ConnectionBarItem>();
            }
            set
            {
                if (GetPropertyValue<ConnectionBarItem>() != value)
                {
                    SetPropertyValue<ConnectionBarItem>(value);
                }
            }
        }
        public IConnection SelectedConnection
        {
            get
            {
                return SelectedConnectionItem.Connection;
            }
            
        }        

        #endregion

        #region Methods
        public async Task AddConnectionGroup(string item)
        {
            if (ClientConnectionGroups == null)
            {
                ClientConnectionGroups = new ObservableCollection<ConnectionBarGroup>();
                ClientConnectionGroups.CollectionChanged += (s, e) => { SaveGroups(); };
            }           
            if (item.Equals("Local"))
            {
                ClientConnectionGroups.Add(await GetLocal());
            }
            else
            {
                ClientConnectionGroups.Add(new ConnectionBarGroup() { Name = item, ID = Guid.NewGuid(),IsExpanded = false });
            }
   
        }
        private async Task<ConnectionBarGroup> GetLocal()
        {
            ConnectionBarGroup CBG = new ConnectionBarGroup() { Name = "Local", ID = Guid.NewGuid(),IsExpanded = true};
           
            if (!Directory.Exists(_SavePath))
            {
                Directory.CreateDirectory(_SavePath);
            }

            DirectoryInfo Di = new DirectoryInfo(_SavePath);
            foreach(DirectoryInfo InnrInfo in Di.EnumerateDirectories())
            {
               if(InnrInfo.GetFiles($"{InnrInfo.Name}.json").Any())
                {
                    CBG.ConnectionBarItems.Add(new ConnectionBarItem()
                    {
                        GrpID = CBG.ID, Connection = new LocalFileConnection()
                        {
                            ConnectionName = InnrInfo.Name,
                            ConnectionString = InnrInfo.GetFiles($"{InnrInfo.Name}.json").FirstOrDefault().FullName,
                            DSType = DataSourceType.LocalFile,
                            SaveFormat = SaveFormat.Json
                        }
                    }
                    );
                }
            }
            return CBG;
        }
        private  void AddGrp()
        {
            AddConnectionGroup("New Group");
        }
        private void DelGrp(ConnectionBarGroup Grp)
        {
            ClientConnectionGroups.Remove(Grp);
        }
        private void Del(ConnectionBarItem Item)
        {
            ClientConnectionGroups.FirstOrDefault(x => x.ID == Item.GrpID).ConnectionBarItems.Remove(Item);
        }
        
        private void SaveGroups()
        {
            File.WriteAllText(@"Connections.json", ClientConnectionGroups.ToJson());
        }
        private async void LoadGroups()
        {
            string AppSettingsConnectionPath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\CFSim\\Connections.json";
            

            if (!File.Exists(AppSettingsConnectionPath))
            {
                AddConnectionGroup("Favourites");
                AddConnectionGroup("Local");
            }
            else
            {
                string getSettings = File.ReadAllText(AppSettingsConnectionPath);
                if (!string.IsNullOrEmpty(getSettings))
                {
                    ClientConnectionGroups = await ReadConfig(AppSettingsConnectionPath);
                    ClientConnectionGroups.CollectionChanged += (s, e) => { SaveGroups(); };
                }
                else
                {
                    AddConnectionGroup("Favourites");
                    AddConnectionGroup("Local");
                }
            }
           
        }

        private async Task<ObservableCollection<ConnectionBarGroup>> ReadConfig(string Path)
        {
            if (File.Exists(Path))
            {
                using (StreamReader sr = new StreamReader(Path))
                {
                    string contents = await sr.ReadToEndAsync();
                    sr.Close();
                    return Core.Extensions.Serialization.FromJson<ObservableCollection<ConnectionBarGroup>>(contents);
                }
            }
            return new ObservableCollection<ConnectionBarGroup>();
        }

        private void NewProject()
        {
            GlobalSettings.Instance.ShellContext.NewProjectCmd.Execute(null);
        }

        #endregion
    }
    public class ConnectionBarItem : Core.Helpers.NotifyPropertyChanged
    {
        #region Constructor
        
        #endregion
        
        #region Properties
        public IConnection Connection
        {
            get
            {
                return GetPropertyValue<IConnection>();
            }
            set
            {
                SetPropertyValue<IConnection>(value);
                if (Connection != null)
                {
                    Connection.ConnectionChangedEvent -= ConChanged;
                    Connection.ConnectionChangedEvent += ConChanged;
                }
            }
        }

        public bool IsFavourite { get; set; } = false;
        public bool IsSelected { get; set; } = false;

        public Guid GrpID { get; set; }
        [JsonIgnore]
        public Guid ID { get; set; }
        [JsonIgnore]
        public bool IsConnected 
        {
            get
            {
                return Connection.ConnectionState ==  System.Data.ConnectionState.Open;
            }
            
        }

        #endregion
        private void ConChanged(object sender, ConnectionChangedEventArgs args)
        {
            OnPropertyChanged("IsConnected");
        }
    }
    public class ConnectionBarGroup
    {
        #region Constructor
        public ConnectionBarGroup()
        {
            ConnectionBarItems = new ObservableCollection<ConnectionBarItem>();
        }
        #endregion

        #region Properties
        public string Name { get; set; }
        public Guid ID { get; set; }
        public ObservableCollection<ConnectionBarItem> ConnectionBarItems { get; private set; }

        public bool IsExpanded { get; set; }
        [JsonIgnore]
        public Uri ImageSource
        {
        get
            {
                if (Name.Equals("Favourites"))
                {
                    return new Uri("pack://application:,,/AssetsNet;component/Images/ic_star_white_18dp.png", UriKind.Absolute);
                }
                return new Uri("pack://application:,,/AssetsNet;component/Images/ic_folder_white_24dp", UriKind.Absolute);
            }
        }
        public BitmapImage ImageSourceBrush
        {

            get
            {
                if (Name.Equals("Favourites"))
                {
                   
                    return (BitmapImage)App.Current.TryFindResource("FavouritesIcon");
                }
                return (BitmapImage)App.Current.TryFindResource("FolderIconClosed");
            }
        }
        #endregion
    }
}
