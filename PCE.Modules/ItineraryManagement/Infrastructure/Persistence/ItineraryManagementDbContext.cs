using Microsoft.EntityFrameworkCore;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using PCE.Shared.Abstractions.Domain;
using PCE.Shared.Abstractions.Persistence;

namespace PCE.Modules.ItineraryManagement.Infrastructure.Persistence;

public class ItineraryManagementDbContext(DbContextOptions<ItineraryManagementDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Itinerary> Itineraries { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Itinerary>(entity =>
        {
            entity.ToTable("Itineraries");

            entity.HasKey(i => i.Id);

            entity.Property(i => i.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Slug)
                .HasConversion(v => v.Value, v => Slug.Create(v))
                .IsRequired();

            entity.Property(e => e.UserSlug)
                .HasConversion(v => v.Value, v => Slug.Create(v))
                .IsRequired();

            entity.HasMany(i => i.ItineraryDays)
                .WithOne(d => d.Itinerary)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItineraryDay>(entity =>
        {
            entity.ToTable("ItineraryDays");
            entity.HasKey(e => e.Id);

            entity.HasMany(d => d.ItineraryStops)
                .WithOne(s => s.ItineraryDay)
                .HasForeignKey(s => s.ItineraryDayId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItineraryStop>(entity =>
        {
            entity.ToTable("ItineraryStops");
            entity.HasKey(e => e.Id);
        });

        base.OnModelCreating(modelBuilder);
    }
}