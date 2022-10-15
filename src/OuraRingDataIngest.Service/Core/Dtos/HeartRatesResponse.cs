using System;
using OuraRingDataIngest.Service.Domain.Models;
using ServiceStack;

namespace OuraRingDataIngest.Service.Core.Dtos;

public class HeartRatesResponse
{
    public HeartRates HeartRates { get; set; }
}