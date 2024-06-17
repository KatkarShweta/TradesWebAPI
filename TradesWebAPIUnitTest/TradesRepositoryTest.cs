using Microsoft.Extensions.Logging;
using Moq;
using TradesWebAPIDataAccess;
using TradesWebAPISharedLibrary.Model;
using Xunit;

namespace TradesWebAPIUnitTest
{
    [Collection("InMemory Database collection")]
    public class TradesRepositoryTest
    {
        private readonly InMemoryDatabaseFixture _fixture;
              
        public TradesRepositoryTest(InMemoryDatabaseFixture fixture)
        {
            _fixture = fixture;            
        }

        [Fact]
        public async Task CreateEntity_ValidEntity_CreatesEntity()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();
            
            var repository = new TradesRepository(context, loggerMock.Object);
            var entity = new Entity
            {
                Id = "EntityCreate",
                Names = new List<Name>() {
                    new Name() { EntityId = "EntityCreate", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "EntityCreate", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "Argentina" }
                },
                Dates = new List<Dates> { new Dates {EntityId= "EntityCreate", DateType="DOB", Date=Convert.ToDateTime("1980-01-01")} }
            };
                        
            // Act
            await repository.CreateEntity(entity);

            // Assert
            var createdEntity = await context.Entity.FindAsync(entity.Id);
            Assert.NotNull(createdEntity);
            Assert.Equal(entity.Id, createdEntity.Id);

            // flush out the added TradeEntity record
            context.Remove(entity);
            context.SaveChanges();
        }

        [Fact]
        public async Task UpdateEntity_ValidEntity_UpdatesEntitySuccessfully()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();

