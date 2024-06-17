using Microsoft.EntityFrameworkCore;
using TradesWebAPIDataAccess;
using Xunit;

namespace TradesWebAPIUnitTest
{
    public class InMemoryDatabaseFixture : IDisposable
    {
        private readonly DbContextOptions<TradesDbContext> _options;

        public TradesDbContext Context { get; set; }

        public InMemoryDatabaseFixture()
        {
            var _options = new DbContextOptionsBuilder<TradesDbContext>()
                .UseInMemoryDatabase($"MasterTestDataBase{Guid.NewGuid().ToString()}")
                .EnableSensitiveDataLogging()
                .Options;

            Context= new TradesDbContext(_options); 
        }
               
        public TradesDbContext CreateContext()
        {
            var _options = new DbContextOptionsBuilder<TradesDbContext>()
                .UseInMemoryDatabase("ConextTestDataBase"+Guid.NewGuid().ToString())
                .EnableSensitiveDataLogging()
                .Options;

            return new TradesDbContext(_options);
        }
        
        public void Dispose()
        {
            
        }
    }

    [CollectionDefinition("InMemory Database collection")]
    public class InMemoryDatabaseCollection : ICollectionFixture<InMemoryDatabaseFixture> { }
}
