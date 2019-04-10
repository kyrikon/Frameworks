using System;
using System.Collections.Generic;
using System.IO;
using System.Reactive.Linq;
using System.Linq;
using DataInterface;
using System.Threading.Tasks;
using Core.Extensions;
using System.Text;

namespace DataSource
{
    public class LocalFileDataSource : IDataSource
    {


        #region Events / Delegates      
        public event DataInitializedEventHandler DataInitializedEvent;
        #endregion
        #region Fields 
        private bool IsBson;
        #endregion
        #region Constructors
        public LocalFileDataSource(IConnection _Connection,SaveFormat _SaveFmt =  SaveFormat.Json)
        {
            Connection = _Connection;
            Connection.SaveFormat = _SaveFmt;
            Hierarchy = new DynamicObjectHierarchy();
        }
        #endregion
        #region Commands   
        #endregion
        #region Properties
        public IConnection Connection { get; private set; }
        public bool IsSelected
        {
            get; set;
        }
        public DynamicObjectHierarchy Hierarchy
        {
            get; private set;
        }
        #endregion
        #region Methods        
      
        public Tuple<bool, string> ValidateNewConnection()
        {
            if (string.IsNullOrEmpty(Connection.ConnectionString))
            {
                return new Tuple<bool, string>(true, $"Project name can't be empty");
            }
            FileInfo FI = new FileInfo(Connection.ConnectionString);
            
            if (FI.Exists)
            {
                return new Tuple<bool, string>(false, $"Project already exists");
            }
            return new Tuple<bool, string>(true, $"Project name is Valid");
        }

        public void CreateNewProject()
        {
            FileInfo FI = new FileInfo(Connection.ConnectionString);
            DirectoryInfo DI = FI.Directory;
            if (!FI.Directory.Exists)
            {
                FI.Directory.Create();               
            }
            OnDataInitialized(new DataInitializedEventEventArgs());
        }
        public async Task<Tuple<bool, string>> SaveProject(Object Objects)
        {
            ObservableConcurrentDictionary<HKey, HDynamicObject> SaveObj = (ObservableConcurrentDictionary<HKey, HDynamicObject>)Objects;
            using (FileStream fs = new FileStream(Connection.ConnectionString,FileMode.Create,FileAccess.Write,FileShare.None, 4096,  true))
            {
                switch (Connection.SaveFormat)
                {
                    case SaveFormat.Json:
                        String str = SaveObj.Values.ToList().ToJson();
                        byte[] EncStr = Encoding.ASCII.GetBytes(str);
                      
                        Task t = fs.WriteAsync(EncStr, 0, EncStr.Length);
                        t.Wait();                 
                        break;
                    case SaveFormat.Bson:
                         str = SaveObj.Values.ToList().ToBSon();
                        EncStr = Encoding.ASCII.GetBytes(str);
                        await fs.WriteAsync(EncStr, 0, EncStr.Length);                       
                        break;
                    case SaveFormat.Bin:
                        await fs.WriteAsync(SaveObj.Values.ToArray().ToBinary(), 0, SaveObj.Count);
                        break;
                }
               
                fs.Close();
            }
            return new Tuple<bool, string>(true,"Save Complete");
        }
        public async Task<IObservable<IHDynamicObject>> LoadProject()
        {
            List<HDynamicObject> GetData;
            switch (Connection.SaveFormat)
            {
                case SaveFormat.Json:
                    GetData = await ReadJson();
                    break;
                case SaveFormat.Bson:
                    GetData = await ReadBson();
                    break;
                case SaveFormat.Bin:
                    GetData = await ReadBin();
                    break;
                default:
                    GetData = new List<HDynamicObject>();
                    break;
            }
            return Observable.Create<HDynamicObject>(
                 async obs => 
                 {
                     foreach (HDynamicObject item in GetData.OrderBy(x => x.HID))
                     {
                         obs.OnNext(item);
                     }
                     obs.OnCompleted();
                     OnDataInitialized(new DataInitializedEventEventArgs());
                 });
        }
        private async Task<List<HDynamicObject>> ReadJson()
        {
            if (File.Exists(Connection.ConnectionString))
            {
                using (StreamReader sr = new StreamReader(Connection.ConnectionString))
                {
                    string contents = await sr.ReadToEndAsync();
                    sr.Close();
                    return Core.Extensions.Serialization.FromJson<List<HDynamicObject>>(contents);
                }
            }
            return new List<HDynamicObject>() { };
        }
        private async Task<List<HDynamicObject>> ReadBson()
        {
            if (File.Exists(Connection.ConnectionString))
            {
                using (StreamReader sr = new StreamReader(Connection.ConnectionString))
                {
                    string contents = await sr.ReadToEndAsync();
                    sr.Close();
                    return Core.Extensions.Serialization.FromBSon<List<HDynamicObject>>(contents, true);
                }
            }
            return new List<HDynamicObject>() { };
        }
        private async Task<List<HDynamicObject>> ReadBin()
        {
            if (File.Exists(Connection.ConnectionString))
            {
                using (FileStream sr = File.OpenRead(Connection.ConnectionString))
                {                   
                    byte[] fileBytes = new byte[sr.Length];                    
                   await sr.ReadAsync(fileBytes, 0, fileBytes.Length);
                    sr.Close();                
                    return Core.Extensions.Serialization.FromBinary<List<HDynamicObject>>(fileBytes);
                }
            }
            return new List<HDynamicObject>() { };
        }
        #endregion
        #region Callbacks
        protected void OnDataInitialized(DataInitializedEventEventArgs Args)
        {
            DataInitializedEvent?.Invoke(this, Args);
        }

        
        #endregion
    }
}
