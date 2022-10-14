using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OuraRingDataIngest.Service.Infrastructure.Cron.CronClient;

namespace OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker
{
    public interface IHeartRateIngestWorker
    {
        Task ExecuteAsync(CronInfo cronInfo);
    }
}