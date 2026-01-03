using Microsoft.EntityFrameworkCore;
using PCE.Shared.DependencyInjection;
using PCE.Modules.EscapeManagement;
using PCE.Modules.Infrastructure.Security;
using PCE.Modules.Location;
using PCE.Modules.ItineraryManagement;
using PCE.Modules.EscapeManagement.Infrastructure.Persistence;
using PCE.Modules.ItineraryManagement.Infrastructure.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services
    .AddShared()
    .AddEscapeManagementModule(builder.Configuration)
    .AddLocationModule(builder.Configuration)
    .AddItineraryManagementModule(builder.Configuration);

builder.WebHost.UseUrls("http://0.0.0.0:2000");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<InternalSecurityMiddleware>();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var escapeDbContext = scope.ServiceProvider.GetRequiredService<EscapeManagementDbContext>();
    escapeDbContext.Database.Migrate();

    var itineraryDbContext = scope.ServiceProvider.GetRequiredService<ItineraryManagementDbContext>();
    itineraryDbContext.Database.Migrate();
}

app.Run();