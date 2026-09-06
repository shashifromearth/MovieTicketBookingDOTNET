namespace MovieTicketBooking.Api.DTOs;

public record TheaterDto(int Id, string Name, string Location);

public record ScreenDto(int Id, int TheaterId, string Name, int TotalRows, int SeatsPerRow);

public record CreateTheaterRequest(string Name, string Location);

public record CreateScreenRequest(int TheaterId, string Name, int TotalRows, int SeatsPerRow);
