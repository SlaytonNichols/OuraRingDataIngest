
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;

namespace OuraRingDataIngest.Controllers;

[Route("/heartrates", "GET")]
[ValidateHasRole("Admin")]
public class ExecuteHeartRateIngest : HeartRateIngestWorkerRequest, IReturn<HeartRateIngestWorkerResponse> { }

public class HeartRateIngestController : ServiceStack.Service
{
    private readonly IHeartRateIngestWorker _worker;
    public HeartRateIngestController(IHeartRateIngestWorker worker)
    {
        _worker = worker;
    }

    public async Task<object> Get(ExecuteHeartRateIngest query)
    {
        var request = new HeartRateIngestWorkerRequest
        {
            StartQueryDate = query.StartQueryDate,
            EndQueryDate = query.EndQueryDate,
            CronInfo = null
        };
        var result = await _worker.ExecuteAsync(request);

        return result;
    }
}
