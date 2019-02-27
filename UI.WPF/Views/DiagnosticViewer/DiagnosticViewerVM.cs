using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using System.Xml.Linq;
using DevExpress.Mvvm;
using PLEXOS.Core;
using PLEXOS.Core.Helpers;
using PLEXOS.DataModel;
using System.Text.RegularExpressions;
using System.IO;
using DevExpress.Mvvm.POCO;

namespace PLEXOS.UI.Views.DiagnosticViewer
{
    public class DiagnosticViewerVM : NotifyPropertyChanged,ISupportServices
    {

        #region Events / Delegates
        #endregion
        #region Fields 
        private XmlDocument m_Result;
        #endregion
        #region Constructors
        public DiagnosticViewerVM()
        {
            LoadCmd = new DelegateCommand(() => Load());
            SaveCmd = new DelegateCommand(() => Save());
        }

       
        #endregion
        #region Commands   
        public DelegateCommand LoadCmd
        {
            get;private set;
        }
        public DelegateCommand AddItemsCmd
        {
            get; private set;
        }
        public DelegateCommand SaveCmd
        {
            get; private set;
        }
        
        #endregion
        #region Properties

        public DataTable DiagnosticFile
        {
            get
            {
                return GetPropertyValue<DataTable>();
            }
            private set
            {
                if(GetPropertyValue<DataTable>() != value)
                {
                    SetPropertyValue<DataTable>(value);
                }
            }

        }

        public XmlDocument DiagnosticXML
        {
            get
            {
                return GetPropertyValue<XmlDocument>();
            }
            private set
            {
                if(GetPropertyValue<XmlDocument>() != value)
                {
                    SetPropertyValue<XmlDocument>(value);
                }
            }

        }
        protected IOpenFileDialogService OpenFileDialogService { get { return ServiceContainer.GetService<IOpenFileDialogService>(); } }
        protected ISaveFileDialogService SaveFileDialogService { get { return ServiceContainer.GetService<ISaveFileDialogService>(); } }

        IServiceContainer serviceContainer = null;
        protected IServiceContainer ServiceContainer
        {
            get
            {
                if(serviceContainer == null)
                    serviceContainer = new ServiceContainer(this);
                return serviceContainer;
            }
        }
        IServiceContainer ISupportServices.ServiceContainer { get { return ServiceContainer; } }

       
        public string FileName
        {
            get
            {
                return GetPropertyValue<string>();
            }
            private set
            {
                if(GetPropertyValue<string>() != value)
                {
                    SetPropertyValue<string>(value);
                }
            }
        }
        #endregion
        #region Methods     
        private void Load()
        {           
            if(OpenFileDialogService.ShowDialog())
            {

                Uri myXmlFile = new Uri(OpenFileDialogService.GetFullFileName());

                XPathDocument myXPathDoc = new XPathDocument(myXmlFile.LocalPath);

                DiagnosticXML = new XmlDocument();
                DiagnosticXML.Load(myXmlFile.LocalPath);
                string href = string.Empty;
                XmlNode pi = DiagnosticXML.SelectSingleNode("processing-instruction('xml-stylesheet')");
                if(pi != null)
                {
                    XmlElement piEl = (XmlElement)DiagnosticXML.ReadNode(XmlReader.Create(new StringReader("<pi " + pi.Value + "/>")));
                    href = piEl.GetAttribute("href");                                      
                }
                else
                {
                    Console.WriteLine("StyleSheet pi not found.");
                    return;
                }
                FileName = new FileInfo(myXmlFile.LocalPath).Name;
                Uri myStyleSheet = new Uri(href);
                if(!File.Exists(myStyleSheet.LocalPath))
                {
                    Console.WriteLine("StyleSheet File Not Found.");
                    return;
                }
                XslCompiledTransform myXslTrans = new XslCompiledTransform();
                myXslTrans.Load(myStyleSheet.LocalPath);

                m_Result = new XmlDocument();
                using(XmlWriter xw = m_Result.CreateNavigator().AppendChild())
                {
                    myXslTrans.Transform(myXPathDoc, null, xw);
                    xw.Close();
                }
                ToDataTable();
            }
           


        }
        private void Save()
        {
            if(SaveFileDialogService.ShowDialog())
            {
                DiagnosticXML.DocumentElement.RemoveAll();
                foreach(DataRow Dr in DiagnosticFile.Rows)
                {
                    XmlElement NewNode = DiagnosticXML.CreateElement("r");
                    foreach(DataColumn Dc in DiagnosticFile.Columns)
                    {
                        XmlAttribute NewAttrib = DiagnosticXML.CreateAttribute($"c{Dc.Ordinal + 1}");
                        NewAttrib.Value = Dr[Dc].ToString();
                        NewNode.Attributes.Append(NewAttrib);
                    }
                    DiagnosticXML.DocumentElement.AppendChild(NewNode);
                }
                string filename = SaveFileDialogService.GetFullFileName();
                DiagnosticXML.Save(filename);
            }
        }
        private void AddItems()
        {            
        }
        private void ToDataTable()
        { 

            var headers = m_Result.DocumentElement.SelectNodes("//tr/th");
            DiagnosticFile = new DataTable();
            foreach(XmlNode node in headers)
            {
                if(node.Attributes.Count > 0)
                {
                    string NodeType = node.Attributes["itemtype"]?.Value;
                  
                    Type ColType = typeof(string);
                    switch(NodeType)
                    {
                        
                        case "integer":
                            ColType = typeof(int);
                            break;
                        case "double":
                            ColType = typeof(double);
                            break;
                        case "boolean":
                            ColType = typeof(bool);
                            break;
                        case "datetime":
                            ColType = typeof(DateTime);
                            break;
                        default:
                            ColType = typeof(string);
                            break;

                    }
                    DiagnosticFile.Columns.Add(node.InnerText, ColType);
                }
                else
                {
                    DiagnosticFile.Columns.Add(node.InnerText);
                }
               
            }
            List<string[]> tmpStr = new List<string[]>();
            foreach(XmlNode row in m_Result.DocumentElement.SelectNodes("//tr[td]"))
            {
                tmpStr.Add(row.SelectNodes("td").Cast<XmlNode>().Select(td => td.InnerText).ToArray());
            }

            // infer col type redundant, we will get this from the Transformation
            /*
            foreach(DataColumn Col in DiagnosticFile.Columns)
            {
                int MaxRows = 100;
                if(MaxRows > tmpStr.Count)
                {
                    MaxRows = tmpStr.Count;
                }
                Col.DataType = typeof(string);

                bool IsBool = true;
                bool IsDouble = true;
                bool IsInt = true;


                for(int i = 0; i < MaxRows; i++)
                {
                    string ValToStr = tmpStr[i][Col.Ordinal];
                    if(!ValToStr.Equals("true", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if(!ValToStr.Equals("false", StringComparison.InvariantCultureIgnoreCase))
                        {
                            IsBool = false;
                        }
                    }
                    double tstDec = -1;
                    if(!double.TryParse(ValToStr, out tstDec))
                    {
                        IsDouble = false;
                        break;
                    }
                    else
                    {
                        IsInt = IsInt && Math.Abs(tstDec % 1) <= (Double.Epsilon * 100);
                    }
                }

                if(IsBool)
                {
                    Col.DataType = typeof(bool);
                }
                else if(IsDouble)
                {
                    if(IsInt)
                    {
                        Col.DataType = typeof(int);
                    }
                    else
                    {
                        Col.DataType = typeof(double);
                    }
                }
            }
            */

            foreach(string[] row in tmpStr)
            {
                DiagnosticFile.Rows.Add(row);
            }

        }
        #endregion
    }
}
