using System;
using ServiceStack;
using OuraRingDataIngest.ServiceModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiceStack.Text;
using ServiceStack.Configuration;

namespace OuraRingDataIngest.ServiceInterface
{
    public class HeartRateIngestService : BackgroundService
    {
        private readonly ILogger<HeartRateIngestService> _logger;

        public HeartRateIngestService(ILogger<HeartRateIngestService> logger)
        {
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("HeartRateIngestService Starting...");

                    var startDate = DateTime.Now.AddDays(-1);
                    var endDate = DateTime.Now;

                    var heartRateUrl = $"https://api.ouraring.com/v2/usercollection/heartrate";
                    heartRateUrl = heartRateUrl.AddQueryParam("start_datetime", $"{startDate:yyyy-MM-ddThh:mm:ss}", false);
                    heartRateUrl = heartRateUrl.AddQueryParam("end_datetime", $"{endDate:yyyy-MM-ddThh:mm:ss}", false);

                    var response = await heartRateUrl.GetJsonFromUrlAsync(x =>
                    {
                        x.With(req =>
                        {
                            req.AddHeader("Authorization", "Bearer " + Environment.GetEnvironmentVariable("Oura__Pat"));
                            req.AddHeader("Accept", "application/json");
                        });
                    }).ConfigAwait();


                    _logger.LogInformation(response);
                    _logger.LogInformation("HeartRateIngestService Completed.");
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "Error in HeartRateIngestService");
            }
        }
    }
}
