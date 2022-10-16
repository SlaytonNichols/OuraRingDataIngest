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

    public override void Configure(Container container)
    {
        Plugins.Add(new OpenApiFeature());
        Plugins.Add(new PostmanFeature());

        SetConfig(new HostConfig
        {
        });

        Plugins.Add(new SpaFeature
        {
            EnableSpaFallback = true
        });


        Plugins.Add(new CorsFeature(allowOriginWhitelist: new[]{
            "http://localhost:5002",
            "http://localhost:3000",
            "http://localhost:5174",
            "http://localhost:5173",
            "https://localhost:5003",
            "https://" + Environment.GetEnvironmentVariable("DEPLOY_CDN"),
            "https://" + Environment.GetEnvironmentVariable("DEPLOY_API")
        }, allowCredentials: true));


        ConfigurePlugin<UiFeature>(feature => { });
    }
}
