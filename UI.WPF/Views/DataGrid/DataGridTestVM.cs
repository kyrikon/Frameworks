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
using System.Xml;
using System.Xml.Schema;
using System.Dynamic;
using DataModel = UI.WPF.Models.DataModel;
using UI.WPF.Models;
using DelegateCommand = DevExpress.Mvvm.DelegateCommand;

namespace UI.WPF.Views.DataGrid
{
    public class DataGridTestVM : NotifyPropertyChanged
    {

        #region Events / Delegates
        #endregion
        #region Fields 
        #endregion
        #region Constructors
        public DataGridTestVM()
        {
            DM = new  DataModel(DataConnectionFactory.CreateNewDataSource(DataConnectionFactory.CreateNewConnection(DataSourceType.LocalFile)));
            DM.ModelInitialized += DM_ModelInitialized;

            AddItemCmd = new DelegateCommand(() => InSpectRoot());
            AddItemsCmd = new DelegateCommand(() => DoAddItems());
            ExportItemsCmd = new DelegateCommand(() => DoExportItems());
            GetItemsCmd = new DelegateCommand<IConnection>(DS => GetItems(DS));
            ClearItemsCmd = new DelegateCommand(() => ClearItems());
        }

        #endregion
        #region Commands   
        public DelegateCommand AddItemCmd
        {
            get; private set;
        }
        public DelegateCommand AddItemsCmd
        {
            get; private set;
        }
        public DelegateCommand ExportItemsCmd
        {
            get; private set;
        }
        public DelegateCommand<IConnection> GetItemsCmd
        {
            get; private set;
        }
        public DelegateCommand ClearItemsCmd
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
        #endregion
        #region Methods     
        Stopwatch sw1;
        private async void GetItems(IConnection DC)
        {
            if (DM != null)
            {
                ClearItems();
            }
         
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add Objects", $"from {DC.ConnectionName} ");
            sw1 = Stopwatch.StartNew();           
            DM.Initialize();
        }
        private async void DoAddItems()
        {
            if (DM.DataSource.GetType() == typeof(LocalFileDataSource))
            {

                //DM.DataSource = new MSSQLDataSource();
                //GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add Objects", $"from EI ");
                //sw1 = Stopwatch.StartNew();
                //DM.Initialize();

            }
            else
            {
                //DM.DataSource = new LocalFileDataSource();
                //GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add Objects", $"from in mem ");
                //sw1 = Stopwatch.StartNew();
                //DM.Initialize();
            }
            //GlobalLogging.Instance.ShowProgress = true;
            //GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add Objects", $"start op");

            //var sw1 = Stopwatch.StartNew();
            //GlobalLogging.Instance.BarStyleSetting = new ProgressBarStyleSettings();
            //List<KeyValuePair<HKey, DataObject>> AddLst = await Task.Run(() => AddItems());
            //sw1.Stop();

            //GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add {AddLst.Count} Objects", $"creation took {sw1.Elapsed.TotalSeconds} seconds");
            //sw1 = Stopwatch.StartNew();
            //GlobalLogging.Instance.ProgressVal = 0;
            //GlobalLogging.Instance.BarStyleSetting = new ProgressBarMarqueeStyleSettings() {   };

            //await Task.Run(() => DM.Objects.AddList(AddLst));

            //DM.Objects.EndEdit(AddLst);
            //sw1.Stop();
            //Console.WriteLine($"addition took {sw1.Elapsed.TotalSeconds} seconds");
            //GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add {AddLst.Count} Objects", $"addition took {sw1.Elapsed.TotalSeconds} seconds");
            //MyRoot = new ObservableCollection<DataObject>(DM.Root.Values);
            //GlobalLogging.Instance.ShowProgress = false;
        }
        private async void DoExportItems()
        {
            //  string getObjects = DM.Objects.ToJson();            
           
            byte[] SerBin = DM.Objects.ToBinary();
            File.WriteAllBytes(@"DB.Bin", SerBin);
        }
        private void ClearItems()
        {           
            DM.Clear();
            MyRoot = new ObservableCollection<HDynamicObject>((ICollection<HDynamicObject>)DM.Root.Values);
        }
        private void InSpectRoot()
        {
            mylist.FirstOrDefault().Value["Name"] = $"{++Key}";
        }

        private void DM_ModelInitialized(object sender, EventArgs args)
        {
            MyRoot = new ObservableCollection<HDynamicObject>((ICollection<HDynamicObject>)DM.Root.Values);
            sw1.Stop();
            GlobalLogging.AddLog(Core.Logging.LogTypes.Notifiction, $"Add {DM.Objects.Count} Objects", $"creation took {sw1.Elapsed.TotalSeconds} seconds");
        }
        #endregion
    }
   
}
