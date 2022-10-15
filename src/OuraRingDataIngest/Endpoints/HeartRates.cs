
using OuraRingDataIngest.Service;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using OuraRingDataIngest.Service.Protocol;
using ServiceStack;

namespace OuraRingDataIngest.Endpoints;

[Route("/heartrates", "GET")]
[ValidateHasRole("Admin")]
public class ExecuteHeartRateIngestWorker : HeartRatesRequest, IReturn<HeartRatesPresenter> { }

public class HeartRates : ServiceStack.Service
{
    private readonly IHeartRateIngestWorker _worker;
    public HeartRates(IHeartRateIngestWorker worker)
    {
        _worker = worker;
    }

    public async Task<object> Get(ExecuteHeartRateIngestWorker query)
    {
        var response = new HeartRatesPresenter();
        var request = new HeartRatesRequest
        {
            StartQueryDate = query.StartQueryDate,
            EndQueryDate = query.EndQueryDate,
            CronInfo = null
        };
        var result = await _worker.ExecuteAsync(request);
        response.Response.HeartRates = result.HeartRates;
        return response;
    }
}
