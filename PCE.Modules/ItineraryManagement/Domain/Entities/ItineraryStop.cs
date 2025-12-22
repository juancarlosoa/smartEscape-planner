using PCE.Modules.EscapeManagement.Domain.EscapeRooms.Entities;
using PCE.Shared.Data;

namespace PCE.Modules.ItineraryManagement.Domain.Entities;

public class ItineraryStop : BaseEntity
{
    public string Notes { get; private set; } = string.Empty;

    public Guid ItineraryDayId { get; private set; }
    public ItineraryDay ItineraryDay { get; private set; } = null!;

    public Guid EscapeRoomId { get; private set; }
    public virtual EscapeRoom EscapeRoom { get; private set; } = null!;

    public static ItineraryStop Create(Guid escapeRoomId, string notes)
    {
        return new ItineraryStop
        {
            Id = Guid.NewGuid(),
            EscapeRoomId = escapeRoomId,
            Notes = notes,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Update(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }
}