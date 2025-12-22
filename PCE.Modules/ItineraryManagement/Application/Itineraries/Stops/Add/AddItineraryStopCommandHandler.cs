using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Add;

public class AddItineraryStopCommandHandler : IRequestHandler<AddItineraryStopCommand, Result<Guid>>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public AddItineraryStopCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(AddItineraryStopCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.ItinerarySlug, cancellationToken);
        if (itinerary is null)
        {
            return Result<Guid>.Failure(Error.NotFound("Itinerary.NotFound", $"Itinerary with slug {request.ItinerarySlug} for user {request.UserSlug} was not found."));
        }

        var stop = itinerary.AddStopToDay(request.DayId, request.EscapeRoomId, request.Notes);
        if (stop is null)
        {
             return Result<Guid>.Failure(Error.NotFound("ItineraryDay.NotFound", $"ItineraryDay with id {request.DayId} was not found in itinerary {request.ItinerarySlug}."));
        }

        _repository.Update(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(stop.Id);
    }
}
