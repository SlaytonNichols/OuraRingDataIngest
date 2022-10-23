using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using OuraRingDataIngest.Service.Domain.Models;

namespace OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.OuraRingClient
{
    public interface IOuraRingClient
    {
        Task<HeartRates> GetHeartRatesAsync(string uri);
        string BuildHeartRateUri(DateTime startDate, DateTime endDate);
    }
}