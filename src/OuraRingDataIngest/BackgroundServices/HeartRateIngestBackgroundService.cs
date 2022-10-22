using System;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using SlaytonNichols.Common.Infrastructure.Cron;

namespace OuraRingDataIngest.BackgroundServices
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

                    var request = new HeartRateIngestWorkerRequest
                    {
                        StartQueryDate = null,
                        EndQueryDate = null,
                        CronInfo = cronInfo
                    };
                    _logger.LogInformation("Starting...");

                    await _heartRateIngestWorker.ExecuteAsync(request);

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