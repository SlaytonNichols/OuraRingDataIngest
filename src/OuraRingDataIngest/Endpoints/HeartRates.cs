
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using ServiceStack;

namespace OuraRingDataIngest.Endpoints;

[Route("/heartrates", "GET")]
[ValidateHasRole("Admin")]
public class QueryHeartRates : IReturn<string>
{
    public DateTime StartQueryDate { get; set; }
    public DateTime EndQueryDate { get; set; }
}

public class HeartRates : ServiceStack.Service
{
    private readonly IHeartRateIngestWorker _worker;
    public HeartRates(IHeartRateIngestWorker worker)
    {
        _worker = worker;
    }

    public async Task<string> Get(QueryHeartRates query)
    {
        return await _worker.ExecuteAsync(start: query.StartQueryDate, end: query.EndQueryDate);
    }
}
