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
using OuraRingDataIngest.Service.Infrastructure.Adls;
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
        private readonly IAdlsClient _adlsClient;

        public HeartRateIngestWorker(ILogger<HeartRateIngestWorker> logger, IOuraRingClient ouraRingClient, IHeartRatesMapper heartRatesMapper, IAdlsClient adlsClient)
        {
            _logger = logger;
            _ouraRingClient = ouraRingClient;
            _heartRatesMapper = heartRatesMapper;
            _adlsClient = adlsClient;
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
                    await _adlsClient.WriteJsonToAdls(json);
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
    }
}