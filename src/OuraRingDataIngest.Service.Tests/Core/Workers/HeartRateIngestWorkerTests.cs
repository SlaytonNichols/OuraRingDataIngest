using System;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using Azure.Storage.Files.DataLake;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using OuraRingDataIngest.Service.Core.Dtos;
using OuraRingDataIngest.Service.Core.Managers;
using OuraRingDataIngest.Service.Core.Mappers;
using OuraRingDataIngest.Service.Core.Workers.HeartRateIngestWorker;
using OuraRingDataIngest.Service.Infrastructure.Adls;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing;
using OuraRingDataIngest.Service.Infrastructure.HttpClients.OuraRing.OuraRingClient;
using ServiceStack;

namespace OuraRingDataIngest.Service.Tests.Core.Workers;

public class HeartRateIngestWorkerTests
{
    [Test]
    public async Task Api_Success()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Create<HeartRateIngestWorkerRequest>();
        request.CronInfo = null;
        var logger = new Mock<ILogger<HeartRateIngestWorker>>();
        var dateManager = new Mock<IHeartRateIngestWorkerDateManager>();
        var ouraRingClient = new Mock<IOuraRingClient>();
        var mapper = new Mock<IHeartRatesMapper>();
        var adlsClient = new Mock<IAdlsClient>();

        var dateRespose = fixture.Create<(DateTime, DateTime)>();
        dateManager.Setup(x => x.GetQueryDates(request)).Returns(dateRespose);
        ouraRingClient.Setup(x => x.BuildUri(dateRespose.Item1, dateRespose.Item2)).Returns("uri");
        var heartRates = fixture.Build<HeartRates>().With(x => x.Data, fixture.CreateMany<HeartRate>().ToList()).Create();
        ouraRingClient.Setup(x => x.GetHeartRatesAsync("uri")).ReturnsAsync(heartRates);
        heartRates.Errors = null;

        var dirClient = new Mock<DataLakeDirectoryClient>();
        adlsClient.Setup(x => x.CreateHeartRatesDirectoryIfNotExists()).ReturnsAsync(dirClient.Object);
        adlsClient.Setup(x => x.WriteJsonToDirectory(dirClient.Object, heartRates.ToJson())).Returns(Task.CompletedTask);

        // Act
        var response = await new HeartRateIngestWorker(logger.Object, ouraRingClient.Object, mapper.Object, adlsClient.Object, dateManager.Object).ExecuteAsync(request);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Results);

    }

    [Test]
    public async Task Cron_Success()
    {
        // Arrange
        var fixture = new Fixture();
        var request = fixture.Create<HeartRateIngestWorkerRequest>();
        request.StartQueryDate = null;
        request.EndQueryDate = null;
        var logger = new Mock<ILogger<HeartRateIngestWorker>>();
        var dateManager = new Mock<IHeartRateIngestWorkerDateManager>();
        var ouraRingClient = new Mock<IOuraRingClient>();
        var mapper = new Mock<IHeartRatesMapper>();
        var adlsClient = new Mock<IAdlsClient>();

        var dateRespose = fixture.Create<(DateTime, DateTime)>();
        dateManager.Setup(x => x.GetQueryDates(request)).Returns(dateRespose);
        ouraRingClient.Setup(x => x.BuildUri(dateRespose.Item1, dateRespose.Item2)).Returns("uri");
        var heartRates = fixture.Build<HeartRates>().With(x => x.Data, fixture.CreateMany<HeartRate>().ToList()).Create();
        ouraRingClient.Setup(x => x.GetHeartRatesAsync("uri")).ReturnsAsync(heartRates);
        heartRates.Errors = null;

        var dirClient = new Mock<DataLakeDirectoryClient>();
        adlsClient.Setup(x => x.CreateHeartRatesDirectoryIfNotExists()).ReturnsAsync(dirClient.Object);
        adlsClient.Setup(x => x.WriteJsonToDirectory(dirClient.Object, heartRates.ToJson())).Returns(Task.CompletedTask);

        // Act
        var response = await new HeartRateIngestWorker(logger.Object, ouraRingClient.Object, mapper.Object, adlsClient.Object, dateManager.Object).ExecuteAsync(request);

        // Assert
        Assert.Null(response.Errors);
        Assert.NotNull(response.Results);

    }
}