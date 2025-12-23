using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Domain;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Update;

public class UpdateItineraryCommandHandler : IRequestHandler<UpdateItineraryCommand, Result<string>>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItineraryCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<string>> Handle(UpdateItineraryCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.Slug, cancellationToken);
        if (itinerary is null)
        {
            return Result<string>.Failure("Itinerary.NotFound", $"Itinerary with slug {request.Slug} for user {request.UserSlug} was not found.");
        }

        if (itinerary.Name != request.Name)
        {
            var newSlug = Slug.Create(request.Name);
            if (newSlug.Value != itinerary.Slug.Value)
            {
                if (await _repository.SlugExistsAsync(request.UserSlug, newSlug.Value, cancellationToken))
                {
                    return Result<string>.Failure("Itinerary.Conflict", "Itinerary with this name already exists for the user.");
                }
            }
        }

        itinerary.Update(
            request.Name,
            request.Description,
            request.StartDate,
            request.EndDate
        );

        _repository.Update(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<string>.Success(itinerary.Slug.Value);
    }
}
