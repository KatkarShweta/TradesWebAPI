using Microsoft.AspNetCore.Mvc;
using TradesWebAPIBusinessLogic;
using TradesWebAPISharedLibrary.DTOs;
using TradesWebAPISharedLibrary.ExceptionHandler;
using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TradesController : ControllerBase
    {
        private readonly ILogger<TradesController> _logger;
        private readonly ITradesService _tradesService;

        public TradesController(ILogger<TradesController> logger, ITradesService tradesService)
        {
            _logger = logger;
            _tradesService = tradesService;
        }

        /// <summary>
        /// Create New Entity and corresponding records for Address, Date and Name.
        /// </summary>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        /// <exception cref="BadRequestException"></exception>
        [HttpPost]
        public async Task<ActionResult<EntityDto>> CreateEntity(EntityDto entityDto)
        {
            _logger.LogInformation("POST request received to create new Entity");

            if (entityDto == null)
                throw new BadRequestException("Invalid Entity");

            await _tradesService.CreateEntity(entityDto);
            _logger.LogInformation($"POST request executed successfully for Entity ID {entityDto.Id}.");
            return CreatedAtAction(nameof(CreateEntity), entityDto);
        }

        /// <summary>
        /// Update the Entity and corresponding Address, Date and Name.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entityDto"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<EntityDto>> UpdateEntity(string id, [FromBody]EntityDto entityDto)
        {
            _logger.LogInformation($"PUT request received to update the Entity with ID: {id}.");
            
            if (entityDto == null || id != entityDto.Id)
            {
                throw new BadRequestException("Invalid Entity data or Entity ID mismatch");
            }

            await _tradesService.UpdateEntity(entityDto);
            _logger.LogInformation($"PUT request executed successfully for Entity with ID = {id}");
            return Ok(entityDto);
        }

        /// <summary>
        /// Delete Entity and it's corresponding Address, Date and Name 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEntity(string id)
        {
            _logger.LogInformation($"DELETE request received to delete the Entity with ID: {id}.");

            if (string.IsNullOrWhiteSpace(id))
            {
                throw new BadRequestException("Invalid Id");
            }

            await _tradesService.DeleteEntity(id);
            _logger.LogInformation($"DELETE request executed successfully for Entity with ID: {id}.");
            return Ok();
        }

        /// <summary>
        /// Get all Entities
        /// </summary>
        /// <param name="search"></param>
        /// <param name="gender"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="countries"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<List<Entity>>> GetAllEntities(
            [FromQuery] string? search = null,
            [FromQuery] string? gender = null,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] List<string>? countries = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? sortBy = null,
            [FromQuery] bool ascending = true
            )
        {
            _logger.LogInformation("GET request received for getting all entities");
            var entities = await _tradesService.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending);                                    
            return Ok(entities);
        }
        
        /// <summary>
        /// Get Entity by ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Entity>> GetEntityById(string id)
        {
            _logger.LogInformation($"GET request received for getting entity with Id {id}");

            if (string.IsNullOrWhiteSpace(id))
                throw new BadRequestException("Invalid Id");

            var entity = await _tradesService.GetEntityById(id);

            if (entity == null)
            {
                throw new NotFoundException();
            }
            return Ok(entity);
        }
    }
}