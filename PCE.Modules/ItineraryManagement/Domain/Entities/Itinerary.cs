using PCE.Shared.Abstractions.Domain;
using PCE.Shared.Data;

namespace PCE.Modules.ItineraryManagement.Domain.Entities;

public class Itinerary : BaseEntity
{
    public Slug UserSlug { get; private set; } = null!;
    public Slug Slug { get; private set; } = null!;
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }

    private readonly List<ItineraryStop> _itineraryStops = [];
    public IReadOnlyCollection<ItineraryStop> ItineraryStops => _itineraryStops.AsReadOnly();

    public static Itinerary Create(
        string userSlug,
        string name,
        string description,
        DateTime startDate,
        DateTime endDate
        )
    {
        var id = Guid.NewGuid();
        var slug = Slug.Create(name);
        var uSlug = Slug.Create(userSlug);

        return new Itinerary
        {
            Id = id,
            UserSlug = uSlug,
            Name = name,
            Slug = slug,
            Description = description,
            StartDate = startDate.ToUniversalTime(),
            EndDate = endDate.ToUniversalTime(),
            CreatedAt = DateTime.UtcNow
        };

    }

    public void Update(
        string name,
        string description,
        DateTime startDate,
        DateTime endDate
    )
    {
        if (Name != name)
        {
            Slug = Slug.Create(name);
        }
        Description = description;
        StartDate = startDate.ToUniversalTime();
        EndDate = endDate.ToUniversalTime();
    }
}