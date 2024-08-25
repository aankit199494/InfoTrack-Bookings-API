using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;
using InfoTrackBookingAPI.Controllers;
using InfoTrackBookingAPI.DTO.Request.Booking;
using InfoTrackBookingAPI.DTO.Response;
using InfoTrackBookingAPI.DTO.Response.Booking;
using InfoTrackBookingAPI.Services.Abstract;

namespace InfoTrackBookingAPI.Tests
{
    public class BookingControllerTests
    {
        private readonly BookingController _controller;
        private readonly IBookingService _bookingService = Substitute.For<IBookingService>();
        private readonly ILogger<BookingController> _logger = Substitute.For<ILogger<BookingController>>();

        public BookingControllerTests()
        {
            _controller = new BookingController(_bookingService, _logger);
        }

        [Fact]
        public void CreateBooking_ReturnsBadRequest_WhenModelStateIsInvalid()
        {
            // Arrange
            _controller.ModelState.AddModelError("BookingTime", "Required");

            var request = new BookingRequest();

            // Act
            var result = _controller.CreateBooking(request);

            // Assert
            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public void CreateBooking_ReturnsBadRequest_WhenBookingTimeIsOutsideBusinessHours()
        {
            // Arrange
            var request = new BookingRequest
            {
                BookingTime = "08:00",
                Name = "Test User"
            };

            _bookingService.IsWithinBusinessHours(request.BookingTime).Returns(false);

            // Act
            var result = _controller.CreateBooking(request);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            var errorResponse = Assert.IsType<GenericApiErrorResponse>(badRequestResult.Value);
            Assert.Equal("Booking time is outside of business hours.", errorResponse.Message);
        }

        [Fact]
        public void CreateBooking_ReturnsConflict_WhenAllBookingsAreFull()
        {
            // Arrange
            var request = new BookingRequest
            {
                BookingTime = "10:00",
                Name = "Test User"
            };

            _bookingService.IsWithinBusinessHours(request.BookingTime).Returns(true);
            _bookingService.TryBook(request).Returns(new BookingResponse { Success = false });

            // Act
            var result = _controller.CreateBooking(request);

            // Assert
            var conflictResult = Assert.IsType<ConflictObjectResult>(result);
            var errorResponse = Assert.IsType<GenericApiErrorResponse>(conflictResult.Value);
            Assert.Equal("All bookings at this time are reserved.", errorResponse.Message);
        }

        [Fact]
        public void CreateBooking_ReturnsOk_WhenBookingIsSuccessful()
        {
            // Arrange
            var request = new BookingRequest
            {
                BookingTime = "10:00",
                Name = "Test User"
            };

            var response = new BookingResponse
            {
                Success = true,
                BookingId = Guid.NewGuid()
            };

            _bookingService.IsWithinBusinessHours(request.BookingTime).Returns(true);
            _bookingService.TryBook(request).Returns(response);

            // Act
            var result = _controller.CreateBooking(request);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var bookingResponse = Assert.IsType<BookingResponse>(okResult.Value);
            Assert.Equal(response.BookingId, bookingResponse.BookingId);
            Assert.True(bookingResponse.Success);
        }

        [Fact]
        public void CreateBooking_ReturnsServerError_WhenExceptionIsThrown()
        {
            // Arrange
            var request = new BookingRequest
            {
                BookingTime = "10:00",
                Name = "Test User"
            };

            _bookingService.IsWithinBusinessHours(request.BookingTime).Returns(true);
            _bookingService.When(x => x.TryBook(request)).Do(x => { throw new Exception("Test exception"); });

            // Act
            var result = _controller.CreateBooking(request);

            // Assert
            var serverErrorResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(500, serverErrorResult.StatusCode);

            var errorResponse = Assert.IsType<GenericApiErrorResponse>(serverErrorResult.Value);
            Assert.Equal("Test exception", errorResponse.Message);
        }
    }

}
