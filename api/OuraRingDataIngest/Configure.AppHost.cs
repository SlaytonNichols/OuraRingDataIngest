using Funq;
using ServiceStack;
using OuraRingDataIngest.ServiceInterface;
using System.Text.Encodings.Web;
using ServiceStack.Api.OpenApi;

[assembly: HostingStartup(typeof(OuraRingDataIngest.AppHost))]

namespace OuraRingDataIngest;

public class AppHost : AppHostBase, IHostingStartup
{
    public void Configure(IWebHostBuilder builder) => builder
        .ConfigureServices((context, services) =>
        {
            services.ConfigureNonBreakingSameSiteCookies(context.HostingEnvironment);
            services.AddHostedService<HeartRateIngestService>();
            services.AddHttpUtilsClient();
            services.AddJsonApiClient(Environment.GetEnvironmentVariable("BASE_URI"));
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

    public AppHost() : base("OuraRingDataIngest", typeof(HeartRateIngestService).Assembly) { }

    public override void Configure(Container container)
    {
        Plugins.Add(new OpenApiFeature());
        Plugins.Add(new PostmanFeature());
        //ServiceStack
        SetConfig(new HostConfig
        {
            WebHostUrl = Environment.GetEnvironmentVariable("BASE_URI"),
        });


        Plugins.Add(new SpaFeature
        {
            EnableSpaFallback = true
        });


        Plugins.Add(new CorsFeature(allowOriginWhitelist: new[]{
            "http://localhost:5000",
            "http://localhost:3000",
            "http://localhost:5174",
            "https://localhost:5001",
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
