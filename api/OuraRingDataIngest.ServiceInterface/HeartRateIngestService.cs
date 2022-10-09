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
using System.Web;

namespace OuraRingDataIngest.ServiceInterface
{
    public class HeartRateIngestService : BackgroundService
    {
        private readonly ILogger<HeartRateIngestService> _logger;
        public IServiceClient CreateClient() => new JsonApiClient(Environment.GetEnvironmentVariable("BASE_URI"))
        {
            UserName = Environment.GetEnvironmentVariable("EMAIL"),
            Password = Environment.GetEnvironmentVariable("PASSWORD"),
            AlwaysSendBasicAuthHeader = true,
        };


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
                    await Task.Delay(3000);
                    var _client = CreateClient();
                    var startDate = DateTime.Now.AddDays(-1);
                    var endDate = DateTime.Now;
                    var authResponse = new ApiResult<AuthenticateResponse>();
                    var addExecutionResponse = new ApiResult<CreateResponse>();
                    var updateExecutionResponse = new ApiResult<UpdateResponse>();
                    var createHeartRateResponse = new ApiResult<IdResponse>();

                    authResponse = await _client.ApiAsync(new Authenticate
                    {
                        provider = BasicAuthProvider.Name,
                        UserName = Environment.GetEnvironmentVariable("EMAIL"),
                        Password = Environment.GetEnvironmentVariable("PASSWORD"),
                        RememberMe = true,
                    });

                    if (authResponse.Succeeded)
                    {
                        addExecutionResponse = await _client.ApiAsync(new CreateExecution
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
                                createHeartRateResponse = await _client.ApiAsync(new CreateHeartRate
                                {
                                    Bpm = item.Bpm,
                                    Source = item.Source,
                                    Timestamp = item.Timestamp
                                });
                                if (createHeartRateResponse.Failed)
                                    _logger.LogError($"HeartRateIngestService CreateHeartRate #{index} Failed: " + createHeartRateResponse.ErrorMessage);
                            }

                        updateExecutionResponse = await _client.ApiAsync(new UpdateExecution
                        {
                            Id = addExecutionResponse.Response.Id,
                            EndDateTime = DateTime.Now,
                            RecordsInserted = heartRates.Data.ToList().Count()
                        });
                        if (updateExecutionResponse.Failed)
                            _logger.LogError("HeartRateIngestService UpdateExecution Failed: " + updateExecutionResponse.ErrorMessage);
                    }
                    else
                        _logger.LogError("HeartRateIngestService Authenticate Failed: " + authResponse.ErrorMessage);

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
