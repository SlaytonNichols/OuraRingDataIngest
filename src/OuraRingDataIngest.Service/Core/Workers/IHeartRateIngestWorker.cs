using System.Threading.Tasks;
using OuraRingDataIngest.Service.Core.Dtos;

namespace OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker
{
    public interface IHeartRateIngestWorker
    {
        Task<HeartRateIngestWorkerResponse> ExecuteAsync(HeartRateIngestWorkerRequest request);
    }
}