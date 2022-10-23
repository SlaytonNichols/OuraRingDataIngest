using System;
using AutoFixture;
using NUnit.Framework;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Core.Managers;
using SlaytonNichols.Common.Infrastructure.Cron;

namespace OuraRingDataIngest.Service.Tests.Core.Managers;

public class HeartRateIngestWorkerDateManagerTests
{
    [Test]
    public void GetQueryDates_WhenRequestStartQueryDateAndEndQueryDateAreNotNull_ReturnsStartQueryDateAndEndQueryDate()
    {
        var fixture = new Fixture();
        var request = fixture.Build<HeartRateIngestWorkerRequest>()
            .With(x => x.StartQueryDate, DateTime.Now)
            .With(x => x.EndQueryDate, DateTime.Now.AddDays(1))
            .Create();
        var _heartRateIngestWorkerDateManager = new HeartRateIngestWorkerDateManager();

        var (startQueryDate, endQueryDate) = _heartRateIngestWorkerDateManager.GetQueryDates(request);

        Assert.NotNull(startQueryDate);
        Assert.NotNull(endQueryDate);
    }

    [Test]
    public void GetQueryDates_WhenRequestStartQueryDateAndEndQueryDateAreNull_ReturnsCronInfoStartQueryDateAndCronInfoEndQueryDate()
    {
        var fixture = new Fixture();
        var request = fixture.Build<HeartRateIngestWorkerRequest>()
            .With(x => x.StartQueryDate, (DateTime?)null)
            .With(x => x.EndQueryDate, (DateTime?)null)
            .With(x => x.CronInfo,
            fixture.Build<CronInfo>()
            .With(x => x.StartQueryDate, DateTime.Now)
            .With(x => x.EndQueryDate, DateTime.Now.AddDays(1)).Create())
            .Create();
        var _heartRateIngestWorkerDateManager = new HeartRateIngestWorkerDateManager();

        var (startQueryDate, endQueryDate) = _heartRateIngestWorkerDateManager.GetQueryDates(request);

        Assert.NotNull(startQueryDate);
        Assert.NotNull(endQueryDate);
    }
}