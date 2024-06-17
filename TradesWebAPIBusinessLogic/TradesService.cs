using AutoMapper;
using Microsoft.Extensions.Logging;
using TradesWebAPIDataAccess;
using TradesWebAPISharedLibrary.DTOs;
using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPIBusinessLogic
{
    public class TradesService : ITradesService
    {
        private readonly ILogger<TradesService> _logger;
        private readonly ITradesRepository _tradesRepository;
        private readonly IMapper _mapper;

        public TradesService(ILogger<TradesService> logger, ITradesRepository tradesRepository, IMapper mapper)
        {
            _logger = logger;
            _tradesRepository = tradesRepository;
            _mapper = mapper;
        }
        public async Task CreateEntity(EntityDto entityDto)
        {
            Entity entity = _mapper.Map<Entity>(entityDto);
            await _tradesRepository.CreateEntity(entity);
        }

        public async Task UpdateEntity(EntityDto entityDto)
        {
            Entity entity = _mapper.Map<Entity>(entityDto);
            await _tradesRepository.UpdateEntity(entity);
        }

        public async Task DeleteEntity(string entityId)
        {
            await _tradesRepository.DeleteEntity(entityId);
        }

        public async Task<List<Entity>> GetAllEntities(string? search, string? gender, DateTime? startDate, DateTime? endDate, List<string>? countries, int pageNumber, int pageSize, string? sortBy, bool ascending)
        {
            return await _tradesRepository.GetAllEntities(search, gender, startDate, endDate, countries, pageNumber, pageSize, sortBy, ascending);
        }

        public async Task<Entity?> GetEntityById(string entityId)
        {
            return await _tradesRepository.GetEntityById(entityId);
        }
    }
}
