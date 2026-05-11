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
builder.Services.AddScoped<IAlbumsLogic, AlbumsLogic>();
builder.Services.AddScoped<ILiveSessionsLogic, LiveSessionsLogic>();
builder.Services.AddScoped<IAuthLogic, AuthLogic>();
builder.Services.AddScoped<IMusicBrainzLogic, MusicBrainzLogic>();
builder.Services.AddScoped<IBandsDao, BandsDao>();
builder.Services.AddScoped<IUsersDao, UsersDao>();
builder.Services.AddScoped<ISongsDao, SongsDao>();
builder.Services.AddScoped<ISetlistsDao, SetlistsDao>();
builder.Services.AddScoped<IAlbumsDao, AlbumsDao>();
builder.Services.AddScoped<ILiveSessionsDao, LiveSessionsDao>();
builder.Services.AddMemoryCache();
builder.Services
    .AddHttpClient<IMusicBrainzClient, MusicBrainzClient>(client =>
    {
        client.BaseAddress = new Uri("https://musicbrainz.org/ws/2/");
        client.Timeout = TimeSpan.FromSeconds(20);
    });
builder.Services.AddCors(options =>
{
    options.AddPolicy(RitualClientCors, policy =>
    {
        policy.SetIsOriginAllowed(origin =>
            Uri.TryCreate(origin, UriKind.Absolute, out var uri) &&
            (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps) &&
            uri.IsLoopback)
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

await EnsureMusicBrainzSchemaAsync(app);

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

static async Task EnsureMusicBrainzSchemaAsync(WebApplication app)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<RitualDbContext>();

    await EnsureColumnAsync(db, "bands", "country", "ALTER TABLE `bands` ADD COLUMN `country` VARCHAR(100) NULL AFTER `description`;");
    await EnsureColumnAsync(db, "bands", "musicbrainz_artist_id", "ALTER TABLE `bands` ADD COLUMN `musicbrainz_artist_id` VARCHAR(36) NULL AFTER `country`;");
    await EnsureColumnAsync(db, "albums", "musicbrainz_release_group_id", "ALTER TABLE `albums` ADD COLUMN `musicbrainz_release_group_id` VARCHAR(36) NULL AFTER `title`;");
    await EnsureColumnAsync(db, "albums", "musicbrainz_release_id", "ALTER TABLE `albums` ADD COLUMN `musicbrainz_release_id` VARCHAR(36) NULL AFTER `musicbrainz_release_group_id`;");
    await EnsureColumnAsync(db, "songs", "musicbrainz_recording_id", "ALTER TABLE `songs` ADD COLUMN `musicbrainz_recording_id` VARCHAR(36) NULL AFTER `title`;");
}

static async Task EnsureColumnAsync(RitualDbContext db, string tableName, string columnName, string alterSql)
{
    if (await ColumnExistsAsync(db, tableName, columnName))
    {
        return;
    }

    await db.Database.ExecuteSqlRawAsync(alterSql);
}

static async Task<bool> ColumnExistsAsync(RitualDbContext db, string tableName, string columnName)
{
    var connection = db.Database.GetDbConnection();
    var shouldClose = connection.State != System.Data.ConnectionState.Open;
    if (shouldClose)
    {
        await connection.OpenAsync();
    }

    try
    {
        await using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT COUNT(*)
FROM information_schema.COLUMNS
WHERE TABLE_SCHEMA = DATABASE()
  AND TABLE_NAME = @tableName
  AND COLUMN_NAME = @columnName";

        var tableParam = command.CreateParameter();
        tableParam.ParameterName = "@tableName";
        tableParam.Value = tableName;
        command.Parameters.Add(tableParam);

        var columnParam = command.CreateParameter();
        columnParam.ParameterName = "@columnName";
        columnParam.Value = columnName;
        command.Parameters.Add(columnParam);

        var result = await command.ExecuteScalarAsync();
        return Convert.ToInt32(result) > 0;
    }
    finally
    {
        if (shouldClose)
        {
            await connection.CloseAsync();
        }
    }
}
