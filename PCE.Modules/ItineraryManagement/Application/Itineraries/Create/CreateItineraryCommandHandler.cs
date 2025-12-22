using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Create;

public class CreateItineraryCommandHandler : IRequestHandler<CreateItineraryCommand, Result<string>>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateItineraryCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(CreateItineraryCommand request, CancellationToken cancellationToken)
    {
        var itinerary = Itinerary.Create(
            request.UserSlug,
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate
        );

        if (await _repository.SlugExistsAsync(itinerary.UserSlug.Value, itinerary.Slug.Value, cancellationToken))
        {
             return Result<string>.Failure(Error.Conflict("Itinerary.Conflict", "Itinerary with this name already exists for the user."));
        }

        await _repository.AddAsync(itinerary, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(itinerary.Slug.Value);
    }
}
