using BrainBay.Application.DependencyInjection;
using BrainBay.Console.BackgroundWorker;
using BrainBay.Infrastructure.DatabaseContext;
using BrainBay.Infrastructure.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var host = Host.CreateDefaultBuilder(args)
    .ConfigureServices((context, services) =>
    {
        // Add HttpClient
        services.AddHttpClient("RickAndMorty", client =>
        {
            client.BaseAddress = new Uri("https://rickandmortyapi.com/api/character?status=alive");
        });
        var configuration = new ConfigurationBuilder()
          .SetBasePath(Directory.GetCurrentDirectory())   // important for console apps
          .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
          .AddEnvironmentVariables() // optional
          .Build();

        services.AddSingleton<IConfiguration>(configuration);

        // Add DbContext
        services.RegisterInfra(configuration);
        services.RegisterApplication();
        // Add Hosted Service (background worker)
        services.AddHostedService<CharacterSyncService>();
    })
    .ConfigureLogging(logging =>
    {
        logging.ClearProviders();
        logging.AddConsole();
    }).Build();


// Apply migrations at startup
using (var scope = host.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<BrainBayDbContext>();
    db.Database.Migrate(); // Applies any pending migrations
}

await host.RunAsync();