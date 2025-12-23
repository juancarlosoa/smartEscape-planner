using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Mappers;

[Mapper]
public partial class ItineraryMapper
{
    [MapProperty("Slug.Value", nameof(ItineraryDto.Slug))]
    [MapProperty(nameof(Itinerary.ItineraryDays), nameof(ItineraryDto.Days))] // This one is fine as it refers to immediate property
    public partial ItineraryDto MapToDto(Itinerary itinerary);

    [MapProperty("Slug.Value", nameof(ItineraryListDto.Slug))]
    public partial ItineraryListDto MapToListDto(Itinerary itinerary);

    [MapProperty(nameof(ItineraryDay.ItineraryStops), nameof(ItineraryDayDto.Stops))]
    public partial ItineraryDayDto MapToDto(ItineraryDay itineraryDay);
    public partial ItineraryStopDto MapToDto(ItineraryStop itineraryStop);

}
