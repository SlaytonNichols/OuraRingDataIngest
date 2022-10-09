using System;
using System.Collections.Generic;
using ServiceStack;
using ServiceStack.DataAnnotations;

namespace OuraRingDataIngest.ServiceModel
{
    [Schema("oura")]
    public class HeartRate
    {
        [AutoIncrement]
        public int Id { get; set; }
        public int Bpm { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }

    [Tag("heart-rate"), Description("Create a new record in the heart rate table")]
    [Route("/heart-rate", "POST")]
    [ValidateHasRole("Admin")]
    public class CreateHeartRate : ICreateDb<HeartRate>, IReturn<IdResponse>
    {
        public int Bpm { get; set; }
        public string Source { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
