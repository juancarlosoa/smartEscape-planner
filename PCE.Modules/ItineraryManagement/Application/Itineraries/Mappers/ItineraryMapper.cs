using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Mappers;

[Mapper]
public partial class ItineraryMapper
{
    [MapProperty(nameof(Itinerary.Slug.Value), nameof(ItineraryDto.Slug))]
    [MapProperty(nameof(Itinerary.ItineraryDays), nameof(ItineraryDto.Days))]
    public partial ItineraryDto MapToDto(Itinerary itinerary);

    [MapProperty(nameof(Itinerary.Slug.Value), nameof(ItineraryListDto.Slug))]
    public partial ItineraryListDto MapToListDto(Itinerary itinerary);

    [MapProperty(nameof(ItineraryDay.ItineraryStops), nameof(ItineraryDayDto.Stops))]
    public partial ItineraryDayDto MapToDto(ItineraryDay itineraryDay);

    [MapProperty(nameof(ItineraryStop.EscapeRoom.Slug.Value), nameof(ItineraryStopDto.EscapeRoomSlug))]
    [MapProperty(nameof(ItineraryStop.EscapeRoom.Name), nameof(ItineraryStopDto.EscapeRoomName))]
    [MapProperty(nameof(ItineraryStop.EscapeRoom.Latitude), nameof(ItineraryStopDto.Latitude))]
    [MapProperty(nameof(ItineraryStop.EscapeRoom.Longitude), nameof(ItineraryStopDto.Longitude))]
    [MapProperty(nameof(ItineraryStop.EscapeRoom.Address), nameof(ItineraryStopDto.Address))]
    public partial ItineraryStopDto MapToDto(ItineraryStop itineraryStop);
}
