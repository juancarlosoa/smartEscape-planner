using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Remove;

public class RemoveItineraryStopCommandHandler : IRequestHandler<RemoveItineraryStopCommand, Result>
{
    private readonly IItineraryRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItineraryStopCommandHandler(IItineraryRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveItineraryStopCommand request, CancellationToken cancellationToken)
    {
        var itinerary = await _repository.GetBySlugAsync(request.UserSlug, request.ItinerarySlug, cancellationToken);
        if (itinerary is null)
        {
            return Result.Failure(Error.NotFound("Itinerary.NotFound", $"Itinerary with slug {request.ItinerarySlug} for user {request.UserSlug} was not found."));
        }

        var day = itinerary.ItineraryDays.FirstOrDefault(d => d.Id == request.DayId);
        if (day is null)
        {
            return Result.Failure(Error.NotFound("ItineraryDay.NotFound", $"ItineraryDay with id {request.DayId} was not found in itinerary {request.ItinerarySlug}."));
        }

        if (!day.ItineraryStops.Any(s => s.Id == request.StopId))
        {
            return Result.Failure(Error.NotFound("ItineraryStop.NotFound", $"ItineraryStop with id {request.StopId} was not found in day {request.DayId}."));
        }

        itinerary.RemoveStopFromDay(request.DayId, request.StopId);

        _repository.Update(itinerary);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
