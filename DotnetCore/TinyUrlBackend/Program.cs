using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using TinyUrlBackend.Data;
using TinyUrlBackend.Models;
using TinyUrlBackend.Services;

var builder = WebApplication.CreateBuilder(args);
var logPath = builder.Configuration.GetValue<string>("Serilog:LogPath") ?? "Logs/default_log.txt";

// Configure Serilog to write to a file
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File(logPath, rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// 1. SERVICES CONFIGURATION
// Pulls connection string or defaults to tinyurl.db
// builder.Services.AddDbContext<AppDbContext>(opt =>  opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=tinyurl.db"));
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddApplicationInsightsTelemetry(options =>
{
    options.ConnectionString = builder.Configuration["APPLICATIONINSIGHTS_CONNECTION_STRING"];
});


// Configure Swagger to match the demo title "Tiny URL API"
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Tiny URL API", Version = "v1" });
});


builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});


builder.Services.AddHealthChecks();

var app = builder.Build();

// --- 2. GLOBAL ERROR HANDLING (Add this here!) ---
app.Use(async (context, next) =>
{
    try
    {
        await next();
    }
    catch (Exception ex)
    {
        // Logs the error to your console window so you can see what broke
        Console.WriteLine($"[CRITICAL ERROR] {DateTime.Now}: {ex.Message}");

        context.Response.StatusCode = 500;
        await context.Response.WriteAsJsonAsync(new { message = "Server Error", details = ex.Message });
    }
});

// This enables Swagger in both Development AND Production (Azure)
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tiny URL API v1");
    c.DocumentTitle = "Tiny URL API";
    c.RoutePrefix = "swagger"; // This ensures it's at /swagger
});

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapHealthChecks("/api/health");
app.MapControllers();

// --- 4. API ROUTES WITH LOGGING ---

// POST: /api/add
app.MapPost("/api/add", async (TinyUrlAddDto dto, AppDbContext db, IConfiguration config, ILogger<Program> logger) =>
{
    logger.LogInformation("Processing request to shorten: {Url}", dto.originalURL);

    if (string.IsNullOrEmpty(dto.originalURL))
    {
        logger.LogWarning("Validation failed: URL is empty.");
        return Results.BadRequest("URL cannot be empty.");
    }

    var code = ShortenerService.GenerateCode();
    var baseDomain = config.GetValue<string>("AppSettings:BaseDomain") ?? "https://localhost:7206";

    var newUrl = new TinyUrl
    {
        code = code,
        originalURL = dto.originalURL,
        shortURL = $"{baseDomain}/{code}",
        isPrivate = dto.isPrivate,
        totalClicks = 0
    };

    db.Urls.Add(newUrl);
    await db.SaveChangesAsync();

    logger.LogInformation("Successfully created code {Code} for URL {Url} ShortURL: {ShortUrl}", code, dto.originalURL, newUrl.shortURL);
    return Results.Ok(newUrl);
}).WithTags("tiny-url");

// GET: /api/public
app.MapGet("/api/public", async (AppDbContext db, ILogger<Program> logger) =>
{
    logger.LogInformation("Fetching all public URLs.");
    return await db.Urls.Where(u => !u.isPrivate).ToListAsync();
}).WithTags("tiny-url");

// GET: /{code} (Redirect Logic)
app.MapGet("/{code}", async (string code, AppDbContext db, ILogger<Program> logger) =>
{
    logger.LogInformation("Redirect request for code: {Code}", code);

    var mapping = await db.Urls.FirstOrDefaultAsync(u => u.code == code);
    if (mapping == null)
    {
        logger.LogWarning("Code {Code} not found in database.", code);
        return Results.NotFound();
    }

    mapping.totalClicks++;
    await db.SaveChangesAsync();

    return Results.Redirect(mapping.originalURL!);
}).WithTags("tiny-url");

// DELETE: /api/delete/{code}
app.MapDelete("/api/delete/{code}", async (string code, AppDbContext db, ILogger<Program> logger) =>
{
    var mapping = await db.Urls.FirstOrDefaultAsync(u => u.code == code);
    if (mapping == null) return Results.NotFound();

    db.Urls.Remove(mapping);
    await db.SaveChangesAsync();
    logger.LogInformation("Deleted code: {Code}", code);
    return Results.NoContent();
}).WithTags("tiny-url");

// DELETE: /api/delete-all
app.MapDelete("/api/delete-all", async (AppDbContext db) =>
{
    var all = await db.Urls.ToListAsync();
    db.Urls.RemoveRange(all);
    await db.SaveChangesAsync();
    return Results.NoContent();
}).WithTags("tiny-url");

// PUT: /api/update/{code}
app.MapPut("/api/update/{code}", async (string code, TinyUrlAddDto dto, AppDbContext db) =>
{
    var existing = await db.Urls.FirstOrDefaultAsync(u => u.code == code);
    if (existing == null) return Results.NotFound();

    existing.originalURL = dto.originalURL;
    existing.isPrivate = dto.isPrivate;
    await db.SaveChangesAsync();
    return Results.Ok(existing);
}).WithTags("tiny-url");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.EnsureCreated();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database.");
    }
}

app.Run();