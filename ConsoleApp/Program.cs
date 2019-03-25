using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Core.Extensions;
using DataInterface;
using DataSource;
using System.Reactive.Linq;
using System.ComponentModel;
using System.Dynamic;

namespace Testingconsoleapp
{
    class Program
    {
        static async Task Main(string[] args)
        {

            HDynamicObject TestSerial = new HDynamicObject();

            DynamicObjectTemplate dmo = new DynamicObjectTemplate();
            dmo.ValueType = DataInterface.ValueType.Text;          
            StrValidationRules IRule = (StrValidationRules)dmo.Validator.Rules;          
            IRule.MinLength = null;

            IRule.RegExpPattern = @"^[a-zA-Z]+$";
            IRule.MaxLength = 4;
            dmo.DefaultValue = "A";
            dmo.DefaultValue = "AC";
            dmo.DefaultValue = "abc".ToUpper();
            dmo.DefaultValue = "abc1".ToUpper();
            dmo.DefaultValue = "12";
            IRule.RegExpPattern = null ;
            dmo.DefaultValue = "45";
            dmo.DefaultValue = 0;
            dmo.DefaultValue = "d";
            dmo.DefaultValue = null;

            //dmo.ValueType = DataInterface.ValueType.Integer;
            //dmo.DefaultValue = 0;
            //dmo.DefaultValue = 2;
            //IntValidationRules IRule = (IntValidationRules)dmo.Validator.Rules;
            //IRule.Min = 0;
            //dmo.DefaultValue = -1;
            //IRule.Min = 1;
            //IRule.Max = 2;
            //dmo.DefaultValue = -1;
            //dmo.DefaultValue = 0;
            //dmo.DefaultValue = 1;
            //dmo.DefaultValue = 2;
            //dmo.DefaultValue = 3;
            //IRule.Nullable = true;
            //dmo.DefaultValue = null;
            //IRule.Nullable = false;
            //dmo.DefaultValue = 0;
            //dmo.DefaultValue = null;



            HKeyDynamicObjectDictionary Dict = new HKeyDynamicObjectDictionary();

          
           // Src.Connection.Connect();

         //   await Src.GetObjects().ForEachAsync((itm) => Dict.TryAdd(itm));
           //var getItems =  await  Src.GetObjects().ToList();
           // Dict.AddList(getItems);
           // byte[] SerBin = Dict.ToBinary();
           // using (Stream file = File.OpenWrite(@"Ei.Bin"))
           // {
           //     file.Write(SerBin, 0, SerBin.Length);
           // }

           // FileStream stream = File.OpenRead(@"Ei.Bin");
           // byte[] fileBytes = new byte[stream.Length];

           // stream.Read(fileBytes, 0, fileBytes.Length);
           // stream.Close();
           // HKeyDictionary cpyDict = HKeyDictionary.FromBinary(fileBytes);
            

          //  DataModel model = new DataModel();
        //    model.Initialize();
       //     model.ModelInitialized += Model_ModelInitialized;
            Console.WriteLine("Loading Data");
                      
            while(true)
            {
               
            }
        }



        private static void PropChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            Console.WriteLine(e.PropertyName);
        }

        private static void Model_ModelInitialized(object sender, EventArgs args)
        {
            Console.WriteLine("Loading Complete");
           // Help();
            while (true)
            {
             //   ParseCmd(Console.ReadLine(), (DataModel)sender);
            }
        }

        //private static void ParseCmd(string Cmd, DataModel Po)
        //{            
        //    //switch(Cmd)
        //    //{
        //    //    case "p":
        //    //        Print(Po);
        //    //        break;
        //    //    case "pc":
        //    //        PrintChildren(Po);
        //    //        break;
        //    //    case "t":
        //    //        PrintTree(Po);
        //    //        break;
        //    //    case "q":
        //    //        Environment.Exit(0);
        //    //        break;
        //    //    case "e":
        //    //      //  Edit(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "chld":
        //    //        AddChild(Po);
        //    //        break;
        //    //    case "d":
        //    // //       DeleteProp(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "u":
        //    //   //     Undo(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "l":
        //    //   //     LastVal(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "r":
        //    //        Reset(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "c":
        //    //        Commit(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "m":
        //    //        Modified(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "cs":
        //    //        ChangeSet(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "pcs":
        //    //        PropChangeSet(Po.CurrentItem.Value);
        //    //        break;
        //    //    case "n":
        //    //        Navigate(Po);
        //    //        break;
        //    //    default:
        //    //        Help();
        //    //        break;
        //    //}
        //}
        //private static void Edit(DataObject Po)
        //{
        //    Console.WriteLine($"Enter Property Name:");
        //    string key = Console.ReadLine();

