using System;
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
    public class SongsControllerTests
    {
        private readonly ApiDbContext _dbContext;
        private readonly SongsController _controller;

        public SongsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApiDbContext(options);
            _controller = new SongsController(_dbContext);

            SeedDatabase();
        }

        private void SeedDatabase()
        {
            _dbContext.Songs.RemoveRange(_dbContext.Songs);
            _dbContext.SaveChanges();

            _dbContext.Songs.AddRange(new List<Song>
            {
                new Song { Id = 1, Title = "Song 1", Duration = "300", Language = "English", IsFeatured = true, UploadedDate = DateTime.Now },
                new Song { Id = 2, Title = "Song 2", Duration = "250", Language = "English", IsFeatured = false, UploadedDate = DateTime.Now.AddDays(-1) },
                new Song { Id = 3, Title = "Song 3", Duration = "200", Language = "English", IsFeatured = true, UploadedDate = DateTime.Now.AddDays(-2) }
            });
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Post_CreatesNewSong()
        {
            // Arrange
            var song = new Song { Title = "New Song", Duration = "300", Language = "English" };

            // Act
            var result = await _controller.Post(song);

            // Assert
            Assert.IsType<StatusCodeResult>(result);
            var statusCodeResult = result as StatusCodeResult;
            Assert.Equal(StatusCodes.Status201Created, statusCodeResult.StatusCode);

            var addedSong = await _dbContext.Songs.FindAsync(song.Id);
            Assert.NotNull(addedSong);
        }

        [Fact]
        public async Task GetAllSongs_ReturnsAllSongs()
        {
            // Act
            var result = await _controller.GetAllSongs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(3, returnValue.Count());
        }

        [Fact]
        public async Task FeaturedSongs_ReturnsFeaturedSongs()
        {
            // Act
            var result = await _controller.FeaturedSongs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(2, returnValue.Count()); // Only 2 featured songs
        }

        [Fact]
        public async Task NewSongs_ReturnsNewSongs()
        {
            // Act
            var result = await _controller.NewSongs();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Equal(3, returnValue.Count()); // All 3 songs in descending order of UploadedDate
        }

        [Fact]
        public async Task SearchSongs_ReturnsMatchingSongs()
        {
            // Act
            var result = await _controller.SearchSongs("Song 1");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<object>>(okResult.Value);
            Assert.Single(returnValue); // Only one song matches "Song 1"
        }
    }
}
