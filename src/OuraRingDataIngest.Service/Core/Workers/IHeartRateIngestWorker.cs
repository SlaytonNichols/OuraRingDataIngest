using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Domain.Models;
using OuraRingDataIngest.Service.Infrastructure.Cron.CronClient;

namespace OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker
{
    public interface IHeartRateIngestWorker
    {
        Task<HeartRatesResponse> ExecuteAsync(HeartRatesRequest request);
    }
}