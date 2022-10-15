using System;
using OuraRingDataIngest.Service.Infrastructure.Cron.CronClient;

namespace OuraRingDataIngest.Service.Core.Dtos;

public class HeartRatesRequest
{
    public DateTime? StartQueryDate { get; set; }
    public DateTime? EndQueryDate { get; set; }
    public CronInfo CronInfo { get; set; }
}