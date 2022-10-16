using Microsoft.Extensions.DependencyInjection;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRingClient;
using ServiceStack;

namespace OuraRingDataIngest.Service;

public static class OuraRingDataIngestDependencyInjectionExtensions
{
    public static void AddOuraRingDataIngestServices(this IServiceCollection services)
    {
        services.AddSingleton<IHeartRateIngestWorker, HeartRateIngestWorker>();
        services.AddSingleton<IOuraRingClient, OuraRingClient>();
    }
}