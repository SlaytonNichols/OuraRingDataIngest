using System;
using ServiceStack;
using OuraRingDataIngest.ServiceModel;

namespace OuraRingDataIngest.ServiceInterface
{
    public class IngestService : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }
    }
}
