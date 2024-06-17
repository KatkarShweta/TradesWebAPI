using Microsoft.EntityFrameworkCore;
using TradesWebAPI.MappingProfile;
using TradesWebAPIBusinessLogic;
using TradesWebAPIDataAccess;
using TradesWebAPISharedLibrary.ExceptionHandler;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ITradesService, TradesService>();
builder.Services.AddScoped<ITradesRepository, TradesRepository>();

// Configure database context
builder.Services.AddDbContext<TradesDbContext>(options =>
{
    IConfiguration configuration = builder.Configuration;
    string connectionString = configuration.GetConnectionString("TradesDBConnection");
    options.UseSqlServer(connectionString);
});
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.AddTransient<ExceptionHandlingMiddleware>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
