using PCE.Shared.Data;

namespace PCE.Modules.ItineraryManagement.Domain.Entities;

public class ItineraryStop : BaseEntity
{
    public string Notes { get; private set; } = string.Empty;
    public DateTime ScheduledTime { get; private set; }

    public Guid ItineraryId { get; private set; }
    public Itinerary Itinerary { get; private set; } = null!;

    public Guid EscapeRoomId { get; private set; }

    public static ItineraryStop Create(Guid itineraryId, Guid escapeRoomId, string notes, DateTime scheduledTime)
    {
        return new ItineraryStop
        {
            Id = Guid.NewGuid(),
            ItineraryId = itineraryId,
            EscapeRoomId = escapeRoomId,
            Notes = notes,
            ScheduledTime = scheduledTime.ToUniversalTime(),
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string notes, Guid escapeRoomId, DateTime scheduledTime)
    {
        Notes = notes;
        ScheduledTime = scheduledTime.ToUniversalTime();
        EscapeRoomId = escapeRoomId;
        UpdatedAt = DateTime.UtcNow;
    }
}