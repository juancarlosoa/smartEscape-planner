using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using PCE.Modules.ItineraryManagement.Domain.Entities;
using PCE.Shared.Abstractions.Domain;
using PCE.Shared.Abstractions.Persistence;

namespace PCE.Modules.ItineraryManagement.Infrastructure.Persistence;

public class ItineraryManagementDbContext(DbContextOptions<ItineraryManagementDbContext> options) : DbContext(options), IUnitOfWork
{
    public DbSet<Itinerary> Itineraries { get; set; } = null!;

    public DbSet<ItineraryStop> ItineraryStops { get; set; } = null!;

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
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasConversion(
                        v => v.Value,
                        v => Slug.Create(v));

            entity.Property(e => e.UserSlug)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasConversion(
                        v => v.Value,
                        v => Slug.Create(v));

            entity.HasMany(i => i.ItineraryStops)
                .WithOne(d => d.Itinerary)
                .HasForeignKey(d => d.ItineraryId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ItineraryStop>(entity =>
        {
            entity.ToTable("ItineraryStops");
            entity.HasKey(e => e.Id);

            entity.Property(x => x.Notes)
                .HasMaxLength(500)
                .IsRequired()
                .HasDefaultValue("");

            entity.Property(x => x.ScheduledTime)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}