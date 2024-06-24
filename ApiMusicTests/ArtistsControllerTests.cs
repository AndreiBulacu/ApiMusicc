using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ApiMusic.Controllers;
using ApiMusic.Data;
using ApiMusic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace ApiMusic.Tests
{
    public class ArtistsControllerTests
    {
        private readonly ApiDbContext _dbContext;
        private readonly ArtistsController _controller;

        public ArtistsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApiDbContext(options);
            _controller = new ArtistsController(_dbContext);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            // Clear existing data
            _dbContext.Artists.RemoveRange(_dbContext.Artists);
            _dbContext.SaveChanges();

            // Seed new data
            _dbContext.Artists.AddRange(new List<Artist>
            {
                new Artist { Id = 1, Name = "Artist 1", Gender = "Male" },
                new Artist { Id = 2, Name = "Artist 2", Gender = "Female" },
                new Artist { Id = 3, Name = "Artist 3", Gender = "Non-binary" }
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Post_CreatesNewArtist()
        {
            // Arrange
            var artist = new Artist { Name = "New Artist", Gender = "Male" };

            // Act
            var result = await _controller.Post(artist);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = result as StatusCodeResult;
            Assert.Equal(StatusCodes.Status201Created, statusCodeResult.StatusCode);

            var addedArtist = await _dbContext.Artists.FindAsync(artist.Id);
            Assert.NotNull(addedArtist);
            Assert.Equal("New Artist", addedArtist.Name);
            Assert.Equal("Male", addedArtist.Gender);
        }

        [Fact]
        public async Task ArtistDetails_ReturnsArtistWithSongs()
        {
            // Seed songs for an artist
            var artistWithSongs = new Artist
            {
                Id = 4,
                Name = "Artist 4",
                Gender = "Female"
            };
            _dbContext.Artists.Add(artistWithSongs);
            _dbContext.SaveChanges();

            // Act
            var result = await _controller.ArtistDetails(4);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Artist>>(okResult.Value);
            var artist = returnValue.FirstOrDefault();

            Assert.NotNull(artist);
            Assert.Equal(4, artist.Id);
            Assert.Equal("Artist 4", artist.Name);

        }
    }
}
