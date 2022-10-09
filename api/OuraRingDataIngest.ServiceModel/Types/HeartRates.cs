using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace OuraRingDataIngest.ServiceModel
{
    public class HeartRates
    {
        public IEnumerable<HeartRate> Data { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
