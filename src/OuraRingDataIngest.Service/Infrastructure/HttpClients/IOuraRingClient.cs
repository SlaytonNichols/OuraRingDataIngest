using System;
using System.Threading.Tasks;
using OuraRingDataIngest.Service.Domain.Models;

namespace OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRingClient
{
    public interface IOuraRingClient
    {
        Task<HeartRates> GetHeartRatesAsync(DateTime startDate, DateTime endDate);
    }
}