using MovieTicketBooking.Domain.Entities;
using MovieTicketBooking.Domain.Enums;
using MovieTicketBooking.Api.DTOs;

namespace MovieTicketBooking.Api.Mappings;

public static class EntityMappings
{
    public static MovieDto ToDto(this Movie movie) =>
        new(movie.Id, movie.Title, movie.Description, movie.DurationMinutes, movie.Genre, movie.Rating);

    public static TheaterDto ToDto(this Theater theater) =>
        new(theater.Id, theater.Name, theater.Location);

    public static ScreenDto ToDto(this Screen screen) =>
        new(screen.Id, screen.TheaterId, screen.Name, screen.TotalRows, screen.SeatsPerRow);

    public static ShowTimeDto ToDto(this ShowTime showTime) =>
        new(
            showTime.Id,
            showTime.MovieId,
            showTime.Movie?.Title ?? string.Empty,
            showTime.ScreenId,
            showTime.Screen?.Name ?? string.Empty,
            showTime.Screen?.Theater?.Name ?? string.Empty,
            showTime.StartTime,
            showTime.BasePrice);

    public static BookingDto ToDto(this Booking booking) =>
        new(
            booking.Id,
            booking.ShowTimeId,
            booking.ShowTime?.Movie?.Title ?? string.Empty,
            booking.ShowTime?.Screen?.Name ?? string.Empty,
            booking.ShowTime?.Screen?.Theater?.Name ?? string.Empty,
            booking.ShowTime?.StartTime ?? default,
            booking.CustomerName,
            booking.CustomerEmail,
            booking.BookedAt,
            booking.TotalAmount,
            booking.Status.ToString(),
            booking.Seats.Select(s => new BookedSeatDto(s.Row, s.Number, s.Price)).ToList());
}
