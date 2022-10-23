using AutoFixture;
using Moq;
using NUnit.Framework;
using OuraRingDataIngest.Service.Core.Mappers;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing;

namespace OuraRingDataIngest.Service.Tests.Core.Mappers;

public class HeartRatesMapperTests
{
    [Test]
    public void Map_WhenHeartRatesIsNotNull_ReturnsHeartRatesMapped()
    {
        var fixture = new Fixture();
        var heartRates = fixture.Create<HeartRates>();
        var _heartRatesMapper = new HeartRatesMapper();

        var heartRatesMapped = _heartRatesMapper.Map(heartRates);

        Assert.NotNull(heartRatesMapped);
    }
}