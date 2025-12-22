using MediatR;
using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Mappers;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.GetBySlug;

public class GetItineraryBySlugQueryHandler : IRequestHandler<GetItineraryBySlugQuery, Result<ItineraryDto>>
{
    private readonly IItineraryRepository _repository;
    private readonly ItineraryMapper _mapper;

    public GetItineraryBySlugQueryHandler(IItineraryRepository repository, ItineraryMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<ItineraryDto>> Handle(GetItineraryBySlugQuery request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.Slug, cancellationToken);

        if (itinerary is null)
        {
            return Result<ItineraryDto>.Failure(Error.NotFound("Itinerary.NotFound", $"Itinerary with slug {request.Slug} for user {request.UserSlug} was not found."));
        }

        return Result<ItineraryDto>.Success(_mapper.MapToDto(itinerary));
    }
}
