using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ServiceStack;
using ServiceStack.Logging;
using OuraRingDataIngest.ServiceModel;

namespace OuraRingDataIngest.ServiceInterface;

public class HeartRateServices : Service
{
    public IAutoQueryDb AutoQuery { get; set; }
    // private readonly ILogger<PostsServices> _logger;

    // public PostsServices(ILogger<PostsServices> logger)
    // {
    //     _logger = logger;
    // }
    public async Task<object> Post(CreateHeartRate query)
    {
        var response = await AutoQuery.CreateAsync(query, base.Request);
        return response;
    }
}
