using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace AIWorkstation
{
    public partial class App : Application
    {
        public static ServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configuration setup
            var configuration = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("config/appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            // Service registration
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IConfiguration>(configuration);
            
            // Future: Add services here when we create them
            // serviceCollection.AddSingleton<IAIService, AIService>();
            // serviceCollection.AddSingleton<IPowerShellService, PowerShellService>();

            ServiceProvider = serviceCollection.BuildServiceProvider();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            ServiceProvider?.Dispose();
            base.OnExit(e);
        }
    }
}
