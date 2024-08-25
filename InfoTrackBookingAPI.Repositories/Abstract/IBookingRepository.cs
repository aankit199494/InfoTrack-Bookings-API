using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrackBookingAPI.Repositories.Abstract
{
    public interface IBookingRepository
    {
        bool IsBookingAvailable(string bookingTime);
        void AddBooking(string bookingTime, Guid bookingId);
        int GetBookingCount(string bookingTime);
    }
}
