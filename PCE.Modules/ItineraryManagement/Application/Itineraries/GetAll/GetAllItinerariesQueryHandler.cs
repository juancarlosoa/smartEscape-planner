using MediatR;
using PCE.Modules.ItineraryManagement.Application.Itineraries.DTOs;
using PCE.Modules.ItineraryManagement.Application.Itineraries.Mappers;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.GetAll;

public class GetAllItinerariesQueryHandler : IRequestHandler<GetAllItinerariesQuery, Result<List<ItineraryListDto>>>
{
    private readonly IItineraryRepository _repository;
    private readonly ItineraryMapper _mapper;

    public GetAllItinerariesQueryHandler(IItineraryRepository repository, ItineraryMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<Result<List<ItineraryListDto>>> Handle(GetAllItinerariesQuery request, CancellationToken cancellationToken)
    {
        var itineraries = await _repository.GetByUserSlugAsync(request.UserSlug, cancellationToken);

        var dtos = itineraries.Select(i => _mapper.MapToListDto(i)).ToList();

        return Result<List<ItineraryListDto>>.Success(dtos);
    }
}
