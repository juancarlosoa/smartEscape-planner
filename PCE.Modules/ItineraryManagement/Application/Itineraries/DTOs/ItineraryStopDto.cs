namespace PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;

public record ItineraryStopDto(
    Guid Id,
    string Notes,
    string EscapeRoomSlug,
    string EscapeRoomName,
    double Latitude, 
    double Longitude,
    string Address
);