        //    Console.WriteLine($"Enter Property Value:");
        //    Po[key] = Console.ReadLine();

        //}
        //private static void AddChild(DataModel Po)
        //{
        //    //Console.WriteLine($"Enter New Key Val:");
        //    //string key = Console.ReadLine();
        //    //int newKey = 0;
        //    //if(int.TryParse(key,out newKey))
        //    //{
        //    //    if(newKey > 0)
        //    //    {
        //    //        HKey DoK = Po.CurrentItem.Key.CreateChildKey(newKey);
        //    //        DataObject AddObject = new DataObject();
        //    //        if(Po.Objects.Any(x => x.Key.Equals(DoK)))
        //    //        {
        //    //            Console.WriteLine($"Child Key Exists:");
        //    //        }
        //    //        else
        //    //        {
        //    //            Po.Objects.TryAdd(DoK, AddObject);
        //    //            Po.SetCurrenItem(DoK);
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        Console.WriteLine($"Invalid Key:");
        //    //    }               
        //    //}
        //    //else
        //    //{
        //    //    Console.WriteLine($"Invalid Key:");
        //    //}
           

        //}
        //private static void DeleteProp(DataObject Po)
        //{
        //    Console.WriteLine($"Enter Property Name:");
        //    string key = Console.ReadLine();
        //    if(Po.HasKey(key))
        //    {
        //        Po.RemoveKey(key);
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Key {key} not found:");
        //    }

        //}        
        //private static void Undo(DataObject Po)
        //{
        //    if(string.IsNullOrEmpty(Po.LastFieldEdited))
        //    {
        //        Console.WriteLine($"No Changes:");
        //    }
        //    else
        //    {
                
        //        Po.Undo();
        //    }

        //}
        //private static void LastVal(DataObject Po)
        //{
        //    if(string.IsNullOrEmpty(Po.LastFieldEdited))
        //    {
        //        Console.WriteLine($"No Changes:");
        //    }
        //    else
        //    {
        //        ModifiedDataItem LastUndo = Po.GetLastUndo();
        //        if(LastUndo != null)
        //        {
        //            Console.WriteLine($"Last Field po: {Po.LastFieldEdited} Last Value {LastUndo.Value}  Last Time { DateTime.FromOADate(LastUndo.TimeStamp).ToString("dd-MM-yy HH:mm:ss ")}");
        //        }
        //    }

