namespace PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;

public record ItineraryDto(
    Guid Id,
    string Slug,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    List<ItineraryStopDto> Stops
);
