using Microsoft.Extensions.DependencyInjection;
using OuraRingDataIngest.Service.Core.Mappers;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.OuraRingClient;
using ServiceStack;
using SlaytonNichols.Common;

namespace OuraRingDataIngest.Service;

public static class OuraRingDataIngestDependencyInjectionExtensions
{
    public static void AddOuraRingDataIngestServices(this IServiceCollection services)
    {
        services.AddCommonServices();
        services.AddSingleton<IHeartRateIngestWorker, HeartRateIngestWorker>();
        services.AddSingleton<IHeartRatesMapper, HeartRatesMapper>();
        services.AddSingleton<IOuraRingClient, OuraRingClient>();
    }
}