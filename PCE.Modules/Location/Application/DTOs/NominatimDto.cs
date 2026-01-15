namespace PCE.Modules.Location.Application.DTOs;

public record NominatimDto(
    string Lat,
    string Lon,
    string DisplayName
);