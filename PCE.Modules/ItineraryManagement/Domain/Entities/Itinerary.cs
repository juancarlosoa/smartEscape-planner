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

    private readonly List<ItineraryDay> _itineraryDays = [];
    public IReadOnlyCollection<ItineraryDay> ItineraryDays => _itineraryDays.AsReadOnly();

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
            StartDate = startDate,
            EndDate = endDate,
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
        StartDate = startDate;
        EndDate = endDate;
    }
    }

    public ItineraryDay AddDay(DateOnly date)
    {
        var dayNumber = _itineraryDays.Count + 1;
        var day = ItineraryDay.Create(dayNumber, date);
        _itineraryDays.Add(day);
        UpdatedAt = DateTime.UtcNow;
        return day;
    }

    public void UpdateDay(Guid dayId, DateOnly date)
    {
        var day = _itineraryDays.FirstOrDefault(d => d.Id == dayId);
        day?.Update(date);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveDay(Guid dayId)
    {
        var day = _itineraryDays.FirstOrDefault(d => d.Id == dayId);
        if (day != null)
        {
            _itineraryDays.Remove(day);
            // Re-indexing day numbers could be added here if needed
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public ItineraryStop? AddStopToDay(Guid dayId, Guid escapeRoomId, string notes)
    {
        var day = _itineraryDays.FirstOrDefault(d => d.Id == dayId);
        if (day == null) return null;
        var stop = day.AddStop(escapeRoomId, notes);
        UpdatedAt = DateTime.UtcNow;
        return stop;
    }

    public void UpdateStopInDay(Guid dayId, Guid stopId, string notes)
    {
        var day = _itineraryDays.FirstOrDefault(d => d.Id == dayId);
        day?.UpdateStop(stopId, notes);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStopFromDay(Guid dayId, Guid stopId)
    {
        var day = _itineraryDays.FirstOrDefault(d => d.Id == dayId);
        day?.RemoveStop(stopId);
        UpdatedAt = DateTime.UtcNow;
    }
}