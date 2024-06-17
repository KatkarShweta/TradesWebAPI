using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using TradesWebAPISharedLibrary.Model;

namespace TradesWebAPIDataAccess
{
    public class TradesRepository : ITradesRepository
    {
        private readonly ILogger<TradesRepository> _logger;
        private readonly TradesDbContext _context;
        private Func<TradesDbContext> context;
        private readonly AsyncRetryPolicy _retryPolicy;

        public TradesRepository(TradesDbContext context, ILogger<TradesRepository> logger)
        {
            _context = context;
            _retryPolicy= getDefaultRetryPolicy();
            _logger = logger;
        }

        public async Task CreateEntity(Entity entity)
        {
            await _retryPolicy.ExecuteAsync(async () =>
            {
                _context.Entity.Add(entity);
                 await _context.SaveChangesAsync();
            });
        }

        public async Task UpdateEntity(Entity updatedEntity)
        {
            
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var existingEntity = await _context.Entity
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == updatedEntity.Id);

                if (existingEntity != null)
                {
                    // Update Deceased & Gender
                    existingEntity.Deceased = updatedEntity.Deceased;
                    existingEntity.Gender = !string.IsNullOrWhiteSpace(updatedEntity.Gender)
                        ? updatedEntity.Gender : existingEntity.Gender;

                    // Update Addresses
                    if (updatedEntity.Addresses != null && updatedEntity.Addresses.Count > 0)
                    {                        
                        existingEntity.Addresses = updatedEntity.Addresses;
                    }

                    // Update Dates
                    if (updatedEntity.Dates != null && updatedEntity.Dates.Count > 0)
                    {
                        existingEntity.Dates = updatedEntity.Dates;
                    }

                    // Update Names
                    if (updatedEntity.Names != null && updatedEntity.Names.Count > 0)
                    {                        
                        existingEntity.Names = updatedEntity.Names;
                    }
                    _context.Entity.Update(existingEntity);
                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task DeleteEntity(string entityId)
        {
            
            await _retryPolicy.ExecuteAsync(async () =>
            {
                var entity = await _context.Entity
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == entityId);

                if (entity != null)
                {
                    _context.Address.RemoveRange(entity.Addresses);
                    _context.Date.RemoveRange(entity.Dates);
                    _context.Name.RemoveRange(entity.Names);
                    _context.Entity.Remove(entity);

                    await _context.SaveChangesAsync();
                }
            });
        }

        public async Task<List<Entity>> GetAllEntities(string? search, string? gender, DateTime? startDate, DateTime? endDate, List<string>? countries,
            int pageNumber = 1, int pageSize = 10, string? sortBy = null, bool ascending = true)
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var query = _context.Entity
                   .Include(e => e.Addresses)
                   .Include(e => e.Dates)
                   .Include(e => e.Names)
                   .AsSplitQuery()
                   .AsQueryable();

                if (!string.IsNullOrWhiteSpace(search))
                {
                    // Split the search string by spaces (URL encoded as %20)
                    var searchStrings = search.ToLower().Replace("%20", " ").Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                    foreach (var searchTerm in searchStrings)
                    {
                        query = query.Where(e =>
                                e.Addresses.Any(a => a.Country.ToLower().Contains(searchTerm) ||
                                                     a.AddressLine.ToLower().Contains(searchTerm)) ||
                                e.Names.Any(n => n.FirstName.ToLower().Contains(searchTerm) ||
                                                 n.MiddleName.ToLower().Contains(searchTerm) ||
                                                 n.Surname.ToLower().Contains(searchTerm)));
                    }
                }

                if (!string.IsNullOrEmpty(gender))
                {
                    query = query.Where(e => e.Gender == gender);
                }

                if (startDate.HasValue)
                {
                    query = query.Where(e => e.Dates.Any(d => d.Date.HasValue && d.Date.Value.Date >= startDate.Value.Date));
                }

                if (endDate.HasValue)
                {
                    query = query.Where(e => e.Dates.Any(d => d.Date.HasValue && d.Date.Value.Date <= endDate.Value.Date));
                }

                if (countries != null && countries.Any())
                {
                    query = query.Where(e => e.Addresses.Any(a => countries.Contains(a.Country)));
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    switch (sortBy.ToLower())
                    {
                        case "firstname":
                            query = ascending ? query.OrderBy(e => e.Names.FirstOrDefault().FirstName) : query.OrderByDescending(e => e.Names.FirstOrDefault().FirstName);
                            break;
                        case "lastname":
                            query = ascending ? query.OrderBy(e => e.Names.FirstOrDefault().Surname) : query.OrderByDescending(e => e.Names.FirstOrDefault().Surname);
                            break;
                        case "date":
                            query = ascending ? query.OrderBy(e => e.Dates.FirstOrDefault().Date) : query.OrderByDescending(e => e.Dates.FirstOrDefault().Date);
                            break;
                        default:
                            query = ascending ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id);
                            break;
                    }
                }
                else
                {
                    query = ascending ? query.OrderBy(e => e.Id) : query.OrderByDescending(e => e.Id);
                }
                return await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            });
        }

        public async Task<Entity?> GetEntityById(string entityId)
        {            
            return await _retryPolicy.ExecuteAsync(async () =>
            {                
                return await _context.Entity
                .Include(e => e.Addresses)
                .Include(e => e.Dates)
                .Include(e => e.Names)
                .AsSplitQuery()
                .FirstOrDefaultAsync(e => e.Id == entityId);
            });
        }

        public  AsyncRetryPolicy getDefaultRetryPolicy()
        {
            return Policy
             .Handle<SqlException>()
             .Or<TimeoutException>()
             .WaitAndRetryAsync(
                 retryCount: 3,
                 sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)),
                 onRetry: (exception, timespan, retryCount, context) =>
                 {
                     // Log the retry attempt
                     _logger.LogInformation($"Retry {retryCount} encountered an exception: {exception.Message}. Waiting {timespan} before next retry.");
                 });

        }
    }
}
