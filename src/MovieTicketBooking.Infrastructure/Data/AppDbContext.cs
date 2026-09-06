using Microsoft.EntityFrameworkCore;
using MovieTicketBooking.Domain.Entities;

namespace MovieTicketBooking.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Movie> Movies => Set<Movie>();
    public DbSet<Theater> Theaters => Set<Theater>();
    public DbSet<Screen> Screens => Set<Screen>();
    public DbSet<ShowTime> ShowTimes => Set<ShowTime>();
    public DbSet<Booking> Bookings => Set<Booking>();
    public DbSet<BookedSeat> BookedSeats => Set<BookedSeat>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.Property(m => m.Title).HasMaxLength(200).IsRequired();
            entity.Property(m => m.Genre).HasMaxLength(100).IsRequired();
            entity.Property(m => m.Rating).HasMaxLength(10);
        });

        modelBuilder.Entity<Theater>(entity =>
        {
            entity.Property(t => t.Name).HasMaxLength(200).IsRequired();
            entity.Property(t => t.Location).HasMaxLength(300).IsRequired();
        });

        modelBuilder.Entity<Screen>(entity =>
        {
            entity.Property(s => s.Name).HasMaxLength(100).IsRequired();
            entity.HasOne(s => s.Theater)
                .WithMany(t => t.Screens)
                .HasForeignKey(s => s.TheaterId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<ShowTime>(entity =>
        {
            entity.Property(st => st.BasePrice).HasPrecision(10, 2);
            entity.HasOne(st => st.Movie)
                .WithMany(m => m.ShowTimes)
                .HasForeignKey(st => st.MovieId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasOne(st => st.Screen)
                .WithMany(s => s.ShowTimes)
                .HasForeignKey(st => st.ScreenId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.Property(b => b.CustomerName).HasMaxLength(200).IsRequired();
            entity.Property(b => b.CustomerEmail).HasMaxLength(200).IsRequired();
            entity.Property(b => b.TotalAmount).HasPrecision(10, 2);
            entity.HasOne(b => b.ShowTime)
                .WithMany(st => st.Bookings)
                .HasForeignKey(b => b.ShowTimeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<BookedSeat>(entity =>
        {
            entity.Property(bs => bs.Row).HasMaxLength(5).IsRequired();
            entity.Property(bs => bs.Price).HasPrecision(10, 2);
            entity.HasOne(bs => bs.Booking)
                .WithMany(b => b.Seats)
                .HasForeignKey(bs => bs.BookingId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(bs => new { bs.BookingId, bs.Row, bs.Number }).IsUnique();
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>().HasData(
            new Movie { Id = 1, Title = "Inception", Description = "A thief who steals corporate secrets through dream-sharing technology.", DurationMinutes = 148, Genre = "Sci-Fi", Rating = "PG-13" },
            new Movie { Id = 2, Title = "The Dark Knight", Description = "Batman faces the Joker in Gotham City.", DurationMinutes = 152, Genre = "Action", Rating = "PG-13" },
            new Movie { Id = 3, Title = "Interstellar", Description = "Explorers travel through a wormhole in space.", DurationMinutes = 169, Genre = "Sci-Fi", Rating = "PG-13" }
        );

        modelBuilder.Entity<Theater>().HasData(
            new Theater { Id = 1, Name = "CineMax Downtown", Location = "123 Main Street" },
            new Theater { Id = 2, Name = "Star Cinema Mall", Location = "456 Mall Road" }
        );

        modelBuilder.Entity<Screen>().HasData(
            new Screen { Id = 1, TheaterId = 1, Name = "Screen 1", TotalRows = 5, SeatsPerRow = 8 },
            new Screen { Id = 2, TheaterId = 1, Name = "Screen 2", TotalRows = 4, SeatsPerRow = 6 },
            new Screen { Id = 3, TheaterId = 2, Name = "Screen 1", TotalRows = 6, SeatsPerRow = 10 }
        );

        var tomorrow = DateTime.UtcNow.Date.AddDays(1).AddHours(18);
        modelBuilder.Entity<ShowTime>().HasData(
            new ShowTime { Id = 1, MovieId = 1, ScreenId = 1, StartTime = tomorrow, BasePrice = 12.50m },
            new ShowTime { Id = 2, MovieId = 2, ScreenId = 1, StartTime = tomorrow.AddHours(3), BasePrice = 14.00m },
            new ShowTime { Id = 3, MovieId = 3, ScreenId = 3, StartTime = tomorrow.AddHours(1), BasePrice = 15.00m }
        );
    }
}
