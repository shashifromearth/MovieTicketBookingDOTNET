namespace MovieTicketBooking.Api.DTOs;

public record MovieDto(int Id, string Title, string? Description, int DurationMinutes, string Genre, string? Rating);

public record CreateMovieRequest(string Title, string? Description, int DurationMinutes, string Genre, string? Rating);

public record UpdateMovieRequest(string Title, string? Description, int DurationMinutes, string Genre, string? Rating);
