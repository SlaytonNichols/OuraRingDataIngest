using System;
using Microsoft.Extensions.DependencyInjection;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRingClient;
using ServiceStack;
using SlaytonNichols.Common.Infrastructure.Adls;
using SlaytonNichols.Common.Infrastructure.Cron;

namespace OuraRingDataIngest.Service;

public static class OuraRingDataIngestDependencyInjectionExtensions
{
    public static void AddOuraRingDataIngestServices(this IServiceCollection services)
    {
        services.AddSingleton<IHeartRateIngestWorker, HeartRateIngestWorker>();
        services.AddSingleton<IAdlsClient, AdlsClient>();
        services.AddSingleton<IOuraRingClient, OuraRingClient>();
        services.AddSingleton<ICronClient, CronClient>();
    }
}