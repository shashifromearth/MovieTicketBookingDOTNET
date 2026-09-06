namespace MovieTicketBooking.Api.DTOs;

public record ShowTimeDto(
    int Id,
    int MovieId,
    string MovieTitle,
    int ScreenId,
    string ScreenName,
    string TheaterName,
    DateTime StartTime,
    decimal BasePrice);

public record CreateShowTimeRequest(int MovieId, int ScreenId, DateTime StartTime, decimal BasePrice);

public record SeatAvailabilityDto(string Row, int Number, bool IsAvailable);

public record SeatSelectionDto(string Row, int Number);

public record CreateBookingRequest(
    int ShowTimeId,
    string CustomerName,
    string CustomerEmail,
    List<SeatSelectionDto> Seats);

public record BookedSeatDto(string Row, int Number, decimal Price);

public record BookingDto(
    int Id,
    int ShowTimeId,
    string MovieTitle,
    string ScreenName,
    string TheaterName,
    DateTime ShowStartTime,
    string CustomerName,
    string CustomerEmail,
    DateTime BookedAt,
    decimal TotalAmount,
    string Status,
    List<BookedSeatDto> Seats);
