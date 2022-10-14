using System;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using OuraRingDataIngest.Service.Infrastructure.Cron.CronClient;

namespace OuraRingDataIngest
{
    public class HeartRateIngestBackgroundService : BackgroundService
    {
        private readonly ILogger<HeartRateIngestBackgroundService> _logger;
        private readonly IHeartRateIngestWorker _heartRateIngestWorker;
        private readonly ICronClient _cronClient;
        public HeartRateIngestBackgroundService(ILogger<HeartRateIngestBackgroundService> logger, IHeartRateIngestWorker heartRateIngestWorker, ICronClient cronClient)
        {
            _logger = logger;
            _heartRateIngestWorker = heartRateIngestWorker;
            _cronClient = cronClient;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var cronInfo = await _cronClient.AwaitNext(stoppingToken);

                    _logger.LogInformation("Starting...");

                    await _heartRateIngestWorker.ExecuteAsync(cronInfo);

                    _logger.LogInformation("Completed.");
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error");
            }
        }
    }
}