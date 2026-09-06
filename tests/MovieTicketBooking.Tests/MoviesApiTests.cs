using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using MovieTicketBooking.Api.DTOs;

namespace MovieTicketBooking.Tests;

public class MoviesApiTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;

    public MoviesApiTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetMovies_ReturnsSeededMovies()
    {
        var response = await _client.GetAsync("/api/movies");

        response.EnsureSuccessStatusCode();
        var movies = await response.Content.ReadFromJsonAsync<List<MovieDto>>();

        Assert.NotNull(movies);
        Assert.True(movies.Count >= 3);
    }

    [Fact]
    public async Task CreateMovie_ReturnsCreated()
    {
        var request = new CreateMovieRequest("New Movie", "A test movie", 100, "Drama", "PG");

        var response = await _client.PostAsJsonAsync("/api/movies", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var movie = await response.Content.ReadFromJsonAsync<MovieDto>();
        Assert.NotNull(movie);
        Assert.Equal("New Movie", movie.Title);
    }
}