            var repository = new TradesRepository(context, loggerMock.Object);
            var entity = new Entity
            {
                Id = "EntityUpdate1",
                Names = new List<Name>() {
                    new Name() { EntityId = "EntityUpdate1", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "EntityUpdate1", AddressLine = "1600 Pennsylvania Avenue", City = "City", Country = "Ireland" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "EntityUpdate1", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            // Act
            await repository.CreateEntity(entity);

            // Assert
            var createdEntity = await context.Entity.FindAsync(entity.Id);
            Assert.NotNull(createdEntity);
            Assert.NotNull(createdEntity.Id);
            Assert.NotEmpty(createdEntity.Addresses);

            var address = createdEntity.Addresses.First();
            Assert.Equal("1600 Pennsylvania Avenue", address.AddressLine);
            Assert.Equal("City", address.City);
            Assert.Equal("Ireland", address.Country);

            //Updating the City
            entity.Addresses.First().City = "Manchester";

            // Act
            await repository.UpdateEntity(entity);

            // Assert
            var updatedEntity = await context.Entity.FindAsync(entity.Id);
            Assert.NotNull(updatedEntity);
            Assert.NotNull(updatedEntity.Id);
            Assert.NotEmpty(updatedEntity.Addresses);

            address = updatedEntity.Addresses.First();
            Assert.Equal("1600 Pennsylvania Avenue", address.AddressLine);
            Assert.Equal("Manchester", address.City);
            Assert.Equal("Ireland", address.Country);

            // flush out the added TradeEntity record
            context.Remove(entity);
            context.SaveChanges();
        }

        [Fact]
        public async Task DeleteEntity_ValidEntity_DeletesSuccessfully()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();

            var repository = new TradesRepository(context, loggerMock.Object);
            var entity = new Entity
            {
                Id = "EntityDelete",
                Names = new List<Name>() {
                    new Name() { EntityId = "EntityDelete", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "EntityDelete", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "EntityDelete", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            // Act
            await repository.CreateEntity(entity);

            // Assert
            var createdEntity = await context.Entity.FindAsync(entity.Id);
            Assert.NotNull(createdEntity);
            Assert.Equal(entity.Id, createdEntity.Id);
            
            // Act
            await repository.DeleteEntity(entity.Id);

            // Assert
            var deletedEntity = await context.Entity.FindAsync(entity.Id);
            Assert.Null(deletedEntity);            
        }

        [Fact]
        public async Task GetEntityById_ValidEntity_Exists()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();

            var repository = new TradesRepository(context, loggerMock.Object);
            var entity = new Entity
            {
                Id = "EntityById",
                Names = new List<Name>() {
                    new Name() { EntityId = "EntityById", FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                Addresses = new List<Address> {
                    new Address { EntityId = "EntityById", AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                Dates = new List<Dates> { new Dates { EntityId = "EntityById", DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
            };

            // Act
            await repository.CreateEntity(entity);
            
            // Assert
            var resultEntityById = await repository.GetEntityById(entity.Id);
            Assert.NotNull(resultEntityById);
            Assert.Equal(entity.Id, resultEntityById.Id);

            // flush out the added TradeEntity record
            context.Remove(entity);
            context.SaveChanges();
        }

        [Fact]
        public async Task GetAllEntities_ValidEntities_SearchByCountry()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();

            var repository = new TradesRepository(context, loggerMock.Object);
            foreach (var index in Enumerable.Range(1, 10))
            {
                var entity = new Entity
                {
                    Id = "EntityIdCountrySearch" + index,
                    Names = new List<Name>() {
                    new Name() { EntityId = "EntityIdCountrySearch" + index, FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                    Addresses = new List<Address> {
                    new Address { EntityId = "EntityIdCountrySearch" + index, AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "INDIA" }
                },
                    Dates = new List<Dates> { new Dates { EntityId = "EntityIdCountrySearch" + index, DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
                };

                // Act
                await repository.CreateEntity(entity);
            }
            foreach (var index in Enumerable.Range(11, 20))
            {
                var entity = new Entity
                {
                    Id = "EntityIdCountrySearch" + index,
                    Names = new List<Name>() {
                    new Name() { EntityId = "EntityIdCountrySearch" + index, FirstName = "DavidU", MiddleName = "JamesU", Surname = "WilsonU" }
                },
                    Addresses = new List<Address> {
                    new Address { EntityId = "EntityIdCountrySearch" + index, AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "JAPAN" }
                },
                    Dates = new List<Dates> { new Dates { EntityId = "EntityIdCountrySearch" + index, DateType = "DOB", Date = Convert.ToDateTime("1980-01-01") } }
                };

                // Act
                await repository.CreateEntity(entity);
            }

            string? search = "";
            string? gender = "";
            DateTime? startDate = null;
            DateTime? endDate = null;
            List<string>? countries = new List<string> { "JAPAN", "INDIA" };
            int pageNumber = 1;
            int pageSize = 20;
            string? sortBy = "Name";
            bool ascending = false;
            var searchResult  = await repository.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending);

            // flush out the added TradeEntity record
             //context.DisposeAsync();
        }


        [Fact]
        public async Task GetAllEntities_ValidEntities_SearchByDate()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();

            var repository = new TradesRepository(context, loggerMock.Object);
            foreach (var index in Enumerable.Range(1, 5))
            {
                var entity = new Entity
                {
                    Id = "EntityIdSerachByDate" + index,
                    Names = new List<Name>() {
                    new Name() { EntityId = "EntityIdSerachByDate" + index, FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                    Addresses = new List<Address> {
                    new Address { EntityId = "EntityIdSerachByDate" + index, AddressLine = "1600 Pennsylvania Avenue", City = "Leeds", Country = "UK" }
                },
                    Dates = new List<Dates> { new Dates { EntityId = "EntityIdSerachByDate" + index, DateType = "DOB", Date = DateTime.Now.AddDays(+index) } }
                };

                // Act
                await repository.CreateEntity(entity);
            }
            
            string? search = "";
            string? gender = "";
            DateTime? startDate = DateTime.Now.AddDays(1);
            DateTime? endDate = DateTime.Now.AddDays(5); ;
            List<string>? countries = new List<string> { "USA", "UK" };
            int pageNumber = 1;
            int pageSize = 20;
            string? sortBy = "Name";
            bool ascending = false;
            var searchResult = await repository.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending);

            Assert.NotNull(searchResult);
            Assert.Equal(5, searchResult.Count);

            foreach (var entity in searchResult)
            {
                Assert.NotNull(entity.Id);
                Assert.NotEmpty(entity.Names);
                Assert.NotEmpty(entity.Addresses);
                Assert.NotEmpty(entity.Dates);

                var name = entity.Names.First();
                Assert.Equal("David", name.FirstName);
                Assert.Equal("James", name.MiddleName);
                Assert.Equal("Wilson", name.Surname);

                var address = entity.Addresses.First();
                Assert.Equal("1600 Pennsylvania Avenue", address.AddressLine);
                Assert.Equal("Leeds", address.City);
                Assert.Equal("UK", address.Country);

                var date = entity.Dates.First();
                Assert.Equal("DOB", date.DateType);
                Assert.True(date.Date > DateTime.Now);
                Assert.True(date.Date < DateTime.Now.AddDays(5));
            }

            // flush out the added TradeEntity record            
            //context.DisposeAsync();
        }

        [Fact]
        public async Task GetAllEntities_ValidEntities_SearchBySearchQuery()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesRepository>>();
            var context = _fixture.CreateContext();

            var repository = new TradesRepository(context, loggerMock.Object);
            foreach (var index in Enumerable.Range(100, 10))
            {
                var entity = new Entity
                {
                    Id = "EntityIdSearchQuery" + index,
                    Names = new List<Name>() {
                    new Name() { EntityId = "EntityIdSearchQuery" + index, FirstName = "David", MiddleName = "James", Surname = "Wilson" }
                },
                    Addresses = new List<Address> {
                    new Address { EntityId = "EntityIdSearchQuery" + index, AddressLine = "1600 PennsylvaniaWest Land", City = "Leeds", Country = "UK" }
                },
                    Dates = new List<Dates> { new Dates { EntityId = "EntityIdSearchQuery" + index, DateType = "DOB", Date = DateTime.Now.AddDays(2) } }
                };

                // Act
                await repository.CreateEntity(entity);
            }
                       
            string? search = "PennsylvaniaWest%20Land";
            string? gender = "";
            DateTime? startDate = null;
            DateTime? endDate = null;
            List<string>? countries = null;
            int pageNumber = 1;
            int pageSize = 20;
            string? sortBy = "Name";
            bool ascending = false;
            var searchResult = await repository.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending);

            Assert.NotNull(searchResult);
            Assert.Equal(10, searchResult.Count);

            foreach (var entity in searchResult)
            {
                var address = entity.Addresses.First();
               
                Assert.True(address.AddressLine.Contains("PennsylvaniaWest"));
                Assert.True(address.AddressLine.Contains("Land"));
                Assert.Equal("Leeds", address.City);
                Assert.Equal("UK", address.Country);

                var date = entity.Dates.First();
                Assert.Equal("DOB", date.DateType);
                Assert.True(date.Date > DateTime.Now);
                Assert.True(date.Date < DateTime.Now.AddDays(10));
            }
                      
            // flush out the added TradeEntity record
            //await context.DisposeAsync();
        }
    }
}
