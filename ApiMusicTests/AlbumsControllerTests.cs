using ApiMusic.Controllers;
using ApiMusic.Data;
using ApiMusic.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ApiMusic.Tests
{
    public class AlbumsControllerTests
    {
        private readonly ApiDbContext _dbContext;
        private readonly AlbumsController _controller;

        public AlbumsControllerTests()
        {
            var options = new DbContextOptionsBuilder<ApiDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            _dbContext = new ApiDbContext(options);
            _controller = new AlbumsController(_dbContext);
        }

        private void SeedDatabase()
        {
            // Clear existing records
            _dbContext.Albums.RemoveRange(_dbContext.Albums);
            _dbContext.Songs.RemoveRange(_dbContext.Songs);
            _dbContext.SaveChanges();

            // Add initial data
            _dbContext.Albums.AddRange(
                new Album { Id = 1, Name = "Album 1", ArtistId = 1 },
                new Album { Id = 2, Name = "Album 2", ArtistId = 2 }
            );
            _dbContext.SaveChanges();
        }

        [Fact]
        public async Task Post_ValidAlbum_ReturnsStatus201Created()
        {
            // Arrange
            SeedDatabase();
            var album = new Album { Id = 3, Name = "Test Album", ArtistId = 1 };

            // Act
            var result = await _controller.Post(album) as StatusCodeResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(StatusCodes.Status201Created, result.StatusCode);
            var albumInDb = await _dbContext.Albums.FindAsync(album.Id);
            Assert.NotNull(albumInDb);
            Assert.Equal(album.Name, albumInDb.Name);
        }

        [Fact]
        public async Task GetAlbums_ReturnsOkResult_WithListOfAlbums()
        {
            // Arrange
            SeedDatabase();

            // Act
            var result = await _controller.GetAlbums() as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<object>>(result.Value);
            Assert.Equal(2, returnValue.Count());
        }

        [Fact]
        public async Task AlbumDetails_ValidAlbumId_ReturnsOkResult_WithAlbumDetails()
        {
            // Arrange
            SeedDatabase();
            var albumId = 1;
            var album = await _dbContext.Albums.FindAsync(albumId);
           

            // Act
            var result = await _controller.AlbumDetails(albumId) as OkObjectResult;

            // Assert
            Assert.NotNull(result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<Album>>(result.Value);
            Assert.Single(returnValue);
            Assert.Equal(albumId, returnValue.First().Id);
        }
    }
}
