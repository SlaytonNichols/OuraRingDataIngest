using Funq;
using ServiceStack;
using System.Text.Encodings.Web;
using ServiceStack.Api.OpenApi;
using SlaytonNichols.Common.ServiceStack;

[assembly: HostingStartup(typeof(OuraRingDataIngest.AppHost))]

namespace OuraRingDataIngest;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder.ConfigureApplication();

    public AppHost() : base("OuraRingDataIngest", typeof(HeartRates).Assembly) { }

    public override void Configure(Container container) { }
}
