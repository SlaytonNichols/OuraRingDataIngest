using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
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

        public async Task ExecuteAsync(CronInfo cronInfo)
        {
            try
            {
                _logger.LogInformation($"Query From: {cronInfo.StartQueryDate:yyyy-MM-ddTHH:mm:sszzz}, Query To: {cronInfo.EndQueryDate:yyyy-MM-ddTHH:mm:sszzz}");
                var heartRates = await _ouraRingClient.GetHeartRatesAsync(cronInfo.StartQueryDate, cronInfo.EndQueryDate);
                var json = heartRates.Data.ToList().ToJson(x =>
                            {
                                x.DateHandler = DateHandler.ISO8601DateTime;
                                x.DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
                            });
                if (heartRates.Errors == null)
                    await _adlsClient.WriteJsonToAdls(json);
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error");
            }
        }
    }
}