using RFSerialBridge;

Host.CreateDefaultBuilder(args)
    .UseWindowsService()
    .ConfigureAppConfiguration(config => { config.AddJsonFile("appsettings.json", optional: true); })
    .ConfigureServices((context, services) =>
    {
        services.Configure<Config>(context.Configuration.GetSection("config"));
        services.AddHttpClient();
        services.AddHostedService<Worker>();
    })
    
    .Build()
    .Run();