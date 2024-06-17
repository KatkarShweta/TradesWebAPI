using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPIDataAccess
{
    public interface ITradesRepository
    {
        public Task CreateEntity(Entity entity);
        public Task UpdateEntity(Entity entity);
        public Task DeleteEntity(string entityId);
        public Task<List<Entity>> GetAllEntities(string? search, string? gender, 
            DateTime? startDate, DateTime? endDate, List<string>? countries, 
            int pageNumber, int pageSize, string? sortBy, bool ascending);
        public Task<Entity?> GetEntityById(string entityId);
    }
}
