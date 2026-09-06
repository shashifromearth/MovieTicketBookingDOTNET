namespace MovieTicketBooking.Domain.Entities;

public class Movie
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public int DurationMinutes { get; set; }
    public required string Genre { get; set; }
    public string? Rating { get; set; }

    public ICollection<ShowTime> ShowTimes { get; set; } = [];
}
