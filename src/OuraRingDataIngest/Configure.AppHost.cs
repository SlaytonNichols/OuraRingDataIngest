using Funq;
using OuraRingDataIngest.Controllers;
using SlaytonNichols.Common.ServiceStack;

[assembly: HostingStartup(typeof(OuraRingDataIngest.AppHost))]

namespace OuraRingDataIngest;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder.ConfigureApplication();

    public AppHost() : base("OuraRingDataIngest", typeof(HeartRateIngestController).Assembly) { }

    public override void Configure(Container container) { }
}
