using MediatR;
using PCE.Modules.Location.Application.DTOs;
using PCE.Shared.Primitives;

namespace PCE.Modules.Location.Application.SearchLocation;

public class SearchLocationQueryHandler : IRequestHandler<SearchLocationQuery, Result<List<NominatimDto>>>
{
    private readonly HttpClient _httpClient;

    public SearchLocationQueryHandler(IHttpClientFactory httpClient)
    {
        _httpClient = httpClient.CreateClient("NominatimClient");
    }

    public async Task<Result<List<NominatimDto>>> Handle(SearchLocationQuery query, CancellationToken cancellationToken)
    {
        try
        {
            var queryString = $"search?" +
                              $"format=json&limit=5" +
                              $"&street={Uri.EscapeDataString(query.Street)}" +
                              $"&city={Uri.EscapeDataString(query.City)}" +
                              $"&state={Uri.EscapeDataString(query.State)}" +
                              $"&postalcode={Uri.EscapeDataString(query.PostalCode)}" +
                              $"&country={Uri.EscapeDataString(query.Country)}";

            var nominatimResults = await _httpClient.GetFromJsonAsync<List<NominatimDto>>(queryString, cancellationToken);

            if (nominatimResults != null && nominatimResults.Count > 0)
            {
                var results = nominatimResults.Select(r => new NominatimDto(
                    Lat: r.Lat,
                    Lon: r.Lon,
                    DisplayName: r.DisplayName
                )).ToList();

                return Result<List<NominatimDto>>.Success(results);
            }

            return Result<List<NominatimDto>>.Success(new List<NominatimDto>());
        }
        catch (Exception ex)
        {
            return Result<List<NominatimDto>>.Failure($"Error geocoding address: {ex.Message}");
        }
    }
}