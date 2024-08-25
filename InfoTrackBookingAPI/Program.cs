using FluentValidation;
using FluentValidation.AspNetCore;
using InfoTrackBookingAPI.DTO.Validators;
using InfoTrackBookingAPI.Repositories;
using InfoTrackBookingAPI.Repositories.Abstract;
using InfoTrackBookingAPI.Services;
using InfoTrackBookingAPI.Services.Abstract;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddSingleton<IBookingRepository, InMemoryBookingRepository>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddValidatorsFromAssemblyContaining<BookingRequestValidator>();
builder.Services.AddFluentValidationAutoValidation();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
