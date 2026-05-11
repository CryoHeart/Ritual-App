using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using server.Dao.Implementations;
using server.Dao.Interfaces;
using server.Data;
using server.Logic.Implementations;
using server.Logic.Interfaces;
using DotNetEnv;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://localhost:5000");

var envPath = Path.Combine(builder.Environment.ContentRootPath, ".env");
if (File.Exists(envPath))
{
    Env.Load(envPath);
}

// Add services to the container.

const string RitualClientCors = "RitualClientCors";

DatabaseSettings dbSettings;
string connectionString;
try
{
    dbSettings = GetDatabaseSettings();
    connectionString = BuildConnectionString(dbSettings);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"[DB CONFIG ERROR] {ex.Message}");
    throw;
}

builder.Services.AddDbContext<RitualDbContext>(options =>
    options.UseMySql(
        connectionString,
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

await ValidateDatabaseConnectionAsync(app, dbSettings);
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

static string BuildConnectionString(DatabaseSettings settings)
{
    var builder = new MySqlConnectionStringBuilder
    {
        Server = settings.Host,
        Port = (uint)settings.Port,
        UserID = settings.User,
        Password = settings.Password,
        Database = settings.Name,
        GuidFormat = MySqlGuidFormat.None,
        SslMode = settings.SslEnabled ? MySqlSslMode.Required : MySqlSslMode.None
    };

    return builder.ConnectionString;
}

static DatabaseSettings GetDatabaseSettings()
{
    var host = RequireEnv("DB_HOST");
    var user = RequireEnv("DB_USER");
    var password = RequireEnv("DB_PASSWORD");
    var name = RequireEnv("DB_NAME");
    var portRaw = RequireEnv("DB_PORT");
    var sslRaw = RequireEnv("DB_SSL");

    if (!int.TryParse(portRaw, out var port) || port <= 0 || port > 65535)
    {
        throw new InvalidOperationException("DB_PORT must be a valid port number.");
    }

    if (!TryParseBool(sslRaw, out var sslEnabled))
    {
        throw new InvalidOperationException("DB_SSL must be true or false.");
    }

    return new DatabaseSettings(host, port, user, password, name, sslEnabled);
}

static async Task ValidateDatabaseConnectionAsync(WebApplication app, DatabaseSettings settings)
{
    using var scope = app.Services.CreateScope();
    var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DatabaseStartup");
    var db = scope.ServiceProvider.GetRequiredService<RitualDbContext>();

    try
    {
        await db.Database.OpenConnectionAsync();
        await db.Database.CloseConnectionAsync();
        logger.LogInformation(
            "Database connection established to {DbHost}:{DbPort}/{DbName} (SSL: {DbSsl}).",
            settings.Host,
            settings.Port,
            settings.Name,
            settings.SslEnabled
        );
    }
    catch (Exception ex)
    {
        logger.LogError(
            ex,
            "Database connection failed to {DbHost}:{DbPort}/{DbName}. Check DB_HOST, DB_PORT, DB_USER, DB_PASSWORD, DB_NAME, DB_SSL.",
            settings.Host,
            settings.Port,
            settings.Name
        );
        throw;
    }
}

static string RequireEnv(string key)
{
    var value = Environment.GetEnvironmentVariable(key);
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"Missing required environment variable: {key}");
    }

    return value;
}

static bool TryParseBool(string value, out bool result)
{
    if (bool.TryParse(value, out result))
    {
        return true;
    }

    switch (value.Trim().ToLowerInvariant())
    {
        case "1":
        case "yes":
        case "y":
            result = true;
            return true;
        case "0":
        case "no":
        case "n":
            result = false;
            return true;
        default:
            result = false;
            return false;
    }
}

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
