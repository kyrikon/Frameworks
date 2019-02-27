using Core.Converters;
using Core.Extensions;
using Core.Helpers;
using Core.Logging;
using Core.Models;
using Core.Network;
using DataInterface;
using DevExpress.Mvvm;
using DevExpress.Xpf.Editors;
using DevExpress.Xpf.WindowsUI;
using IdentityModel.Client;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using HDataObject = DataInterface.HDataObject;

namespace UI.WPF
{
    public class MainWindowVM : NotifyPropertyChanged
    {

        #region Instance Variables / Events / Delegates
        private NavigationFrame _NFrame;
        private PasswordBox _Pbox;
        private Dictionary<Type, UserControl> _Views;
        private SolidColorBrush _BlackBrush = new SolidColorBrush(Colors.Black);
        private SolidColorBrush _RedBrush = new SolidColorBrush(Colors.Red);
        private NavigationFrame _ShellNFrame;
        private Dictionary<Guid, SimProjectVM> _Projects;

        private const string AppSecret = "CC703B5F-D24F-46E1-92D5-1335FF3A610B";
        #endregion
        #region Constructors
        public MainWindowVM(NavigationFrame NF)
        {

            _NFrame = NF;
            _Projects = new Dictionary<Guid, SimProjectVM>();
             Message = "Loading";
            IsAuthenticating = true;

            //load settings
            Task.Run(() => this.LoadSettings()).Wait();

            //Init User
            CurrUser = new User();
            //CurrUser.IdentServerChanged += CurrUser_IdentServerChanged;
            CurrUser.UserAuthName = @"kyriacos.kontozis@energyexemplar.com";
            APIs = new List<APIDetails>();
            //Init views 
            _Views = new Dictionary<Type, UserControl>();

            //Init Commands
            _NFrame = NF;
            
            AuthenticateCmd = new DelegateCommand(() =>  this.Authenticate().GetAwaiter());
            LogoutCmd = new DelegateCommand(() => this.Logout());
            //init server list
            IdentServers = new List<string>();

            this.PopulateServers();

            //Init Logging 
            GlobalLogging.Instance.AppLog.AppLogItems.CollectionChanged += AppLogItems_CollectionChanged;
            GlobalLogging.Instance.ProgressValChanged += Instance_ProgressValChanged;
            GlobalLogging.Instance.MaxProgressValChanged += Instance_MaxProgressValChanged;
            GlobalLogging.Instance.BarStyleChanged += Instance_BarStyleChanged;
            GlobalLogging.Instance.ShowProgressChanged += Instance_ShowProgressChanged; ;

            BarStyleSetting = new ProgressBarStyleSettings() ;
            ShowProgress = false;
            //Set Login as main Frame
            Login LI = new Login();
            LI.DataContext = this;

            MsgColor = _BlackBrush;
            IsAuthenticating = false;

            _Views.Add(typeof(UI.WPF.Views.Settings.Blank), new UI.WPF.Views.Settings.Blank() { });
           

            _Views.Add(typeof(AppSettings), new AppSettings() { DataContext = this });

            //Kyri nav to Login
            _Pbox = LI.PBox;
            _Views.Add(LI.GetType(), LI);

            //   _NFrame.Navigate(_Views[typeof(Login)]);

            //Register Project Views
            _Views.Add(typeof(CreateProject), new CreateProject());
            _Views.Add(typeof(ProjectMainView), new ProjectMainView());
                     
            SendMsgCmd = new DelegateCommand(() => SendMsg());
            SendClientMsgCmd = new DelegateCommand(() => SendClientMsg());
            SendQuestionMsgCmd = new DelegateCommand(() => SendQuestionMsg());
            ConnectionChangedCmd = new DelegateCommand(() => ConnectionChanged());
            NewProjectCmd = new DelegateCommand(() => NewProject());
            NavigateProjectCmd  = new DelegateCommand(() => NavigateProject());
            AppSettingsCmd = new DelegateCommand(() => AppSettings());
            NavBackCmd = new DelegateCommand(() => NavBack());
            Task x = Authenticate();
            ConnectionBar = new ConnectionBarVM();            
        }       

