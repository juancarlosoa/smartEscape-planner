using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Remove;

public class RemoveItineraryCommandHandler : IRequestHandler<RemoveItineraryCommand, Result>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItineraryCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveItineraryCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.Slug, cancellationToken);
        if (itinerary is null)
        {
            return Result.Failure("Itinerary.NotFound", $"Itinerary with slug {request.Slug} for user {request.UserSlug} was not found.");
        }

        _repository.Remove(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
