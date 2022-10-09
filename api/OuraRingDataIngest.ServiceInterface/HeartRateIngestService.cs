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
using ServiceStack.Auth;
using System.Linq;

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
                    var baseUrl = "localhost:5001";//Environment.GetEnvironmentVariable("DEPLOY_API");
                    var client = new JsonApiClient($"https://{baseUrl}");
                    AuthenticateResponse authResponse = await client.PostAsync(new Authenticate
                    {
                        provider = CredentialsAuthProvider.Name,
                        UserName = Environment.GetEnvironmentVariable("EMAIL"),
                        Password = Environment.GetEnvironmentVariable("PASSWORD"),
                        RememberMe = true,
                    });

                    var startDate = DateTime.UtcNow.AddHours(-4);
                    var endDate = DateTime.UtcNow;

                    var executionId = await client.PostAsync(new CreateExecution
                    {
                        StartDateTime = DateTime.UtcNow,
                        StartQueryDateTime = startDate,
                        EndQueryDateTime = endDate
                    });

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

                    var heartRates = response.FromJson<HeartRates>();
                    await client.PatchAsync(new UpdateExecution
                    {
                        Id = executionId.Id,
                        RecordsInserted = heartRates.Data.Count()
                    });
                    foreach (var item in heartRates.Data)
                    {
                        await client.PostAsync(new CreateHeartRate
                        {
                            Bpm = item.Bpm,
                            Source = item.Source,
                            Timestamp = item.Timestamp
                        });
                    }

                    _logger.LogInformation("HeartRateIngestService Completed.");
                    await client.PatchAsync(new UpdateExecution
                    {
                        Id = executionId.Id,
                        EndDateTime = DateTime.UtcNow
                    });
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
