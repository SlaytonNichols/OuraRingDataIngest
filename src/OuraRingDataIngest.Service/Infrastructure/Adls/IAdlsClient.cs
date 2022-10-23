using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;

namespace OuraRingDataIngest.Service.Infrastructure.Adls;
public interface IAdlsClient
{
    Task<DataLakeDirectoryClient> CreateHeartRatesDirectoryIfNotExists();
    Task WriteJsonToDirectory(DataLakeDirectoryClient client, string json);
}