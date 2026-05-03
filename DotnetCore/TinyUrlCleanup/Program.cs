using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;

// 1. Initialize the Host (This sets up Configuration and Logging automatically)
var builder = Host.CreateApplicationBuilder(args);

// 2. Add Application Insights for background tasks
builder.Services.AddApplicationInsightsTelemetryWorkerService();

using IHost host = builder.Build();

// 3. Get services we need
var logger = host.Services.GetRequiredService<ILogger<Program>>();
var config = host.Services.GetRequiredService<IConfiguration>();

logger.LogInformation("WebJob Cleanup Task started at: {time}", DateTimeOffset.Now);

// 4. Logic to delete old URLs
// Note: It looks for 'DefaultConnection' in your Azure App Service Connection Strings

var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables() // This line is the magic!
    .Build();

string? connectionString = configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    logger.LogError("Database Connection String 'DefaultConnection' is missing!");
    return;
}

try
{
    using (SqlConnection conn = new SqlConnection(connectionString))
    {
        await conn.OpenAsync();

        // SQL Query: Deletes records older than 1 hour (adjust table name if needed)
        string sql = "DELETE FROM Urls WHERE CreatedDate < DATEADD(hour, -1, GETUTCDATE())";

        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            int rowsDeleted = await cmd.ExecuteNonQueryAsync();
            logger.LogInformation("Cleanup successful. Rows deleted: {count}", rowsDeleted);
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while cleaning the database.");
}

logger.LogInformation("WebJob Cleanup Task finished.");