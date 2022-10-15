using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Infrastructure.Adls.AdlsClient;
using OuraRingDataIngest.Service.Infrastructure.Cron.CronClient;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRingClient;
using ServiceStack;
using ServiceStack.Text;

namespace OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker
{
    public class HeartRateIngestWorker : IHeartRateIngestWorker
    {
        private readonly ILogger<HeartRateIngestWorker> _logger;
        private readonly IAdlsClient _adlsClient;
        private readonly IOuraRingClient _ouraRingClient;
        public HeartRateIngestWorker(ILogger<HeartRateIngestWorker> logger, IAdlsClient adlsClient, IOuraRingClient ouraRingClient)
        {
            _logger = logger;
            _adlsClient = adlsClient;
            _ouraRingClient = ouraRingClient;
        }

        public async Task<HeartRatesResponse> ExecuteAsync(HeartRatesRequest request)
        {
            try
            {
                var response = new HeartRatesResponse();
                var startQueryDate = request.CronInfo?.StartQueryDate != null ? request.CronInfo.StartQueryDate.Value : request.StartQueryDate.Value;
                var endQueryDate = request.CronInfo != null ? request.CronInfo.EndQueryDate.Value : request.EndQueryDate.Value;
                _logger.LogInformation($"Query From: {startQueryDate:yyyy-MM-ddTHH:mm:sszzz}, Query To: {endQueryDate:yyyy-MM-ddTHH:mm:sszzz}");
                var heartRates = await _ouraRingClient.GetHeartRatesAsync(startQueryDate, endQueryDate);
                var json = heartRates.Data.ToList().ToJson(x =>
                            {
                                x.DateHandler = DateHandler.ISO8601DateTime;
                                x.DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
                            });
                if (heartRates.Errors == null)
                    await _adlsClient.WriteJsonToAdls(json);

                response.HeartRates = heartRates;
                return response;
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error");
                return null;
            }
        }
    }
}