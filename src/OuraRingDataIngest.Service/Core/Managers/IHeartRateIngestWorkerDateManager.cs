using System;
using OuraRingDataIngest.Service.Core.Dtos;

namespace OuraRingDataIngest.Service.Core.Managers;

public interface IHeartRateIngestWorkerDateManager
{
    (DateTime StartQueryDate, DateTime EndQueryDate) GetQueryDates(HeartRateIngestWorkerRequest request);
}