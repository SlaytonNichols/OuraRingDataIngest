using System;
using System.Collections.Generic;

namespace OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing;


public class HeartRates
{
    public IEnumerable<HeartRate> Data { get; set; }
    public IEnumerable<string> Errors { get; set; }
}
public class HeartRate
{
    public int bpm { get; set; }
    public DateTime timestamp { get; set; }
    public string source { get; set; }
}