using NSubstitute;
using InfoTrackBookingAPI.DTO.Request.Booking;
using InfoTrackBookingAPI.Repositories.Abstract;
using InfoTrackBookingAPI.Services;

namespace InfoTrackBookingAPI.Tests
{
    public class BookingServiceTests
    {
        private readonly BookingService _service;
        private readonly IBookingRepository _repository = Substitute.For<IBookingRepository>();

        public BookingServiceTests()
        {
            _service = new BookingService(_repository);
        }

        [Fact]
        public void TryBook_ReturnsUnsuccessful_WhenBookingTimeIsOutsideBusinessHours()
        {
            // Arrange
            var request = new BookingRequest { BookingTime = "08:00" };

            // Act
            var response = _service.TryBook(request);

            // Assert
            Assert.False(response.Success);
            Assert.Null(response.BookingId);
        }

        [Fact]
        public void TryBook_ReturnsUnsuccessful_WhenBookingIsNotAvailable()
        {
            // Arrange
            var request = new BookingRequest { BookingTime = "10:00" };

            _repository.IsBookingAvailable(request.BookingTime).Returns(false);

            // Act
            var response = _service.TryBook(request);

            // Assert
            Assert.False(response.Success);
            Assert.Null(response.BookingId);
        }

        [Fact]
        public void TryBook_ReturnsSuccessful_WhenBookingIsAvailable()
        {
            // Arrange
            var request = new BookingRequest { BookingTime = "10:00" };

            _repository.IsBookingAvailable(request.BookingTime).Returns(true);

            // Act
            var response = _service.TryBook(request);

            // Assert
            Assert.True(response.Success);
            Assert.NotNull(response.BookingId);
            _repository.Received(1).AddBooking(request.BookingTime, Arg.Any<Guid>());
        }

        [Theory]
        [InlineData("08:59", false)]
        [InlineData("09:00", true)]
        [InlineData("12:00", true)]
        [InlineData("16:00", true)]
        [InlineData("16:01", false)]
        public void IsWithinBusinessHours_ReturnsCorrectResult(string bookingTime, bool expectedResult)
        {
            // Act
            var result = _service.IsWithinBusinessHours(bookingTime);

            // Assert
            Assert.Equal(expectedResult, result);
        }
    }

}
