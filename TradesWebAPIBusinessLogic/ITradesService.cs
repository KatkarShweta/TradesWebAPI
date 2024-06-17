using TradesWebAPISharedLibrary.DTOs;
using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPIBusinessLogic
{
    public interface ITradesService
    {
        public Task CreateEntity(EntityDto entity);
        public Task UpdateEntity(EntityDto entity);
        public Task DeleteEntity(string entityId);
        public Task<List<Entity>> GetAllEntities(string? search, string? gender, DateTime? startDate, DateTime? endDate, List<string>? countries, int pageNumber, int pageSize, string? sortBy, bool ascending);
        public Task<Entity?> GetEntityById(string entityId);        
    }
}
