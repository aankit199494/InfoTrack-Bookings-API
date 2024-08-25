using InfoTrackBookingAPI.Repositories;
using InfoTrackBookingAPI.Repositories.Abstract;
using NSubstitute;

namespace InfoTrackBookingAPI.Tests
{
    public class InMemoryBookingRepositoryTests
    {
        private readonly IBookingRepository _repository = Substitute.For<IBookingRepository>();

        public InMemoryBookingRepositoryTests()
        {
            _repository = new InMemoryBookingRepository();
        }

        [Fact]
        public void IsBookingAvailable_ReturnsTrue_WhenNoBookingsExist()
        {
            // Act
            var result = _repository.IsBookingAvailable("10:00");

            // Assert
            Assert.True(result);
        }

        [Fact]
        public void IsBookingAvailable_ReturnsFalse_WhenBookingsAreFull()
        {
            // Arrange
            var bookingTime = "10:00";
            for (int i = 0; i < 4; i++)
            {
                _repository.AddBooking(bookingTime, Guid.NewGuid());
            }

            // Act
            var result = _repository.IsBookingAvailable(bookingTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public void AddBooking_IncreasesBookingCount()
        {
            // Arrange
            var bookingTime = "10:00";
            var initialCount = _repository.GetBookingCount(bookingTime);

            // Act
            _repository.AddBooking(bookingTime, Guid.NewGuid());

            // Assert
            var newCount = _repository.GetBookingCount(bookingTime);
            Assert.Equal(initialCount + 1, newCount);
        }

        [Fact]
        public void GetBookingCount_ReturnsZero_WhenNoBookingsExist()
        {
            // Act
            var count = _repository.GetBookingCount("10:00");

            // Assert
            Assert.Equal(0, count);
        }

        [Fact]
        public void GetBookingCount_ReturnsCorrectCount()
        {
            // Arrange
            var bookingTime = "10:00";
            _repository.AddBooking(bookingTime, Guid.NewGuid());

            // Act
            var count = _repository.GetBookingCount(bookingTime);

            // Assert
            Assert.Equal(1, count);
        }
    }

}
