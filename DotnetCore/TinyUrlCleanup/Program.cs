using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Data.SqlClient;
using Serilog; // Added for Blob Logging

// 1. Initialize Configuration (Pulls from App Settings and Connection Strings)
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: true)
    .AddEnvironmentVariables()
    .Build();

// 2. Configure Serilog for "Logging to File" (Requirement)
// Uses the AzureWebJobsStorage secret from your App Settings
string? storageConn = configuration["AzureWebJobsStorage"];

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.AzureBlobStorage(
        storageConn,                             // 1st: Connection String
        Serilog.Events.LogEventLevel.Information, // 2nd: Minimum Level
        "webjob-logs",                           // 3rd: Container Name
        "cleanup-history.txt"                    // 4th: File Name
    )
    .CreateLogger();

// 3. Initialize the Host
var builder = Host.CreateApplicationBuilder(args);
builder.Services.AddApplicationInsightsTelemetryWorkerService();

// Redirect standard ILogger to use Serilog
builder.Logging.ClearProviders();
builder.Logging.AddSerilog();

using IHost host = builder.Build();
var logger = host.Services.GetRequiredService<ILogger<Program>>();

logger.LogInformation("WebJob Cleanup Task started at: {time}", DateTimeOffset.Now);

// 4. Get SQL Connection String
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

        // SQL Query: Deletes records older than 1 hour from 'Urls' table
        string sql = "DELETE FROM Urls WHERE CreatedDate < DATEADD(hour, -1, GETUTCDATE())";

        using (SqlCommand cmd = new SqlCommand(sql, conn))
        {
            int rowsDeleted = await cmd.ExecuteNonQueryAsync();
            logger.LogInformation("Cleanup successful. Rows deleted from 'Urls': {count}", rowsDeleted);
        }
    }
}
catch (Exception ex)
{
    logger.LogError(ex, "An error occurred while cleaning the 'Urls' database.");
}
finally
{
    logger.LogInformation("WebJob Cleanup Task finished.");
    // This is vital to ensure the last log line is actually written to the Azure Blob file
    await Log.CloseAndFlushAsync();
}