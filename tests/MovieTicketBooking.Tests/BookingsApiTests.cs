using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MovieTicketBooking.Api.DTOs;

namespace MovieTicketBooking.Tests;

public class BookingsApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BookingsApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetSeatAvailability_ReturnsSeats()
    {
        var response = await _client.GetAsync("/api/bookings/showtime/1/seats");

        response.EnsureSuccessStatusCode();
        var seats = await response.Content.ReadFromJsonAsync<List<SeatAvailabilityDto>>();

        Assert.NotNull(seats);
        Assert.NotEmpty(seats);
    }

    [Fact]
    public async Task CreateBooking_ReturnsCreated()
    {
        var request = new CreateBookingRequest(
            ShowTimeId: 1,
            CustomerName: "Integration Test User",
            CustomerEmail: "test@example.com",
            Seats: [new SeatSelectionDto("D", 5)]);

        var response = await _client.PostAsJsonAsync("/api/bookings", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var booking = await response.Content.ReadFromJsonAsync<BookingDto>();
        Assert.NotNull(booking);
        Assert.Single(booking.Seats);
        Assert.Equal("Confirmed", booking.Status);
    }

    [Fact]
    public async Task CreateBooking_DuplicateSeat_ReturnsBadRequest()
    {
        var seat = new SeatSelectionDto("E", 3);
        var request = new CreateBookingRequest(1, "User1", "u1@test.com", [seat]);

        await _client.PostAsJsonAsync("/api/bookings", request);
        var response = await _client.PostAsJsonAsync("/api/bookings",
            new CreateBookingRequest(1, "User2", "u2@test.com", [seat]));

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
}
