using Microsoft.EntityFrameworkCore;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Modules.ItineraryManagement.Infrastructure.Persistence;
using PCE.Modules.ItineraryManagement.Infrastructure.Repositories;
using PCE.Shared.Abstractions.Persistence;

namespace PCE.Modules.ItineraryManagement;

public static class ItineraryManagementModule
{
    public static IServiceCollection AddItineraryManagementModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ItineraryManagementDbContext>(options =>
        {
            options.UseNpgsql(configuration.GetConnectionString("ItineraryManagement"), npgsqlOptions =>
            {
                npgsqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        });

        services.AddScoped<IItineraryRepository, ItineraryRepository>();
        services.AddScoped<IUnitOfWork>(sp => sp.GetRequiredService<ItineraryManagementDbContext>());

        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(ItineraryManagementModule).Assembly));

        return services;
    }
}
