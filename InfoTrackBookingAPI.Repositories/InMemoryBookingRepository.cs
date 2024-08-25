using InfoTrackBookingAPI.Repositories.Abstract;

namespace InfoTrackBookingAPI.Repositories
{
    public class InMemoryBookingRepository : IBookingRepository
    {
        private readonly Dictionary<string, List<Guid>> _bookings = new();

        public bool IsBookingAvailable(string bookingTime)
        {
            TimeSpan bookingStart = TimeSpan.Parse(bookingTime);
            TimeSpan bookingEnd = bookingStart.Add(TimeSpan.FromHours(1));

            // Count simultaneous bookings that overlap with the requested time slot
            int overlappingBookings = _bookings
                .Where(b =>
                    TimeSpan.Parse(b.Key) < bookingEnd &&
                    TimeSpan.Parse(b.Key).Add(TimeSpan.FromHours(1)) > bookingStart)
                .Sum(b => b.Value.Count);

            return overlappingBookings < 4;
        }

        public void AddBooking(string bookingTime, Guid bookingId)
        {
            if (!_bookings.ContainsKey(bookingTime))
            {
                _bookings[bookingTime] = new List<Guid>();
            }

            _bookings[bookingTime].Add(bookingId);
        }

        public int GetBookingCount(string bookingTime)
        {
            if (!_bookings.TryGetValue(bookingTime, out List<Guid>? value))
            {
                return 0;
            }

            return value.Count;
        }
    }
}
