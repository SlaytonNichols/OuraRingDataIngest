using System;
using System.Collections.Generic;

namespace OuraRingDataIngest.Service.Domain.Models
{
    public class HeartRate
    {
        public int Bpm { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
