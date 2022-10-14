using Funq;
using ServiceStack;
using System.Text.Encodings.Web;
using ServiceStack.Api.OpenApi;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;

[assembly: HostingStartup(typeof(OuraRingDataIngest.AppHost))]

namespace OuraRingDataIngest;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.ConfigureNonBreakingSameSiteCookies(context.HostingEnvironment);
            services.AddHttpClient();
            services.AddHttpUtilsClient();
        }).ConfigureLogging(logginBuilder =>
        {
            logginBuilder.ClearProviders();
            logginBuilder.AddJsonConsole(jsonConsoleFormatterOptions =>
            {
                jsonConsoleFormatterOptions.JsonWriterOptions = new()
                {
                    Indented = false,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };
            });
        });

    public AppHost() : base("OuraRingDataIngest", typeof(HeartRateIngestWorker).Assembly) { }

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


        ConfigurePlugin<UiFeature>(feature =>
        {
            feature.Info.BrandIcon.Uri = "/assets/img/logo.svg";
            feature.Info.BrandIcon.Cls = "inline-block w-8 h-8 mr-2";
        });
    }
}
