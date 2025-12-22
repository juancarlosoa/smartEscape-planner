using PCE.Modules.ItineraryManagement.Domain.Entities;
using PCE.Shared.Abstractions.Persistence;

namespace PCE.Modules.ItineraryManagement.Domain.Repositories;

public interface IItineraryRepository : IRepository<Itinerary>
{
    Task<Itinerary?> GetBySlugAsync(string userSlug, string slug, CancellationToken ct = default);
    Task<List<Itinerary>> GetByUserSlugAsync(string userSlug, CancellationToken ct = default);
    Task<bool> SlugExistsAsync(string userSlug, string slug, CancellationToken ct = default);
}