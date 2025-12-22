using PCE.Shared.Data;
namespace PCE.Modules.ItineraryManagement.Domain.Entities;

public class ItineraryDay : BaseEntity
{
    public int DayNumber { get; private set; }
    public DateOnly Date { get; private set; }

    public Guid ItineraryId { get; private set; }
    public Itinerary Itinerary { get; private set; } = null!;

    private readonly List<ItineraryStop> _itineraryStops = [];
    public IReadOnlyCollection<ItineraryStop> ItineraryStops => _itineraryStops.AsReadOnly();

    public static ItineraryDay Create(int dayNumber, DateOnly date)
    {
        return new ItineraryDay
        {
            Id = Guid.NewGuid(),
            DayNumber = dayNumber,
            Date = date,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(DateOnly date)
    {
        Date = date;
        UpdatedAt = DateTime.UtcNow;
    }

    public ItineraryStop AddStop(Guid escapeRoomId, string notes)
    {
        var stop = ItineraryStop.Create(escapeRoomId, notes);
        _itineraryStops.Add(stop);
        UpdatedAt = DateTime.UtcNow;
        return stop;
    }

    public void UpdateStop(Guid stopId, string notes)
    {
        var stop = _itineraryStops.FirstOrDefault(s => s.Id == stopId);
        stop?.Update(notes);
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveStop(Guid stopId)
    {
        var stop = _itineraryStops.FirstOrDefault(s => s.Id == stopId);
        if (stop != null)
        {
            _itineraryStops.Remove(stop);
            UpdatedAt = DateTime.UtcNow;
        }
    }
}