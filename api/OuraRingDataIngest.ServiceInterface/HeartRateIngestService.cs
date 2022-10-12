using System;
using ServiceStack;
using OuraRingDataIngest.ServiceModel;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ServiceStack.Text;
using System.Linq;
using Azure.Storage.Files.DataLake;
using System.IO;
using Azure.Identity;
using Azure.Core;
using NCrontab;

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
                    var s = CrontabSchedule.Parse(Environment.GetEnvironmentVariable("CRON"));
                    var schedule = s.GetNextOccurrences(DateTime.Now.AddYears(-1), DateTime.Now.AddYears(1)).ToList();
                    var next = s.GetNextOccurrence(DateTime.Now);
                    var timespan = schedule[1] - schedule[0];
                    var delay = next - DateTime.Now;
                    await Task.Delay(delay, stoppingToken);

                    _logger.LogInformation("HeartRateIngestService Starting...");

                    var startDate = DateTime.SpecifyKind(DateTime.Now.Subtract(timespan), DateTimeKind.Local);
                    var endDate = DateTime.Now;

                    var heartRates = await GetHeartRatesAsync(startDate, endDate);

                    if (heartRates.Errors == null)
                        await WriteJsonToAdls(heartRates);

                    _logger.LogInformation("HeartRateIngestService Completed.");
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
                var startQueryParam = $"{startDate:yyyy-MM-ddTHH:mm:sszzz}".Replace("+", "%2B");
                var endQueryParam = $"{endDate:yyyy-MM-ddTHH:mm:sszzz}".Replace("+", "%2B");

                var heartRateUrl = $"https://api.ouraring.com/v2/usercollection/heartrate";
                heartRateUrl = heartRateUrl.AddQueryParam("start_datetime", startQueryParam, false);
                heartRateUrl = heartRateUrl.AddQueryParam("end_datetime", endQueryParam, false);

                _logger.LogInformation("HeartRateIngestService GetHeartRatesAsync Calling OuraRing API Endpoint: " + heartRateUrl);
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

        private async Task WriteJsonToAdls(HeartRates heartRates)
        {
            var dataLakeClient = new DataLakeServiceClient(new Uri("https://snadls.blob.core.windows.net"));
            GetDataLakeServiceClient(ref dataLakeClient,
                                     "snadls",
                                     Environment.GetEnvironmentVariable("CLIENTID"),
                                     Environment.GetEnvironmentVariable("CLIENT_SECRET"),
                                     Environment.GetEnvironmentVariable("TENANTID"));

            var databricks = dataLakeClient.GetFileSystemClient("databricks");
            var landing = databricks.GetDirectoryClient("landing");
            if (!landing.Exists())
                await landing.CreateAsync();
            var ouraring = landing.GetSubDirectoryClient("ouraring");
            if (!ouraring.Exists())
                await ouraring.CreateAsync();

            var heartrates = ouraring.GetSubDirectoryClient("heartrates");
            if (!heartrates.Exists())
                await heartrates.CreateAsync();

            var fileName = @$"{DateTime.Now:yyyy-MM-dd-HH-mm}.json";
            File.WriteAllText(fileName, heartRates.Data.ToList().ToJson(x =>
            {
                x.DateHandler = DateHandler.ISO8601DateTime;
                x.DateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz";
            }));
            var file = heartrates.CreateFile(fileName);
            var fileClient = heartrates.GetFileClient(fileName);
            FileStream fileStream = File.OpenRead(fileName);

            long fileSize = fileStream.Length;

            await fileClient.AppendAsync(fileStream, offset: 0);

            await fileClient.FlushAsync(position: fileSize);
            File.Delete(fileName);
        }

        public static void GetDataLakeServiceClient(ref DataLakeServiceClient dataLakeServiceClient,
    String accountName, String clientID, string clientSecret, string tenantID)
        {

            TokenCredential credential = new ClientSecretCredential(
                tenantID, clientID, clientSecret, new TokenCredentialOptions());

            string dfsUri = "https://snadls.dfs.core.windows.net";

            dataLakeServiceClient = new DataLakeServiceClient(new Uri(dfsUri), credential);
        }
    }
}