        #endregion
        #region Commands   
        public DelegateCommand AuthenticateCmd
        {
            get;
            private set;
        }
        public DelegateCommand LogoutCmd
        {
            get;
            private set;
        }
        public DelegateCommand StartHubCmd
        {
            get;
            private set;
        }
        public DelegateCommand NavGDCmd
        {
            get;
            private set;
        }
        public DelegateCommand NavGXCmd
        {
            get;
            private set;
        }
        public DelegateCommand SendMsgCmd
        {
            get;
            private set;
        }
        public DelegateCommand SendClientMsgCmd
        {
            get;
            private set;
        }
        public DelegateCommand SendQuestionMsgCmd
        {
            get;
            private set;
        }
        public DelegateCommand GetGraphCmd
        {
            get;
            private set;
        }
        public DelegateCommand ConnectionChangedCmd
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
        public DelegateCommand NavBackCmd
        {
            get;
            private set;
        } 

        #endregion
        #region Properties

        public List<string> IdentServers
        {
            get
            {
                return GetPropertyValue<List<string>>();
            }
            set
            {
                SetPropertyValue<List<string>>(value);
            }
        }

        public SimProjectVM CurrentProject
        {
            get;set;
        }
        public List<APIDetails> APIs
        {
            get
            {
                return GetPropertyValue<List<APIDetails>>();
            }
            set
            {
                SetPropertyValue<List<APIDetails>>(value);
            }
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

        public SolidColorBrush MsgColor
        {
            get
            {
                return GetPropertyValue<SolidColorBrush>();
            }
            set
            {
                if (GetPropertyValue<SolidColorBrush>() != value)
                {
                    SetPropertyValue<SolidColorBrush>(value);
                }
            }
        }

        public string Message
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
        public string PushNotification
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

        public bool IsAuthenticating
        {
            get
            {
                return GetPropertyValue<bool>();
            }
            set
            {
                if (GetPropertyValue<bool>() != value)
                {
                    SetPropertyValue<bool>(value);
                }
            }
        }

        public PLEXOSClient SocketClient
        {
            get
            {
                return GetPropertyValue<PLEXOSClient>();
            }
            set
            {
                if (GetPropertyValue<PLEXOSClient>() != value)
                {
                    SetPropertyValue<PLEXOSClient>(value);
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

        #endregion
        #region Methods   
        private async Task Authenticate()
        {
            MsgColor = _BlackBrush;
            IsAuthenticating = true;
            Message = "Connecting to Identity server";

            //var disco = await DiscoveryClient.GetAsync(CurrUser.IdentServer);
            //var tokenClient = new TokenClient(disco.TokenEndpoint, "PLEXOS", AppSecret);

            Message = "Signing in";
            //var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(CurrUser.UserAuthName, _Pbox.Password, CurrUser.API.Name);
            //if (tokenResponse.IsError)
            //{
            //    //  rslt.Text = tokenResponse.Error;
            //    MsgColor = _RedBrush;
            //    Message = "Invalid Credentials";
            //    IsAuthenticating = false;
            //    return;
            //}
            //CurrUser.UserToken = tokenResponse.AccessToken;

            //get seleced API Endpoint
    //        Message = "Getting Notification settings";
         //   await GetNotificationEndpoint();

           
            MsgColor = _BlackBrush;


            Message = "Authentication Successful";
            System.Threading.Thread.Sleep(1000);
            IsAuthenticating = false;

            Shell SH = new Shell();
            _ShellNFrame = SH.ShellNavFrame;
            SH.DataContext = this;
            _NFrame.Navigate(SH);
            _ShellNFrame.Navigate(_Views[typeof(UI.WPF.Views.Settings.Blank)]);

            // StartClient();
            DataGridTest GD;
            if (!_Views.Any(x => x.Key == typeof(DataGridTest)))
            {
                GD = new DataGridTest();
                _Views.Add(GD.GetType(), GD);
            }

            _ShellNFrame.Navigate(_Views[typeof(DataGridTest)]);

        }
        private void PopulateServers()
        {
            
            IdentServers.Add("http://localhost:64091");
            CurrUser.IdentServer = IdentServers.FirstOrDefault();
            Message = string.Empty;
        }
        private async Task PopulateAPIs()
        {
            APIs = new List<APIDetails>();
            var client = new System.Net.Http.HttpClient();
            var builder = new UriBuilder($"{CurrUser.IdentServer}/Apiaccess/GetAPIs");
           
            var query = HttpUtility.ParseQueryString(builder.Query);
            builder.Query = query.ToString();
            string url = builder.ToString();
            var response = await client.GetAsync(url);
            string getVal = await response.Content.ReadAsStringAsync();
            APIs = JsonConvert.DeserializeObject<List<APIDetails>>(getVal);
            CurrUser.API = APIs.FirstOrDefault();           
            APIs.Add(new APIDetails() { ID = 1, Description = "Local Test" , Name = "PLEXOSGraph", EndPoint = "http://localhost:5000/" });
            
        }
        private void Logout()
        {
            Message = string.Empty;
            MsgColor = _BlackBrush;
            _NFrame.Navigate(_Views[typeof(Login)]);
        }        
        private void StartClient()
        {
            Task.Run(() =>
            {
               // SocketClient = new PLEXOSClient(Dns.GetHostName(), 11000, CurrUser.UserAuthName);
                SocketClient = new PLEXOSClient("52.230.14.8", 11000,CurrUser.UserAuthName);
                SocketClient.Connect();
                ReadReceiveQueue();
            });
          
        }
        private async Task GetNotificationEndpoint()
        {
            var client = new System.Net.Http.HttpClient();
            client.SetBearerToken(CurrUser.UserToken);
            var builder = new UriBuilder($"{CurrUser.API.EndPoint}/api/NotificationSettings");
            var query = HttpUtility.ParseQueryString(builder.Query);
            builder.Query = query.ToString();
            string url = builder.ToString();
            var response = await client.GetAsync(url);
            var getVal = await response.Content.ReadAsStringAsync();
            CurrUser.Notifications = JsonConvert.DeserializeObject<NotificationSettings>(getVal);                       
        }


        private void ReadReceiveQueue()
        {
            while (true)
            {
                if(SocketClient.ReceiveQueue.Count > 0)
                {
                    NetworkMessage Msg = SocketClient.ReceiveQueue.Dequeue();
                    //handle message
                    MsgHandler(Msg);
                }
                Thread.Sleep(100);
            }
        }
        private void SendMsg()
        {
            NetworkMessage NM = new NetworkMessage(SocketClient.ID);
            NM.MessageBody = new MessageBody() { MessageType =  MessageTypes.StringMsg, MessageSerial = "Hello" };
            SocketClient.SendQueue.Enqueue(NM);
        }
        private void SendClientMsg()
        {
            NetworkMessage NM = new NetworkMessage(SocketClient.ID);
            NM.MessageBody = new MessageBody() { MessageType = MessageTypes.StringMsg, MessageSerial = "Relay Hello" };
         //   NM.RecipientIDs = new List<Guid>(ClientConnections.Where(x => x.Value.IsChecked).Select(x => x.Key));
            SocketClient.SendQueue.Enqueue(NM);
        }
        private void SendQuestionMsg()
        {
            NetworkMessage NM = new NetworkMessage(SocketClient.ID);
            NM.MessageBody = new MessageBody() { MessageType = MessageTypes.GetDate, MessageSerial = string.Empty };
            SocketClient.SendQueue.Enqueue(NM);
        }
        private void MsgHandler(NetworkMessage Msg)
        {
            switch (Msg.MessageBody.MessageType)
            {
                case MessageTypes.ConnectMsg:
                    GlobalLogging.AddLog(LogTypes.Notifiction, "Hub Message", $"Connected to server ID {SocketClient.ServerID}. Allocated ID {SocketClient.ID}");
                    CurrUser.Notifications = new NotificationSettings();
                    CurrUser.Notifications.NotificationEndpoint = SocketClient.EndPointStr;
                    CurrUser.Notifications.Status = "Connected";
                    CurrUser.Notifications.DeviceID = SocketClient.ID.ToString();
                    break;
                    case MessageTypes.StringMsg:
                    GlobalLogging.AddLog(LogTypes.Notifiction,"Hub Message",Msg.MessageBody.MessageSerial);
                    break;
                case MessageTypes.GetClients:
                    GlobalLogging.AddLog(LogTypes.Notifiction, "Hub Message", $"Updated Client List");
                    //ClientConnections = ProcessClientConnections(Msg.MessageBody.MessageSerial);
                    break;
                case MessageTypes.GetDate:
                    DateTime GetServerDate = JsonConvert.DeserializeObject<DateTime>(Msg.MessageBody.MessageSerial);
                    GlobalLogging.AddLog(LogTypes.Notifiction, "Hub Message", $"Server Date {GetServerDate.ToString("dd/MM/yyyy hh:mm tt") }");                   
                    break;
                case MessageTypes.DBChange:
                    DBChangeData GetChange = JsonConvert.DeserializeObject<DBChangeData>(Msg.MessageBody.MessageSerial);
                    GlobalLogging.AddLog(LogTypes.Notifiction, "Hub Message", $"Data Notification Source {GetChange.DataSource} Data type {GetChange.DataType} DataKey {GetChange.DataKey}");
                    break;
                default:
                    break;
                        
            }
        }

        private Dictionary<Guid, ConnectedClientsInfo> ProcessClientConnections(string SerialStr)
        {
            Dictionary<Guid, ConnectedClientsInfo> result = new Dictionary<Guid, ConnectedClientsInfo>();
            Dictionary<Guid, string> DeserialiseItem =  JsonConvert.DeserializeObject<Dictionary<Guid, string>>(SerialStr);
            foreach (KeyValuePair<Guid, string> ClientItem in DeserialiseItem)
            {
                if (!ClientItem.Key.Equals(SocketClient.ID))
                {
                    result.Add(ClientItem.Key, new ConnectedClientsInfo() { ClientName = ClientItem.Value, IsChecked = true });
                }
            }
            return result;
        }
       

        private void ConnectionChanged()
        {
            DataGridTestVM GTVM = ((DataGridTestVM)_Views[typeof(DataGridTest)].DataContext);
            GTVM.GetItemsCmd.Execute(ClientConnection);
            _ShellNFrame.Navigate(_Views[typeof(DataGridTest)]);
            //_ShellNFrame.Navigate(_Views[typeof(ProjectMainView)]);
            //SimProjectVM GTVM = ((SimProjectVM)_Views[typeof(ProjectMainView)].DataContext);
            //GTVM.GetItemsCmd.Execute(ClientConnection);
        }
        private void NewProject()
        {
            SimProjectVM Proj = new SimProjectVM();
            CurrentProject = Proj;
            _Projects.Add(Proj.UID, Proj);

            _Views[typeof(CreateProject)].DataContext = Proj;
            _Views[typeof(ProjectMainView)].DataContext = Proj;
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
        private async void NavBack()
        {
            await WriteConfig();
            _ShellNFrame.GoBack();
        }
        
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
                Configuration["Theme"]  = (ThemeName)(Convert.ToInt32(Configuration["Theme"]));
            }

            Configuration.PropertyChanged += Configuration_PropertyChanged;
        }

        private void Configuration_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
           if(Configuration.LastFieldEdited.Equals("Theme"))
            {

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
        //private async void CurrUser_IdentServerChanged(object sender, EventArgs e)
        //{
        //    IsAuthenticating = true;
        //    Message = "Getting Available API's";
        // //   await this.PopulateAPIs();
        //    IsAuthenticating = false;
        //    Message = string.Empty;
        //}
        //volatile ConnectionStatusChangesHandler connectionStatusChangesHandler;
        //public void SetConnectionStatusChangesHandler(ConnectionStatusChangesHandler statusChangesHandler)
        //{
        //    // codes_SRS_DEVICECLIENT_28_025: [** `SetConnectionStatusChangesHandler` shall set connectionStatusChangesHandler **]**
        //    // codes_SRS_DEVICECLIENT_28_026: [** `SetConnectionStatusChangesHandler` shall unset connectionStatusChangesHandler if `statusChangesHandler` is null **]**
        //    this.connectionStatusChangesHandler = statusChangesHandler;
        //}

        private void AppLogItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            foreach(AppLogItem item in e.NewItems)
            {
                LogMsg = $"{item.LogTimeStamp.ToString("HH:mm:ss")} - {item.LogType.ToString()} - {item.LogDescription} - {item.LogDetails}" + Environment.NewLine + LogMsg;
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
