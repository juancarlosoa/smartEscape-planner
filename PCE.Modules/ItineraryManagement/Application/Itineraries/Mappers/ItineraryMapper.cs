using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using Riok.Mapperly.Abstractions;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Mappers;

[Mapper]
public partial class ItineraryMapper
{
    [MapProperty("Slug.Value", nameof(ItineraryDto.Slug))]
    [MapProperty(nameof(Itinerary.ItineraryStops), nameof(ItineraryDto.Stops))] 
    public partial ItineraryDto MapToDto(Itinerary itinerary);

    [MapProperty("Slug.Value", nameof(ItineraryListDto.Slug))]
    public partial ItineraryListDto MapToListDto(Itinerary itinerary);

    public partial ItineraryStopDto MapToDto(ItineraryStop itineraryStop);

}
