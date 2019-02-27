using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using PLEXOS.Identity.Data;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace PLEXOS.Identity
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.Title = "PLEXOS Identity";
          //  GetCert();
            var host = BuildWebHost(args);
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                  //  var context = services.GetRequiredService<IdentityServerDbContext>();
                
                     //  SeedData.Initialize(services);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }
            host.Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseKestrel()
                .UseUrls("http://localhost:64091")
                .Build();

        public static async void GetCert()
        {
            string getPath = AppContext.BaseDirectory;
            if (!File.Exists($@"{getPath}\certs\IdentityServer4Auth.pfx"))
            {
                byte[] fileStream = new byte[] { };
                string Connectionstring = "DefaultEndpointsProtocol=https;AccountName=plexosstore;AccountKey=8THyouCAWRp7QaOmsOyFUpMwj5yl5wpa6ERXKLh4e6DtqMudB5mZ8SjepjhzxXI8Gmauh2N9aSFjaY93kP39wQ==;EndpointSuffix=core.windows.net";//Configuration.GetValue<string>("StorageConnectionString");
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Connectionstring);
                CloudBlobClient cloudBlobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer cloudBlobContainer =
                                 cloudBlobClient.GetContainerReference("certs");
                if (await cloudBlobContainer.ExistsAsync())
                {
                    CloudBlob blockBlob = cloudBlobContainer.GetBlobReference("IdentityServer4Auth.pfx");
                    await blockBlob.FetchAttributesAsync();
                    fileStream = new byte[blockBlob.Properties.Length];
                    await blockBlob.DownloadToFileAsync($@".\certs\IdentityServer4Auth.pfx", FileMode.CreateNew);
                }
            }
               
            
        }
    }
}
