using System;
using Microsoft.Extensions.DependencyInjection;
using ServiceStack;
using ServiceStack.Auth;
using ServiceStack.Configuration;
using ServiceStack.Authentication.MongoDb;
using MongoDB.Driver;
using ServiceStack.Web;

[assembly: HostingStartup(typeof(OuraRingDataIngest.ConfigureAuthRepository))]

namespace OuraRingDataIngest
{
    public class AppUserAuthEvents : AuthEvents
    {
        public override void OnAuthenticated(IRequest req, IAuthSession session, IServiceBase authService,
            IAuthTokens tokens, Dictionary<string, string> authInfo)
        {
            var authRepo = HostContext.AppHost.GetAuthRepository(req);
            using (authRepo as IDisposable)
            {
                var userAuth = authRepo.GetUserAuth(session.UserAuthId);
                authRepo.SaveUserAuth(userAuth);
            }
        }
    }

    public class ConfigureAuthRepository : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder) => builder
            .ConfigureServices(services => services.AddSingleton<IAuthRepository>(c =>
                new MongoDbAuthRepository(c.Resolve<IMongoDatabase>(), createMissingCollections: false)))
            .ConfigureAppHost(appHost =>
            {
                var authRepo = appHost.Resolve<IAuthRepository>();
                authRepo.InitSchema();

            }, afterConfigure: appHost =>
                appHost.AssertPlugin<AuthFeature>().AuthEvents.Add(new AppUserAuthEvents()));
    }
}
