using InfoTrackBookingAPI.DTO.Request.Booking;
using InfoTrackBookingAPI.DTO.Response.Booking;
using InfoTrackBookingAPI.Repositories.Abstract;
using InfoTrackBookingAPI.Services.Abstract;

namespace InfoTrackBookingAPI.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingService(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        public BookingResponse TryBook(BookingRequest bookingRequest)
        {
            try
            {
                if (!IsWithinBusinessHours(bookingRequest.BookingTime) ||
                    !_bookingRepository.IsBookingAvailable(bookingRequest.BookingTime))
                {
                    return new BookingResponse
                    {
                        BookingId = null,
                        Success = false,
                    };
                }

                var bookingId = Guid.NewGuid();
                _bookingRepository.AddBooking(bookingRequest.BookingTime, bookingId);
                return new BookingResponse
                {
                    BookingId = bookingId,
                    Success = true,
                };
            }
            catch (Exception)
            {
                throw;
            }
        }

        public bool IsWithinBusinessHours(string bookingTime)
        {
            TimeSpan time = TimeSpan.Parse(bookingTime);

            TimeSpan startTime = new(9, 0, 0);
            TimeSpan endTime = new(16, 0, 0);

            return time >= startTime && time <= endTime;
        }
    }
}
