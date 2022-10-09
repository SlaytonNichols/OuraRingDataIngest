using System;
using ServiceStack;

namespace OuraRingDataIngest.ServiceModel
{
    public class HeartRate
    {

        public int Bpm { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
