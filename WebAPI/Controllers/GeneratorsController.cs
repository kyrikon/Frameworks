using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Net.Http;
using IdentityModel.Client;
using System.Web;
using PLEXOS.Core.Models;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using Microsoft.Azure.Graphs;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Graphs.Elements;
using System.ComponentModel;
using PLEXOS.GraphAPI.Helpers;
using PLEXOS.DataModel;

namespace PLEXOS.GraphAPI.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class GeneratorsController : Controller
    {
       

        private string _IdentToken;
        

        #region Controller actions
        // GET api/values
        [HttpGet]
        public async Task<List<DataObject>> Get()
       // public async Task<DataObject> Get()
        {


            PLEXOS.DataModel.DataModel DM = new DataModel.DataModel();
            DM.Initialize();
            return DM.Objects.Values.ToList();

            int GetAccess = await IdentityManager.GetAccessLevel(await IdentityManager.GetIdentityToken(),User);
            //if(GetAccess >  0)
            //{
            //    using (DocumentClient client = new DocumentClient(
            //     new Uri("https://plexosgraph.documents.azure.com:443/")
            //     , "0F03QP55C9IKt75oYpDAd38aVi6NbwQOR2zS0ME3gLjPFQ71QRwFtfhJFoJphx6oEqaS1X2IhmFASTt4w3Qaqg=="))
            //    {
            //        Database getGraph = await client.ReadDatabaseAsync(UriFactory.CreateDatabaseUri("PLEXOSGraphDB"));
            //        DocumentCollection graph = await client.CreateDocumentCollectionIfNotExistsAsync(
            //                                    UriFactory.CreateDatabaseUri("PLEXOSGraphDB"),
            //                                    new DocumentCollection { Id = "NEM" },
            //                                    new RequestOptions { OfferThroughput = 1000 });

            //        IDocumentQuery<Vertex> query = client.CreateGremlinQuery<Vertex>(graph, "g.V().hasLabel('Generators')");
                  
            //        while (query.HasMoreResults)
            //        {                       
            //            foreach (Vertex result in await query.ExecuteNextAsync<Vertex>())
            //            {                          
            //                DBRslt["DBID"]= new Guid(result.Id.ToString());
            //                DBRslt["Label"] = result.Label;                           
            //            }
            //        }
      
            //        query = client.CreateGremlinQuery<Vertex>(graph, "g.V().outE('Generators.Generator').inV()");
            //        int keyRes = 0;
            //        while (query.HasMoreResults)
            //        {
            //            foreach (Vertex result in await query.ExecuteNextAsync<Vertex>())
            //            {
            //                Guid getId = new Guid(result.Id.ToString());
            //                DataObject ci = new DataObject(DBRslt.ID.CreateChildKey(++keyRes));
            //                ci["DBID"] = getId;
            //                ci["Label"] = result.Label;

            //                foreach (VertexProperty item in result.GetVertexProperties())
            //                {
            //                    ci[item.Key] = item.Value;                               
            //                }
            //                DBRslt.Children.TryAdd(ci.ID, ci);
            //            }
            //        }
            //    }
            //}
           
            //return DBRslt;
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {

            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
        #endregion

        #region Methods
        
        
        #endregion
    }
}
