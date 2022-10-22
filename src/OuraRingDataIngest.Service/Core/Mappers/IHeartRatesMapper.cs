using System.Collections.Generic;
using System.Linq;
using OuraRingDataIngest.Service.Domain.Models;
using OuraRingHeartRates = OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.HeartRates;

namespace OuraRingDataIngest.Service.Core.Mappers;

public interface IHeartRatesMapper
{
    IEnumerable<HeartRate> Map(OuraRingHeartRates heartRates);
}