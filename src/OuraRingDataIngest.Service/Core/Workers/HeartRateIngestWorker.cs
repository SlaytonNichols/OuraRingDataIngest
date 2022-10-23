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
using OuraRingDataIngest.Service.Core.Managers;
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
        private readonly IHeartRateIngestWorkerDateManager _dateManager;

        public HeartRateIngestWorker(ILogger<HeartRateIngestWorker> logger, IOuraRingClient ouraRingClient, IHeartRatesMapper heartRatesMapper, IAdlsClient adlsClient, IHeartRateIngestWorkerDateManager dateManager)
        {
            _logger = logger;
            _ouraRingClient = ouraRingClient;
            _heartRatesMapper = heartRatesMapper;
            _adlsClient = adlsClient;
            _dateManager = dateManager;
        }

        public async Task<HeartRateIngestWorkerResponse> ExecuteAsync(HeartRateIngestWorkerRequest request)
        {
            var response = new HeartRateIngestWorkerResponse();
            try
            {
                var queryDates = _dateManager.GetQueryDates(request);
                var startQueryDate = queryDates.StartQueryDate;
                var endQueryDate = queryDates.EndQueryDate;

                _logger.LogInformation($"Query From: {startQueryDate:yyyy-MM-ddTHH:mm:sszzz}, Query To: {endQueryDate:yyyy-MM-ddTHH:mm:sszzz}");
                var heartRatesUri = _ouraRingClient.BuildHeartRateUri(startQueryDate, endQueryDate);
                var heartRates = await _ouraRingClient.GetHeartRatesAsync(heartRatesUri);
                var heartRatesMapped = _heartRatesMapper.Map(heartRates);
                var json = heartRatesMapped.ToList().ToJson(x =>
                            {
                                x.DateHandler = DateHandler.ISO8601DateTime;
                                x.DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
                            });

                if (heartRates != null && heartRates.Errors == null)
                {
                    var target = await _adlsClient.CreateHeartRatesDirectoryIfNotExists();
                    await _adlsClient.WriteJsonToDirectory(target, json);
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