        //}        
        //private static void Print(DataModel Po)
        //{
        //    //Console.WriteLine($"Key = {string.Join(",", (int[])Po.CurrentItem.Key)}");
        //    //Console.WriteLine($"Properties");
        //    //foreach(string Prop in Po.CurrentItem.Value.Fields)
        //    //{
        //    //    Console.WriteLine($"{Prop} = {Po.CurrentItem.Value[Prop]}");
        //    //}
        //    //Console.WriteLine($"Children");
        //    //foreach(KeyValuePair<HKey, DataObject> vals in Po.Objects.Where(x => x.Key.Contains(Po.CurrentItem.Key) && ((int[])x.Key).Length == ((int[])Po.CurrentItem.Key).Length + 1  && !x.Key.Equals(Po.CurrentItem.Key)).OrderBy(x => x.Key))
        //    //{
        //    //    Console.WriteLine(Po.GetHierarchyNames(vals.Key));
        //    //}
        //}
        //private static void PrintChildren(DataModel Po)
        //{
        //    //Console.WriteLine($"Key = {string.Join(",", Po.CurrentItem.Key)}");
        //    //Console.WriteLine($"Properties");
        //    //foreach(string Prop in Po.CurrentItem.Value.Fields)
        //    //{
        //    //    Console.WriteLine($"{Prop} = {Po.CurrentItem.Value[Prop]}");
        //    //}
        //    //Console.WriteLine($"Children");
        //    //foreach(KeyValuePair<HKey, DataObject> vals in Po.Objects.Where(x => x.Key.Contains(Po.CurrentItem.Key) && ((int[])x.Key).Length == ((int[])Po.CurrentItem.Key).Length + 1 && !x.Key.Equals(Po.CurrentItem.Key)).OrderBy(x => x.Key))
        //    //{
        //    //    Console.WriteLine(Po.GetHierarchyNames(vals.Key));
        //    //    Console.WriteLine($"Child Properties");
        //    //    foreach(string Prop in vals.Value.Fields)
        //    //    {
        //    //        Console.WriteLine($"{Prop} = {vals.Value[Prop]}");
        //    //    }
        //    //}
        //}
        //private static void PrintTree(DataModel model)
        //{
        //    //foreach(KeyValuePair<HKey, DataObject> vals in model.Objects.OrderBy(x => x.Key))
        //    //{
        //    //    Console.WriteLine(model.GetHierarchyNames(vals.Key));
        //    //}
        //}        
        //private static void ChangeSet(DataObject Po)
        //{          
        //    Console.WriteLine($"Changed properties : {string.Join(',', Po.GetModifiedKeys())}");
        //}
        //private static void PropChangeSet(DataObject Po)
        //{
        //    Console.WriteLine($"Enter Property Name:");
        //    string key = Console.ReadLine();
        //    if(Po.HasKey(key))
        //    {
        //        foreach(var getChanges in Po.GetChangeSet(key))
        //        {
        //            Console.WriteLine($"Value : {getChanges.GetValue<string>()} - Last edited {DateTime.FromOADate(getChanges.TimeStamp).ToString("dd-MM-yy HH:mm:ss ")}");
        //        }
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Key - {key} not found");
        //    }
            
        //}
        //private static void Help()
        //{
        //    Console.WriteLine($"t  - print tree");
        //    Console.WriteLine($"p  - print properties");
        //    Console.WriteLine($"e  - edit property");
        //    Console.WriteLine($"d  - delete property");
        //    Console.WriteLine($"u  - undo last change");
        //    Console.WriteLine($"l  - last property edited");
        //    Console.WriteLine($"r  - reset changes properties");
        //    Console.WriteLine($"c  - commit changes");
        //    Console.WriteLine($"m  - print is modified");
        //    Console.WriteLine($"n  - Navigate to object");
        //    Console.WriteLine($"cs - print modified properties");
        //    Console.WriteLine($"chld - Add Child"); 
        //    Console.WriteLine($"pcs - print modified property history");            
        //    Console.WriteLine($"q  - quit");
        //}
        //private static void Reset(DataObject Po)
        //{
        //    Po.Reset();
        //}
        //private static void Commit(DataObject Po)
        //{
        //    Po.CommitChanges();
        //}
        //private static void Modified(DataObject Po)
        //{
        //    if(Po.IsModified) { 
        //    Console.WriteLine($"Is Modified");
        //    }
        //    else
        //    {
        //        Console.WriteLine($"Not Modified");
        //    }

        //}
        //private static void Navigate(DataModel Po)
        //{
        //    Console.WriteLine($"Enter key (comma separated)"); 
        //    string getinput = Console.ReadLine();
        //    string[] rslt = getinput.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
        //    if(rslt.Length > Po.Hierarchy.Count)
        //    {
        //        Console.WriteLine($"Invalid Key");
        //        return;
        //    }
        //    int[] intRes = new int[rslt.Length];
        //    int cnt = 0;
        //    foreach(string s in rslt)
        //    {
        //        intRes[cnt] = 0;
        //        int tmpInt;
        //        if(!int.TryParse(s, out tmpInt))
        //        {
        //            Console.WriteLine($"Invalid Key");
        //            return;
        //        }
        //        intRes[cnt] = tmpInt;
        //        cnt++;
        //    }
        //    HKey DK = Po.Hierarchy.CreateKey(intRes);
           
        //    //Console.WriteLine(Po.SetCurrenItem(DK));
        //}
       
    }
}
