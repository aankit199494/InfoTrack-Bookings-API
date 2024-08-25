using InfoTrackBookingAPI.DTO.Request.Booking;
using InfoTrackBookingAPI.DTO.Response;
using InfoTrackBookingAPI.DTO.Response.Booking;
using InfoTrackBookingAPI.Services.Abstract;
using Microsoft.AspNetCore.Mvc;

namespace InfoTrackBookingAPI.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BookingController : ControllerBase
    {
        private readonly IBookingService _bookingService;
        private readonly ILogger<BookingController> _logger;

        public BookingController(IBookingService bookingService, ILogger<BookingController> logger)
        {
            _bookingService = bookingService;
            _logger = logger;
        }

        [HttpPost]
        public IActionResult CreateBooking([FromBody] BookingRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                _logger.LogInformation("Received booking request at {BookingTime} by {Name}", request.BookingTime, request.Name);

                if (!_bookingService.IsWithinBusinessHours(request.BookingTime))
                {
                    _logger.LogWarning("Booking request at {BookingTime} is outside of business hours", request.BookingTime);
                    return BadRequest(new GenericApiErrorResponse
                    {
                        Message = "Booking time is outside of business hours."
                    });
                }

                var bookingResponse = _bookingService.TryBook(request);

                if (!bookingResponse.Success)
                {
                    _logger.LogWarning("All bookings at {BookingTime} are full", request.BookingTime);
                    return Conflict(new GenericApiErrorResponse
                    {
                        Message = "All bookings at this time are reserved."
                    });
                }

                _logger.LogInformation("Booking successful with ID {BookingId}", bookingResponse.BookingId);
                return Ok(new BookingResponse
                {
                    BookingId = bookingResponse.BookingId,
                    Success = true
                });

            } catch (Exception ex)
            {
                return StatusCode(500, new GenericApiErrorResponse
                {
                    Message = ex.Message ?? "An error occurred while processing the booking request."
                });
            }
        }
    }
}
