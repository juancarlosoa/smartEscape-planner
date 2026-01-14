namespace PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;

public record ItineraryDto(
    string Slug,
    string Name,
    string Description,
    DateTime StartDate,
    DateTime EndDate,
    List<ItineraryStopDto> Stops
);
