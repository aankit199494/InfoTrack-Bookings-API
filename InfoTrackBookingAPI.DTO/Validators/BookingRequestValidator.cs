namespace InfoTrackBookingAPI.DTO.Validators
{
    using FluentValidation;
    using InfoTrackBookingAPI.DTO.Request.Booking;

    public class BookingRequestValidator : AbstractValidator<BookingRequest>
    {
        public BookingRequestValidator()
        {
            RuleFor(x => x.BookingTime)
                .NotEmpty()
                .Must(BeAValidTime)
                .WithMessage("BookingTime must be a valid 24-hour time in HH:mm format.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name cannot be empty.");
        }

        private bool BeAValidTime(string bookingTime)
        {
            return TimeSpan.TryParse(bookingTime, out _);
        }
    }

}
