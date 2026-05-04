using Microsoft.EntityFrameworkCore;
using server.Dao.Implementations;
using server.Dao.Interfaces;
using server.Data;
using server.Logic.Implementations;
using server.Logic.Interfaces;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.

const string RitualClientCors = "RitualClientCors";

builder.Services.AddDbContext<RitualDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("RitualDb"),
        new MySqlServerVersion(new Version(8, 0, 0))
    )
);

builder.Services.AddControllers();
builder.Services.AddScoped<IHealthLogic, HealthLogic>();
builder.Services.AddScoped<IHealthDao, HealthDao>();
builder.Services.AddScoped<IBandsLogic, BandsLogic>();
builder.Services.AddScoped<ISongsLogic, SongsLogic>();
builder.Services.AddScoped<ISetlistsLogic, SetlistsLogic>();
builder.Services.AddScoped<ILiveSessionsLogic, LiveSessionsLogic>();
builder.Services.AddScoped<IBandsDao, BandsDao>();
builder.Services.AddScoped<ISongsDao, SongsDao>();
builder.Services.AddScoped<ISetlistsDao, SetlistsDao>();
builder.Services.AddScoped<ILiveSessionsDao, LiveSessionsDao>();
builder.Services.AddCors(options =>
{
    options.AddPolicy(RitualClientCors, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(RitualClientCors);

app.UseAuthorization();

app.MapControllers();

app.Run();
