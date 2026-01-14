using MediatR;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Shared.Abstractions.Persistence;
using PCE.Shared.Primitives;

namespace PCE.Modules.ItineraryManagement.Application.Stops.Remove;

public class RemoveItineraryStopCommandHandler : IRequestHandler<RemoveItineraryStopCommand, Result>
{
    private readonly IItineraryStopRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RemoveItineraryStopCommandHandler(IItineraryStopRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> Handle(RemoveItineraryStopCommand request, CancellationToken cancellationToken)
    {
        var stop = await _repository.GetByIdAsync(request.StopId, cancellationToken);
        if (stop is null)
        {
            return Result.Failure("Stop.NotFound", $"Stop not found.");
        }

        _repository.Remove(stop);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
