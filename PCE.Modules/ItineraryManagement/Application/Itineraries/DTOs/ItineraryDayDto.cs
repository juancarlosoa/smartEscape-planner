namespace PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;

public record ItineraryDayDto(
    Guid Id,
    int DayNumber,
    DateOnly Date,
    List<ItineraryStopDto> Stops
);
