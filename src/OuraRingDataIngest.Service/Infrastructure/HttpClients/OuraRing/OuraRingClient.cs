using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using OuraRingDataIngest.Service.Domain.Models;
using ServiceStack;
using ServiceStack.Text;

namespace OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.OuraRingClient
{
    public class OuraRingClient : IOuraRingClient
    {
        private readonly ILogger<OuraRingClient> _logger;
        public OuraRingClient(ILogger<OuraRingClient> logger)
        {
            _logger = logger;
        }
        public async Task<HeartRates> GetHeartRatesAsync(string uri)
        {
            try
            {
                _logger.LogInformation("GetHeartRatesAsync Starting...");

                _logger.LogInformation("GetHeartRatesAsync Calling OuraRing API Endpoint: " + uri);
                var response = await uri.GetJsonFromUrlAsync(x =>
                {
                    x.With(req =>
                    {
                        req.AddHeader("Authorization", "Bearer " + Environment.GetEnvironmentVariable("Oura__Pat"));
                        req.AddHeader("Accept", "application/json");
                    });
                }).ConfigAwait();
                _logger.LogInformation("GetHeartRatesAsync Completed... Oura Ring API Response:" + response);

                return response.FromJson<HeartRates>();
            }
            catch (System.Exception ex)
            {
                _logger.LogError(ex, "GetHeartRatesAsync Error");
                return new HeartRates { Errors = new List<string> { ex.Message } };
            }
        }

        public string BuildHeartRateUri(DateTime startDate, DateTime endDate)
        {
            var startQueryParam = $"{startDate:yyyy-MM-ddTHH:mm:sszzz}".Replace("+", "%2B");
            var endQueryParam = $"{endDate:yyyy-MM-ddTHH:mm:sszzz}".Replace("+", "%2B");

            var heartRateUrl = $"https://api.ouraring.com/v2/usercollection/heartrate";
            heartRateUrl = heartRateUrl.AddQueryParam("start_datetime", startQueryParam, false);
            heartRateUrl = heartRateUrl.AddQueryParam("end_datetime", endQueryParam, false);

            return heartRateUrl;
        }
    }
}