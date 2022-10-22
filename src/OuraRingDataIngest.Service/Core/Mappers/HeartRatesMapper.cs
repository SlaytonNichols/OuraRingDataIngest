using System.Collections.Generic;
using System.Linq;
using OuraRingDataIngest.Service.Domain.Models;
using OuraRingHeartRates = OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.HeartRates;

namespace OuraRingDataIngest.Service.Core.Mappers;

public class HeartRatesMapper : IHeartRatesMapper
{
    public IEnumerable<HeartRate> Map(OuraRingHeartRates heartRates)
    {
        return heartRates.Data.Select(heartRate => new HeartRate
        {
            Bpm = heartRate.bpm,
            Timestamp = heartRate.timestamp,
            Source = heartRate.source
        });
    }
}