using Microsoft.Azure.Documents.Client;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Microsoft.Azure.Graphs.Elements;
using Microsoft.Azure.Documents;
using PLEXOS.Core.Models;
using Microsoft.Azure.Documents.ChangeFeedProcessor;
using System.Threading.Tasks;
using PLEXOS.Core.Network;

namespace PLEXOS.SocketServer
{
    public class CosmosConnector
    {
        public delegate void ChangeRecievedEventHandler(object sender, ChangeRecievedEventArgs e);
        public event ChangeRecievedEventHandler ChangeRecieved;
        public string CMDStr
        {
            get;
            set;
        }

        public CosmosConnector()
        {           
           
        }

        public async void Connect()
        {
          
            DocumentCollectionInfo PLEXOSGraphCollection = new DocumentCollectionInfo
            {
                Uri = new Uri("https://plexosgraph.documents.azure.com:443/"),
                MasterKey = "0F03QP55C9IKt75oYpDAd38aVi6NbwQOR2zS0ME3gLjPFQ71QRwFtfhJFoJphx6oEqaS1X2IhmFASTt4w3Qaqg==",
                DatabaseName = "PLEXOSGraphDB",
                CollectionName = "NEM"
            };
            DocumentCollectionInfo LeaseCollection = new DocumentCollectionInfo
            {
                Uri = new Uri("https://plexosgraph.documents.azure.com:443/"),
                MasterKey = "0F03QP55C9IKt75oYpDAd38aVi6NbwQOR2zS0ME3gLjPFQ71QRwFtfhJFoJphx6oEqaS1X2IhmFASTt4w3Qaqg==",
                DatabaseName = "PLEXOSGraphDB",
                CollectionName = "LeaseCollection"
            };
            using (DocumentClient client = new DocumentClient(PLEXOSGraphCollection.Uri, PLEXOSGraphCollection.MasterKey)){
                Database getGraph = await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri("PLEXOSGraphDB"));
                DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                                            UriFactory.CreateDatabaseUri("PLEXOSGraphDB"),
                                            new DocumentCollection { Id = "NEM" },
                                            new RequestOptions { OfferThroughput = 1000 });


                // Add changefeed
                ChangeFeedOptions opt = new ChangeFeedOptions();
                opt.StartFromBeginning = false;
            


                ChangeFeedHostOptions hostOptions = new ChangeFeedHostOptions();
                hostOptions.LeaseRenewInterval = TimeSpan.FromSeconds(15);


                string hostName = Guid.NewGuid().ToString();

                DocumentFeedObserverFactory docObserverFactory = new DocumentFeedObserverFactory(client, PLEXOSGraphCollection);
              
                ChangeFeedEventHost MyHost = new ChangeFeedEventHost(hostName, PLEXOSGraphCollection, LeaseCollection,opt,hostOptions);
                         
               
                await MyHost.RegisterObserverFactoryAsync(docObserverFactory);

                Console.WriteLine("Running Graph Listener... type stop g");

                bool RunLoop = true;

                //Task.Run(() =>
                //{
                //    while (true)
                //    {
                //        //string getCmd = Console.ReadLine();
                //        //if (getCmd.ToUpper() == "STOP G")
                //        //{
                //        //    RunLoop = false;
                //        //}
                //    }
                //});

                while (RunLoop)
                {

                    if (docObserverFactory.ChangeList.Count > 0)
                    {
                        Document doc = docObserverFactory.ChangeList.Dequeue();
                        Console.WriteLine($"{doc.Id.ToString()} - {doc.ToString()}");
                        DBChangeData CD = new DBChangeData() { DataSource = "Graph", DataType = "Vertex", DataKey = doc.Id };
                        OnChangeRecieved(new ChangeRecievedEventArgs(CD));
                    }
                   
                 
                }

                await MyHost.UnregisterObserversAsync();
                Console.WriteLine("Graph Listener stopped");
            }
        }

        public async void QueryGraph(DocumentClient client)
        {
            P_Collection Col = new P_Collection();
            DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
                                           UriFactory.CreateDatabaseUri("PLEXOSGraphDB"),
                                           new DocumentCollection { Id = "NEM" },
                                           new RequestOptions { OfferThroughput = 1000 });

            IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(graph, "g.V().hasLabel('Generators')");
            while (query.HasMoreResults)
            {
                foreach (Vertex result in await query.ExecuteNextAsync<Vertex>())
                {

                    Col.ID = new Guid(result.Id.ToString());
                    Col.Label = result.Label;
                }
            }

            query = client.CreateGremlinQuery<Vertex>(graph, "g.V().outE('Generators.Generator').inV()");
            while (query.HasMoreResults)
            {
                foreach (Vertex result in await query.ExecuteNextAsync<Vertex>())
                {
                    Guid getId = new Guid(result.Id.ToString());
                    ChildItem ci = new ChildItem();
                    ci.ID = getId;
                    ci.Label = result.Label;// Dictionary < string, string>
                    Console.WriteLine($"Generators {ci.ID} - { ci.Label}");

                    foreach (VertexProperty item in result.GetVertexProperties())
                    {

                        ci.Properties.Add(item.Key, item.Value);
                        Console.WriteLine($"    Properies {item.Key} - { item.Value}");
                    }
                    Col.Children.Add(getId, ci);
                }
            }
        }

        protected virtual void OnChangeRecieved(ChangeRecievedEventArgs e)
        {
            if (ChangeRecieved != null)
            {
                ChangeRecieved(this, e);               
            }
        }
    }
    public class ChangeRecievedEventArgs : System.EventArgs
    {
        #region instance variables

        public ChangeRecievedEventArgs(DBChangeData chng)
        {
            this.Change = chng;          
        }
        #endregion


        #region Constructors

        #endregion

        #region Properties
        public DBChangeData Change
        {
            get;
            set;
        }
     
        #endregion

    }
}
