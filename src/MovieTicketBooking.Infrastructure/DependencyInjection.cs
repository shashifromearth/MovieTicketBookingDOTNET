using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Interfaces;
using MovieTicketBooking.Infrastructure.Data;
using MovieTicketBooking.Infrastructure.Repositories;
using MovieTicketBooking.Infrastructure.Services;

namespace MovieTicketBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> configureDb)
    {
        services.AddDbContext<AppDbContext>(configureDb);

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IShowTimeRepository, ShowTimeRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IBookingService, BookingService>();

        return services;
    }

    public static IServiceCollection AddInfrastructureForTesting(this IServiceCollection services) =>
        services.AddInfrastructure(options => options.UseInMemoryDatabase("Testing"));
}
