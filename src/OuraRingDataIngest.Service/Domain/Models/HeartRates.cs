using System;
using System.Collections.Generic;

namespace OuraRingDataIngest.Service.Domain.Models
{
    public class HeartRates
    {
        public IEnumerable<HeartRate> Data { get; set; }
        public IEnumerable<string> Errors { get; set; }
        public string Detail { get; set; }
    }
}
