using Microsoft.Data.SqlClient;
using Moq;
using Polly;
using Polly.Retry;
using TradesWebAPIDataAccess;
using TradesWebAPISharedLibrary.Model;
using Xunit;
using Xunit.Abstractions;

namespace TradesWebAPIUnitTest
{
    public class TradesRepositoryRetryTest
    {
        private readonly ITestOutputHelper _output;
        public ITestOutputHelper Output => _output;

        public TradesRepositoryRetryTest(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async Task CreateEntityAsync_ShouldRetry_OnTransientFailure_LastRetrySuccessful()
        {
            //arrange
            var repositoryMock = new Mock<ITradesRepository>();

            var entity = new Entity
            {
                Id = "TestEntityId1",
                Names = new List<Name>() {
                    new Name() { EntityId = "TestEntityId1", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "TestEntityId1", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "TestEntityId1", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            //mock repetitive calls with 4 outcomes (3 failed and last success)
            repositoryMock.SetupSequence(p => p.CreateEntity(entity))
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Returns(Task.CompletedTask);

            // Act
            var retryPolicy = getRetryPolicy();

            await retryPolicy
                   .ExecuteAsync(() =>
                   repositoryMock.Object.CreateEntity(entity));

            Output.WriteLine("Last Retry Successful.");
            // Verify that the method was called 4 times (1 initial + 3 retries)
            repositoryMock.Verify(p => p.CreateEntity(entity), Times.Exactly(4));
        }

        [Fact]
        public async Task CreateEntityAsync_ShouldRetry_OnTransientFailure_AllRetriesFailed()
        {
            //arrange
            var repositoryMock = new Mock<ITradesRepository>();

            var entity = new Entity
            {
                Id = "TestEntityId1",
                Names = new List<Name>() {
                    new Name() { EntityId = "TestEntityId1", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "TestEntityId1", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "TestEntityId1", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            //mock repetitive calls with 3 outcomes (all retries failed)
            repositoryMock.SetupSequence(p => p.CreateEntity(entity))
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Throws(new TimeoutException());

            // Act
            var retryPolicy = getRetryPolicy();

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await retryPolicy.ExecuteAsync(() => repositoryMock.Object.CreateEntity(entity));
            });

            // Verify that the method was called 4 times (1 initial + 3 retries)
            repositoryMock.Verify(p => p.CreateEntity(entity), Times.Exactly(4));
        }

        [Fact]
        public async Task GetEntityById_ShouldRetry_OnTransientFailure_LastRetrySuccessful()
        {
            //arrange
            var repositoryMock = new Mock<ITradesRepository>();

            var entity = new Entity
            {
                Id = "TestEntityId1",
                Names = new List<Name>() {
                    new Name() { EntityId = "TestEntityId1", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "TestEntityId1", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "TestEntityId1", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            //mock repetitive calls with 4 outcomes (3 failed and last success)
            repositoryMock.SetupSequence(p => p.GetEntityById(entity.Id))
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .ReturnsAsync(entity);

            // Act
            var retryPolicy = getRetryPolicy();

            await retryPolicy
                   .ExecuteAsync(() =>
                   repositoryMock.Object.GetEntityById(entity.Id));

            Output.WriteLine("Last Retry Successful.");
            // Verify that the method was called 4 times (1 initial + 3 retries)
            repositoryMock.Verify(p => p.GetEntityById(entity.Id), Times.Exactly(4));
        }

        [Fact]
        public async Task GetEntityById_ShouldRetry_OnTransientFailure_AllRetriesFailed()
        {
            //arrange
            var repositoryMock = new Mock<ITradesRepository>();

            var entity = new Entity
            {
                Id = "TestEntityId1",
                Names = new List<Name>() {
                    new Name() { EntityId = "TestEntityId1", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "TestEntityId1", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "TestEntityId1", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            //mock repetitive calls with 4 outcomes (all retries failed)
            repositoryMock.SetupSequence(p => p.GetEntityById(entity.Id))
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Throws(new TimeoutException())
                .Throws(new TimeoutException());

            // Act
            var retryPolicy = getRetryPolicy();

            // Act & Assert
            await Assert.ThrowsAsync<TimeoutException>(async () =>
            {
                await retryPolicy.ExecuteAsync(() => repositoryMock.Object.GetEntityById(entity.Id));
            });

            // Verify that the method was called 4 times (1 initial + 3 retries)
            repositoryMock.Verify(p => p.GetEntityById(entity.Id), Times.Exactly(4));
        }

        AsyncRetryPolicy getRetryPolicy()
        {
            // Define the retry policy 
            return Policy
            .Handle<SqlException>()
            .Or<TimeoutException>()
            .WaitAndRetryAsync(
             retryCount: 3,
             sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
             onRetry: (exception, timespan, retryCount, context) =>
             {
                 // Log the retry attempt
                 Output.WriteLine($"Retry {retryCount} encountered an exception: {exception.Message}. Waiting {timespan} before next retry.");
             });
        }
    }
}
