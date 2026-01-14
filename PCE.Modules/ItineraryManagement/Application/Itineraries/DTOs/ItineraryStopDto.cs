namespace PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;

public record ItineraryStopDto(
    Guid Id,
    string Notes,
    DateTime ScheduledTime,
    string EscapeRoomSlug = null!
);
