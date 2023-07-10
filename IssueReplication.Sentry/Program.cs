using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sentry.AzureFunctions.Worker;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults((host, builder) =>
    {
        builder.UseSentry(host, options =>
        {
            options.Dsn = Environment.GetEnvironmentVariable("SentryDsn");
            options.Environment = Environment.GetEnvironmentVariable("Environment");
            options.EnableTracing = true;
        });
    }).ConfigureServices(services =>
    {
        services
            .AddLogging();
    }).Build();

host.Run();