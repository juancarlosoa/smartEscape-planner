using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Itineraries.Stops.Update;

public class UpdateItineraryStopCommandHandler : IRequestHandler<UpdateItineraryStopCommand, Result>
{
    private readonly IItineraryStopRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateItineraryStopCommandHandler(IItineraryStopRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(UpdateItineraryStopCommand request, CancellationToken cancellationToken)
    {
        var stop = await _repository.GetByIdAsync(request.StopId, cancellationToken);
        if (stop is null)
        {
            return Result.Failure("Stop.NotFound", "Stop Not Found.");
        }

        stop.Update(
            request.Notes,
            request.EscapeRoomId,
            request.ScheduledTime
        );

        _repository.Update(stop);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
