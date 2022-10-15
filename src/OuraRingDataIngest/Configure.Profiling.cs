using ServiceStack;

[assembly: HostingStartup(typeof(OuraRingDataIngest.ConfigureProfiling))]

namespace OuraRingDataIngest;

public class ConfigureProfiling : IHostingStartup
{
    public void Configure(IWebHostBuilder builder)
    {
        builder.ConfigureAppHost(host => {
            host.Plugins.AddIfDebug(new RequestLogsFeature {
                EnableResponseTracking = true,
            });
            
            host.Plugins.AddIfDebug(new ProfilingFeature {
                IncludeStackTrace = true,
            });
        });
    }
}
