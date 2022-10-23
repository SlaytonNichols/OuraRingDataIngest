using System;
using OuraRingDataIngest.Service.Core.Dtos;

namespace OuraRingDataIngest.Service.Core.Managers;

public class HeartRateIngestWorkerDateManager : IHeartRateIngestWorkerDateManager
{
    public (DateTime StartQueryDate, DateTime EndQueryDate) GetQueryDates(HeartRateIngestWorkerRequest request)
    {
        if (request.StartQueryDate.HasValue && request.EndQueryDate.HasValue)
        {
            return (request.StartQueryDate.Value, request.EndQueryDate.Value);
        }
        else
            return (request.CronInfo.StartQueryDate.Value, request.CronInfo.EndQueryDate.Value);
    }
}