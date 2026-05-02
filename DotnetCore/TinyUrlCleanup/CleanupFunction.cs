using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace TinyUrlCleanup
{
    public class CleanupFunction
    {
        private readonly ILogger _logger;

        public CleanupFunction(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CleanupFunction>();
        }

        // The "TimerTrigger" attribute is what makes this a "Cron Job"
        [Function("DeleteOldUrls")]
        public void Run([TimerTrigger("0 0 * * * *")] TimerInfo myTimer)
        {
            _logger.LogInformation($"C# Timer trigger function executed at: {DateTime.Now}");
        }
    }
}