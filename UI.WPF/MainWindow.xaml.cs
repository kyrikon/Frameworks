using DevExpress.Xpf.Ribbon;
using System.Windows;

namespace UI.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : DXRibbonWindow
    { 
        public MainWindow()
        {
            InitializeComponent();
        }

       

        //private async  void Button_Click(object sender, RoutedEventArgs e)
        //{
        //    if (string.IsNullOrEmpty(_IdentToken))
        //    {   
        //         var disco = await DiscoveryClient.GetAsync("https://plexosidentity.azurewebsites.net");
        //        //var disco = await DiscoveryClient.GetAsync("http://localhost:64091");
        //        var tokenClient = new TokenClient(disco.TokenEndpoint, "PLEXOS", AppSecret);
        //        var tokenResponse = await tokenClient.RequestResourceOwnerPasswordAsync(username.Text, password.Text, "PLEXOSGraph");
        //        if (tokenResponse.IsError)
        //        {
        //          //  rslt.Text = tokenResponse.Error;
        //            return;
        //        }
        //        _IdentToken = tokenResponse.AccessToken;

        //    }

        //    var client = new HttpClient();
        //   client.SetBearerToken(_IdentToken);

        //    //     var response = await client.GetAsync("http://localhost:64091/OrganisationsAPI");      
        //   // var builder = new UriBuilder(((ComboBoxItem)APIEndpoint.SelectedItem).Content.ToString()); 
        //    //if(APIEndpoint.SelectedIndex == 0)
        //    //{
        //    //    builder.Port = -1;
        //    //}
        //    //else
        //    //{
        //    //    builder.Port = 5000;
        //    //}

        //    var query = HttpUtility.ParseQueryString(builder.Query);           
        // //   builder.Query = query.ToString();
        //  //  string url = builder.ToString();
        //    var response = await client.GetAsync(url);
        //    var getVal = await response.Content.ReadAsStringAsync();
        //    _attempts++;
        //    if (!response.IsSuccessStatusCode)
        //    {
        ////        rslt.Text = $"{ _attempts }:{response.StatusCode.ToString()}";
        //    }
        //    else
        //    {
        //    //    rslt.Text = $"{ _attempts }:{ await response.Content.ReadAsStringAsync()}"; 
        //       // rslt.Text = content;
        //    }           
        //}

        //private async void Button_Click1(object sender, RoutedEventArgs e)
        //{

        //    P_Collection Col = new P_Collection();

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
        //        GuidConverter Gconvert = new GuidConverter();
        //        while (query.HasMoreResults)
        //        {
        //            foreach (Vertex result in await query.ExecuteNextAsync<Vertex>())
        //            {

        //                Col.ID = new Guid(result.Id.ToString());
        //                Col.Label = result.Label;
        //            }
        //        }
        //        query = client.CreateGremlinQuery<Vertex>(graph, "g.V().outE('Collection').inV()");
        //        while (query.HasMoreResults)
        //        {
        //            foreach (Vertex result in await query.ExecuteNextAsync<Vertex>())
        //            {
        //                Guid getId = new Guid(result.Id.ToString());
        //                ChildItem ci = new ChildItem();
        //                ci.ID = getId;
        //                ci.Label = result.Label;// Dictionary < string, string>


        //                foreach (VertexProperty item in result.GetVertexProperties())
        //                {

        //                    ci.Properties.Add(item.Key, item.Value);
        //                }
        //                Col.Children.Add(getId, ci);
        //            }
        //        }
        //    }


        //}
    }   
}
