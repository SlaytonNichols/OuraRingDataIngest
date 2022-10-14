using System;
using System.Threading;
using System.Threading.Tasks;

namespace OuraRingDataIngest.Service.Infrastructure.Cron.CronClient
{
    public interface ICronClient
    {
        Task<CronInfo> AwaitNext(CancellationToken stoppingToken);
    }
}