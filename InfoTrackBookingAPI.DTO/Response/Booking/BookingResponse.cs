using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InfoTrackBookingAPI.DTO.Response.Booking
{
    public class BookingResponse
    {
        public bool Success { get; set; }
        public Guid? BookingId { get; set; }
    }
}
