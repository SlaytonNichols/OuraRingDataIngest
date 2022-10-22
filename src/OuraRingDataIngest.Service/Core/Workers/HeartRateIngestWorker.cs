using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.Core;
using Azure.Identity;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Logging;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Core.Mappers;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.OuraRingClient;
using ServiceStack;
using ServiceStack.Text;

namespace OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker
{
    public class HeartRateIngestWorker : IHeartRateIngestWorker
    {
        private readonly ILogger<HeartRateIngestWorker> _logger;
        private readonly IOuraRingClient _ouraRingClient;
        private readonly IHeartRatesMapper _heartRatesMapper;

        public HeartRateIngestWorker(ILogger<HeartRateIngestWorker> logger, IOuraRingClient ouraRingClient, IHeartRatesMapper heartRatesMapper)
        {
            _logger = logger;
            _ouraRingClient = ouraRingClient;
            _heartRatesMapper = heartRatesMapper;
        }

        public async Task<HeartRateIngestWorkerResponse> ExecuteAsync(HeartRateIngestWorkerRequest request)
        {
            var response = new HeartRateIngestWorkerResponse();
            try
            {
                var startQueryDate = request.CronInfo?.StartQueryDate != null ? request.CronInfo.StartQueryDate.Value : request.StartQueryDate.Value;
                var endQueryDate = request.CronInfo != null ? request.CronInfo.EndQueryDate.Value : request.EndQueryDate.Value;

                _logger.LogInformation($"Query From: {startQueryDate:yyyy-MM-ddTHH:mm:sszzz}, Query To: {endQueryDate:yyyy-MM-ddTHH:mm:sszzz}");
                var heartRates = await _ouraRingClient.GetHeartRatesAsync(startQueryDate, endQueryDate);
                var heartRatesMapped = _heartRatesMapper.Map(heartRates);
                var json = heartRatesMapped.ToList().ToJson(x =>
                            {
                                x.DateHandler = DateHandler.ISO8601DateTime;
                                x.DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
                            });

                if (heartRates.Errors == null)
                {
                    await WriteJsonToAdls(json);
                    response.Results = heartRatesMapped.ToList();
                }
                else
                {
                    response.Errors = heartRates.Errors;
                }

                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error");
                response.Errors = new List<string> { ex.Message };
                return response;
            }
        }

        private DataLakeServiceClient GetDataLakeServiceClient(String clientID, string clientSecret, string tenantID)
        {

            TokenCredential credential = new ClientSecretCredential(
                tenantID, clientID, clientSecret, new TokenCredentialOptions());

            string dfsUri = "https://snadls.dfs.core.windows.net";

            return new DataLakeServiceClient(new Uri(dfsUri), credential);
        }

        private async Task WriteJsonToAdls(string json)
        {
            var serviceClient = GetDataLakeServiceClient(Environment.GetEnvironmentVariable("CLIENTID"),
                                     Environment.GetEnvironmentVariable("CLIENT_SECRET"),
                                     Environment.GetEnvironmentVariable("TENANTID"));
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
}