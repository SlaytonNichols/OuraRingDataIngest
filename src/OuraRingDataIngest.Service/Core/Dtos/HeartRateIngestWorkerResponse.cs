using System.Collections.Generic;
using OuraRingDataIngest.Service.Domain.Models;
using ServiceStack;

namespace OuraRingDataIngest.Service.Core.Dtos;

public class HeartRateIngestWorkerResponse : QueryResponse<HeartRate>
{
    public IEnumerable<string> Errors { get; set; }
}