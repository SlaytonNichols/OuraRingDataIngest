using System;
using System.Threading.Tasks;
using Azure.Storage.Files.DataLake;

namespace OuraRingDataIngest.Service.Infrastructure.Adls.AdlsClient
{
    public interface IAdlsClient
    {
        DataLakeServiceClient GetDataLakeServiceClient(String clientID, string clientSecret, string tenantID);
        Task WriteJsonToAdls(string json);
    }
}