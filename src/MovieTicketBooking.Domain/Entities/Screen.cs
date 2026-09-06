namespace MovieTicketBooking.Domain.Entities;

public class Screen
{
    public int Id { get; set; }
    public int TheaterId { get; set; }
    public required string Name { get; set; }
    public int TotalRows { get; set; }
    public int SeatsPerRow { get; set; }

    public Theater Theater { get; set; } = null!;
    public ICollection<ShowTime> ShowTimes { get; set; } = [];
}
