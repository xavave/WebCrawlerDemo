using Serilog;
using Serilog.Events;

namespace WebCrawlerDemo
{
    /// <summary>
    /// Configuration centralisée pour le logging avec Serilog
    /// Itération 7: Structured Logging
    /// </summary>
    public static class LoggingConfiguration
    {
        /// <summary>
        /// Configure Serilog avec Console et File sinks
        /// </summary>
        public static void ConfigureLogging(LogEventLevel minimumLevel = LogEventLevel.Information)
        {
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Is(minimumLevel)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "WebCrawlerDemo")
                .WriteTo.Console(
                    outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Exception}")
                .WriteTo.File(
                    path: "logs/webcrawler-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}",
                    retainedFileCountLimit: 7)
                .CreateLogger();

            Log.Information("Logging configuré avec succès - Niveau: {MinimumLevel}", minimumLevel);
        }

        /// <summary>
        /// Ferme et flush le logger
        /// </summary>
        public static void CloseAndFlush()
        {
            Log.CloseAndFlush();
        }
    }
}
