using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using TradesWebAPI.Controllers;
using TradesWebAPIBusinessLogic;
using TradesWebAPISharedLibrary.DTOs;
using TradesWebAPISharedLibrary.ExceptionHandler;
using TradesWebAPISharedLibrary.Model;
using Xunit;

namespace TradesWebAPIUnitTest
{
    public class TradesControllerTest
    {
        [Fact]
        public async Task CreateEntity_ValidEntity_ReturnsCreatedAtActionResult()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityDto = new EntityDto { };

            // Act
            var actionResult = await controller.CreateEntity(entityDto);

            // Assert
            var result = Assert.IsType<CreatedAtActionResult>(actionResult.Result);
            Assert.NotNull(result);
        }

        [Fact]
        public async Task CreateEntity_NullEntityDto_ThrowsBadRequestException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            EntityDto entityDto = null;

            // Act & Assert
            var ex = await Assert.ThrowsAsync<TradesWebAPISharedLibrary.ExceptionHandler.BadRequestException>(() => controller.CreateEntity(entityDto));
            Assert.Equal("Invalid Entity", ex.Message);
        }

        [Fact]
        public async Task UpdateEntity_ValidData_ReturnsOkResult()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityDto = new EntityDto { Id = "1" };

            // Act
            var actionResult = await controller.UpdateEntity(entityDto.Id, entityDto);

            // Assert
            var result = Assert.IsType<ActionResult<EntityDto>>(actionResult);            
            Assert.NotNull(result);
        }

        [Fact]
        public async Task UpdateEntity_NullEntityDto_ThrowsBadRequestException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            EntityDto entityDto = null;
            string id = "1";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => controller.UpdateEntity(id, entityDto));
            Assert.Equal("Invalid Entity data or Entity ID mismatch", ex.Message);
        }

        [Fact]
        public async Task UpdateEntity_IdMismatch_ThrowsBadRequestException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityDto = new EntityDto { Id = "2"};
            string id = "1";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => controller.UpdateEntity(id, entityDto));
            Assert.Equal("Invalid Entity data or Entity ID mismatch", ex.Message);
        }
                
        [Fact]
        public async Task GetEntityById_ValidId_ReturnsOkResultWithEntity()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityId = "1";
            var entity = new Entity { Id = entityId};
            tradesServiceMock.Setup(service => service.GetEntityById(entityId)).ReturnsAsync(entity);

            // Act
            var result = await controller.GetEntityById(entityId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEntity = Assert.IsType<Entity>(okResult.Value);
            Assert.Equal(entity, returnedEntity);
            tradesServiceMock.Verify(service => service.GetEntityById(entityId), Times.Once);            
        }

        [Fact]
        public async Task GetEntityById_InvalidId_ThrowsBadRequestException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            string invalidId = "";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => controller.GetEntityById(invalidId));
            Assert.Equal("Invalid Id", ex.Message);
            tradesServiceMock.Verify(service => service.GetEntityById(It.IsAny<string>()), Times.Never);            
        }

        [Fact]
        public async Task GetEntityById_EntityNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityId = "1";
            tradesServiceMock.Setup(service => service.GetEntityById(entityId)).ReturnsAsync((Entity)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.GetEntityById(entityId));
            tradesServiceMock.Verify(service => service.GetEntityById(entityId), Times.Once);            
        }
        
        [Fact]
        public async Task GetAllEntities_DefaultParameters_ReturnsOkResultWithEntities()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entities = new List<Entity> { new Entity { } };
            tradesServiceMock.Setup(service => service.GetAllEntities(null, null, null, null, null, 1, 10, null, true))
                             .ReturnsAsync(entities);

            // Act
            var result = await controller.GetAllEntities();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEntities = Assert.IsType<List<Entity>>(okResult.Value);
            Assert.Equal(entities, returnedEntities);
            tradesServiceMock.Verify(service => service.GetAllEntities(null, null, null, null, null, 1, 10, null, true), Times.Once);            
        }

        [Fact]
        public async Task GetAllEntities_SpecificParameters_ReturnsOkResultWithEntities()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var search = "example";
            var gender = "male";
            var startDate = new DateTime(2023, 1, 1);
            var endDate = new DateTime(2023, 12, 31);
            var countries = new List<string> { "USA", "Canada" };
            var pageNumber = 2;
            var pageSize = 20;
            var sortBy = "name";
            var ascending = false;

            var entities = new List<Entity> { new Entity { } };
            tradesServiceMock.Setup(service => service.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending))
                             .ReturnsAsync(entities);

            // Act
            var result = await controller.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedEntities = Assert.IsType<List<Entity>>(okResult.Value);
            Assert.Equal(entities, returnedEntities);
            tradesServiceMock.Verify(service => service.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending), Times.Once);            
        }

        [Fact]
        public async Task DeleteEntity_ValidId_ReturnsOkResult()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityId = "1";
            tradesServiceMock.Setup(service => service.DeleteEntity(entityId)).Returns(Task.CompletedTask);

            // Act
            var result = await controller.DeleteEntity(entityId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            tradesServiceMock.Verify(service => service.DeleteEntity(entityId), Times.Once);            
        }

        [Fact]
        public async Task DeleteEntity_InvalidId_ThrowsBadRequestException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            string invalidId = "";

            // Act & Assert
            var ex = await Assert.ThrowsAsync<BadRequestException>(() => controller.DeleteEntity(invalidId));
            Assert.Equal("Invalid Id", ex.Message);
            tradesServiceMock.Verify(service => service.DeleteEntity(It.IsAny<string>()), Times.Never);            
        }

        [Fact]
        public async Task DeleteEntity_EntityNotFound_ThrowsNotFoundException()
        {
            // Arrange
            var loggerMock = new Mock<ILogger<TradesController>>();
            var tradesServiceMock = new Mock<ITradesService>();
            var controller = new TradesController(loggerMock.Object, tradesServiceMock.Object);

            var entityId = "1";
            tradesServiceMock.Setup(service => service.DeleteEntity(entityId)).ThrowsAsync(new NotFoundException());

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => controller.DeleteEntity(entityId));
            tradesServiceMock.Verify(service => service.DeleteEntity(entityId), Times.Once);            
        }
    }
}

