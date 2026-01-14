using Microsoft.EntityFrameworkCore;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using PCE.Modules.ItineraryManagement.Domain.Repositories;
using PCE.Modules.ItineraryManagement.Infrastructure.Persistence;

namespace PCE.Modules.ItineraryManagement.Infrastructure.Repositories;

public class ItineraryStopRepository : IItineraryStopRepository
{
    private readonly ItineraryManagementDbContext _context;

    public ItineraryStopRepository(ItineraryManagementDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(ItineraryStop entity, CancellationToken ct = default)
            => await _context.ItineraryStops.AddAsync(entity, ct);

    public void Remove(ItineraryStop entity) => _context.ItineraryStops.Remove(entity);

    public void Update(ItineraryStop entity) => _context.ItineraryStops.Update(entity);

    public async Task<ItineraryStop?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _context.ItineraryStops
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<List<ItineraryStop>> ListAsync(CancellationToken ct = default)
        => await _context.ItineraryStops.ToListAsync(ct);
}