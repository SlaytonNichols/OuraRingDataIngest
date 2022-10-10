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

                    var _client = CreateClient();
                    var startDate = DateTime.Now.AddDays(-1);
                    var endDate = DateTime.Now;
                    var authResponse = new ApiResult<AuthenticateResponse>();
                    var addExecutionResponse = new ApiResult<CreateResponse>();
                    var updateExecutionResponse = new ApiResult<UpdateResponse>();
                    var createHeartRateResponse = new ApiResult<IdResponse>();

                    authResponse = _client.Api(new Authenticate
                    {
                        provider = CredentialsAuthProvider.Name,
                        UserName = Environment.GetEnvironmentVariable("EMAIL"),
                        Password = Environment.GetEnvironmentVariable("PASSWORD"),
                        RememberMe = true,
                    });

                    if (authResponse.Succeeded)
                    {
                        addExecutionResponse = _client.Api(new CreateExecution
                        {
                            StartDateTime = DateTime.Now,
                            StartQueryDateTime = startDate,
                            EndQueryDateTime = endDate
                        });
                        if (addExecutionResponse.Failed)
                            _logger.LogError("HeartRateIngestService CreateExecution Failed: " + addExecutionResponse.ErrorMessage);

                        var heartRates = GetHeartRates(startDate, endDate);
                        if (heartRates.Errors == null)
                            foreach (var item in heartRates.Data)
                            {
                                var index = heartRates.Data.ToList().IndexOf(item);
                                createHeartRateResponse = _client.Api(new CreateHeartRate
                                {
                                    Bpm = item.Bpm,
                                    Source = item.Source,
                                    Timestamp = item.Timestamp
                                });
                                if (createHeartRateResponse.Failed)
                                    _logger.LogError($"HeartRateIngestService CreateHeartRate #{index} Failed: " + createHeartRateResponse.ErrorMessage);
                            }

                        updateExecutionResponse = _client.Api(new UpdateExecution
                        {
                            Id = addExecutionResponse.Response.Id,
                            EndDateTime = DateTime.Now,
                            RecordsInserted = heartRates.Data.ToList().Count()
                        });
                        if (updateExecutionResponse.Failed)
                            _logger.LogError("HeartRateIngestService UpdateExecution Failed: " + updateExecutionResponse.ErrorMessage);
                    }
                    else
                    {
                        _logger.LogError("HeartRateIngestService Authenticate Failed: " + authResponse.ErrorMessage);
                        _logger.LogInformation("HeartRateIngestService Completed.");
                        continue;
                    }

                    _logger.LogInformation("HeartRateIngestService Completed.");
                    await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "HeartRateIngestService Error");
            }
        }

        private HeartRates GetHeartRates(DateTime startDate, DateTime endDate)
        {
            try
            {
                _logger.LogInformation("HeartRateIngestService GetHeartRatesAsync Starting...");

                var heartRateUrl = $"https://api.ouraring.com/v2/usercollection/heartrate";

                heartRateUrl = heartRateUrl.AddQueryParam("start_datetime", $"{startDate:yyyy-MM-ddThh:mm:sszzz}".Replace("+", "%2B"), false);
                heartRateUrl = heartRateUrl.AddQueryParam("end_datetime", $"{endDate:yyyy-MM-ddThh:mm:sszzz}".Replace("+", "%2B"), false);

                var response = heartRateUrl.GetJsonFromUrl(x =>
                {
                    x.With(req =>
                    {
                        req.AddHeader("Authorization", "Bearer " + Environment.GetEnvironmentVariable("Oura__Pat"));
                        req.AddHeader("Accept", "application/json");
                    });
                });

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
