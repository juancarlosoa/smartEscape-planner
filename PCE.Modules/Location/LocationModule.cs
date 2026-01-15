namespace PCE.Modules.Location;

public static class LocationModule
{
    public static IServiceCollection AddLocationModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssembly(typeof(LocationModule).Assembly));

        services.AddHttpClient("NominatimClient", client =>
            {
                client.BaseAddress = new Uri("https://nominatim.openstreetmap.org/");
                client.DefaultRequestHeaders.Add("User-Agent", "SmartEscapePlanner/1.0 (contact@smartEscapePlanner.com)");
                client.DefaultRequestHeaders.Add("Referer", "https://smartEscapePlanner.com");
                client.DefaultRequestHeaders.Add("Accept-Language", "es-ES,es;q=0.9,en;q=0.8");
                client.Timeout = TimeSpan.FromSeconds(10);
            });

        return services;
    }
}
