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
    private DataLakeServiceClient GetDataLakeServiceClient()
    {

        TokenCredential credential = new ClientSecretCredential(Environment.GetEnvironmentVariable("TENANTID"),
                                                                Environment.GetEnvironmentVariable("CLIENTID"),
                                                                Environment.GetEnvironmentVariable("CLIENT_SECRET"),
                                                                new TokenCredentialOptions());

        return new DataLakeServiceClient(new Uri(Environment.GetEnvironmentVariable("ADLS_URI")), credential);
    }

    public async Task<DataLakeDirectoryClient> CreateHeartRatesDirectoryIfNotExists()
    {
        var serviceClient = GetDataLakeServiceClient();
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

        return heartrates;
    }

    public async Task WriteJsonToDirectory(DataLakeDirectoryClient client, string json)
    {
        var fileName = @$"{DateTime.Now:yyyy-MM-dd-HH-mm}.json";
        byte[] jsonBytes = Encoding.ASCII.GetBytes(json);

        using (MemoryStream fileStream = new MemoryStream(jsonBytes))
        {
            var file = client.CreateFile(fileName);
            var fileClient = client.GetFileClient(fileName);

            long fileSize = fileStream.Length;

            await fileClient.AppendAsync(fileStream, offset: 0);

            await fileClient.FlushAsync(position: fileSize);
        }
    }
}