using InfoTrackBookingAPI.DTO.Request.Booking;
using InfoTrackBookingAPI.DTO.Response.Booking;

namespace InfoTrackBookingAPI.Services.Abstract
{
    public interface IBookingService
    {
        public BookingResponse TryBook(BookingRequest bookingRequest);
        public bool IsWithinBusinessHours(string bookingTime);
    }
}
