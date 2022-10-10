using System;
using ServiceStack;
using OuraRingDataIngest.ServiceModel;

namespace OuraRingDataIngest.ServiceInterface
{
    public class MyServices : Service
    {
        public object Any(Hello request)
        {
            return new HelloResponse { Result = $"Hello, {request.Name}!" };
        }
    }
}
