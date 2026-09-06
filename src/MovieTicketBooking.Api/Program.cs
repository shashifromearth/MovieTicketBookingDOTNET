using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Infrastructure;
using MovieTicketBooking.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new() { Title = "Movie Ticket Booking API", Version = "v1" });
});

if (builder.Environment.IsEnvironment("Testing"))
    builder.Services.AddInfrastructureForTesting();
else
{
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=movieticketbooking.db";

    builder.Services.AddInfrastructure(options =>
        options.UseSqlite(connectionString));
}

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
else
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Movie Ticket Booking API v1");
    });
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();

public partial class Program;
