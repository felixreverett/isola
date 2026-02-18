using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Isola {
    internal class Program {
        static void Main(string[] args) {

            // Config for logging system
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            using ILoggerFactory loggerFactory = LoggerFactory.Create(builder => {
                builder.AddConfiguration(config.GetSection("Logging"))
                       .AddConsole();
            });

            ILogger<Game1> logger = loggerFactory.CreateLogger<Game1>();

            Game1 game = new Game1(1920, 1080, "Isola alpha v0.1.3", loggerFactory);
            game.Run();
        }
    }
}