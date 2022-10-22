using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Files.DataLake;

namespace OuraRingDataIngest.Service.Infrastructure.Adls;

public class AdlsClient : IAdlsClient
{
    public AdlsClient()
    {

    }
    private DataLakeServiceClient GetDataLakeServiceClient(String clientID, string clientSecret, string tenantID, string adlsUri)
    {

        TokenCredential credential = new ClientSecretCredential(
            tenantID, clientID, clientSecret, new TokenCredentialOptions());

        return new DataLakeServiceClient(new Uri(adlsUri), credential);
    }

    public async Task WriteJsonToAdls(string json)
    {
        var serviceClient = GetDataLakeServiceClient(Environment.GetEnvironmentVariable("CLIENTID"),
                                 Environment.GetEnvironmentVariable("CLIENT_SECRET"),
                                 Environment.GetEnvironmentVariable("TENANTID"),
                                 Environment.GetEnvironmentVariable("ADLS_URI"));
        var databricks = serviceClient.GetFileSystemClient("databricks");
        var landing = databricks.GetDirectoryClient("landing");
        if (!landing.Exists())
            await landing.CreateAsync();
        var ouraring = landing.GetSubDirectoryClient("ouraring");
        if (!ouraring.Exists())
            await ouraring.CreateAsync();

        var heartrates = ouraring.GetSubDirectoryClient("heartrates");
        if (!heartrates.Exists())
            await heartrates.CreateAsync();

        var fileName = @$"{DateTime.Now:yyyy-MM-dd-HH-mm}.json";
        byte[] jsonBytes = Encoding.ASCII.GetBytes(json);

        using (MemoryStream fileStream = new MemoryStream(jsonBytes))
        {
            var file = heartrates.CreateFile(fileName);
            var fileClient = heartrates.GetFileClient(fileName);

            long fileSize = fileStream.Length;

            await fileClient.AppendAsync(fileStream, offset: 0);

            await fileClient.FlushAsync(position: fileSize);
        }
    }
}