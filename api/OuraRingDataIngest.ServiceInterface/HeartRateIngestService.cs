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
        private readonly JsonApiClient _client;

        public HeartRateIngestService(ILogger<HeartRateIngestService> logger, JsonApiClient client)
        {
            _logger = logger;
            _client = client;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("HeartRateIngestService Starting...");
                    var authResponse = await _client.ApiAsync(new Authenticate
                    {
                        provider = CredentialsAuthProvider.Name,
                        UserName = Environment.GetEnvironmentVariable("EMAIL"),
                        Password = Environment.GetEnvironmentVariable("PASSWORD"),
                        RememberMe = true,
                    });

                    if (authResponse.Failed)
                        _logger.LogError("HeartRateIngestService Authenticate Failed: " + authResponse.ErrorMessage);

                    var startDate = DateTime.Now.AddDays(-2);
                    var endDate = DateTime.Now;

                    var addExecutionResponse = await _client.ApiAsync(new CreateExecution
                    {
                        StartDateTime = DateTime.Now,
                        StartQueryDateTime = startDate,
                        EndQueryDateTime = endDate
                    });
                    if (addExecutionResponse.Failed)
                        _logger.LogError("HeartRateIngestService CreateExecution Failed: " + addExecutionResponse.ErrorMessage);

                    var heartRates = await GetHeartRatesAsync(startDate, endDate);
                    if (heartRates.Errors == null)
                        foreach (var item in heartRates.Data)
                        {
                            var index = heartRates.Data.ToList().IndexOf(item);
                            var addHeartRateResponse = await _client.ApiAsync(new CreateHeartRate
                            {
                                Bpm = item.Bpm,
                                Source = item.Source,
                                Timestamp = item.Timestamp
                            });
                            if (addHeartRateResponse.Failed)
                                _logger.LogError($"HeartRateIngestService CreateHeartRate #{index} Failed: " + addHeartRateResponse.ErrorMessage);
                        }

                    var updateExecution = await _client.ApiAsync(new UpdateExecution
                    {
                        Id = addExecutionResponse.Response.Id,
                        EndDateTime = DateTime.Now,
                        RecordsInserted = heartRates.Data.ToList().Count()
                    });
                    if (updateExecution.Failed)
                        _logger.LogError("HeartRateIngestService UpdateExecution Failed: " + updateExecution.ErrorMessage);

                    _logger.LogInformation("HeartRateIngestService Completed.");
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "HeartRateIngestService Error");
            }
        }

        private async Task<HeartRates> GetHeartRatesAsync(DateTime startDate, DateTime endDate)
        {
            // var heartRates = new HeartRates();
            try
            {
                _logger.LogInformation("HeartRateIngestService GetHeartRatesAsync Starting...");

                var heartRateUrl = $"https://api.ouraring.com/v2/usercollection/heartrate";

                heartRateUrl = heartRateUrl.AddQueryParam("start_datetime", $"{startDate:yyyy-MM-ddThh:mm:sszzz}".Replace("+", "%2B"), false);
                heartRateUrl = heartRateUrl.AddQueryParam("end_datetime", $"{endDate:yyyy-MM-ddThh:mm:sszzz}".Replace("+", "%2B"), false);

                var response = await heartRateUrl.GetJsonFromUrlAsync(x =>
                {
                    x.With(req =>
                    {
                        req.AddHeader("Authorization", "Bearer " + Environment.GetEnvironmentVariable("Oura__Pat"));
                        req.AddHeader("Accept", "application/json");
                    });
                }).ConfigAwait();

                // heartRates = response.FromJson<HeartRates>();
                _logger.LogInformation("HeartRateIngestService GetHeartRatesAsync Completed... Oura Ring API Response:" + response);

                return response.FromJson<HeartRates>();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "HeartRateIngestService GetHeartRatesAsync Error");
                return new HeartRates
                {
                    Errors = new List<string> { ex.Message }
                };
            }
        }
    }
}
