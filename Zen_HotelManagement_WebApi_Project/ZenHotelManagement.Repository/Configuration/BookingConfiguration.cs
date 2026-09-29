using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository.Configuration
{
    public class BookingConfiguration : IEntityTypeConfiguration<RoomBooking>
    {
        public void Configure(EntityTypeBuilder<RoomBooking> builder)
        {
            builder.Property(x => x.TotalAmount)
                   .HasPrecision(18, 2);              builder.HasData(
                // Samiksha Shelke's multiple room bookings (Customer ID: 1)
                new RoomBooking
                {
                    BookingId = 1,
                    CustomerId = 101,
                    RoomId = 1,    // A-101 (Single Bed, AC)
                    CheckInDate = new DateTime(2025, 05, 25, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 05, 28, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 20, 10, 30, 00),
                    TotalAmount = 3500.00M,
                    AmountPaid = 1000.00M, // Advance payment
                    PendingAmount = 2500.00M,
                    BookingStatus = "Confirmed",
                    NumberOfGuests = 2, // Assuming 2 guests for this booking
                },
                new RoomBooking
                {
                    BookingId = 2,
                    CustomerId = 101, // Same customer (Samiksha Shelke)
                    RoomId = 2,    // A-104 (Double Bed, AC)
                    CheckInDate = new DateTime(2025, 05, 25, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 05, 28, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 20, 10, 35, 00),
                    TotalAmount = 4500.00M,
                    AmountPaid = 1500.00M, // Advance payment
                    PendingAmount = 3000.00M,
                    BookingStatus = "Cheked-In",
                    NumberOfGuests = 2,
                },
                new RoomBooking
                {
                    BookingId = 3,
                    CustomerId = 101, // Same customer (Samiksha Shelke)
                    RoomId = 8,    // D-402 (Deluxe, AC)
                    CheckInDate = new DateTime(2025, 06, 15, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 06, 20, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 24, 09, 15, 00),
                    TotalAmount = 7500.00M,
                    AmountPaid = 2000.00M, // Advance payment
                    PendingAmount = 5500.00M,
                    BookingStatus = "Confirmed",
                    NumberOfGuests = 3,
                },
                // Rohan Deshmukh's bookings (Customer ID: 2)
                new RoomBooking
                {
                    BookingId = 4,
                    CustomerId = 102,
                    RoomId = 3,    // B-201 (Semi Deluxe, AC)
                    CheckInDate = new DateTime(2025, 05, 30, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 06, 02, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 23, 16, 45, 00),
                    TotalAmount = 5000.00M,
                    AmountPaid = 5000.00M, // Fully paid
                    PendingAmount = 0.00M,
                    BookingStatus = "Completed",
                    NumberOfGuests = 3
                },
                // Priya Sharma's booking (Customer ID: 3)
                new RoomBooking
                {
                    BookingId = 5,
                    CustomerId = 103,
                    RoomId = 5,    // C-301 (Single Bed, Non-AC)
                    CheckInDate = new DateTime(2025, 06, 01, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 06, 05, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 22, 11, 30, 00),
                    TotalAmount = 2800.00M,
                    AmountPaid = 800.00M, // Advance payment
                    PendingAmount = 2000.00M,
                    BookingStatus = "Confirmed",
                    NumberOfGuests = 2
                },
                // Amit Patil's bookings (Customer ID: 4)
                new RoomBooking
                {
                    BookingId = 6,
                    CustomerId = 104,
                    RoomId = 6,    // C-302 (Double Bed, AC)
                    CheckInDate = new DateTime(2025, 05, 26, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 05, 29, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 24, 10, 15, 00),
                    TotalAmount = 4200.00M,
                    AmountPaid = 1200.00M, // Advance payment
                    PendingAmount = 3000.00M,
                    BookingStatus = "Checked-In",
                    NumberOfGuests = 2
                },
                new RoomBooking
                {
                    BookingId = 7,
                    CustomerId = 104, // Same customer (Amit Patil)
                    RoomId = 7,    // D-401 (Semi Deluxe, AC)
                    CheckInDate = new DateTime(2025, 06, 10, 14, 00, 00),
                    CheckOutDate = new DateTime(2025, 06, 15, 12, 00, 00),
                    BookingDate = new DateTime(2025, 05, 24, 10, 20, 00),
                    TotalAmount = 6000.00M,
                    AmountPaid = 6000.00M, // Fully paid
                    PendingAmount = 0.00M,
                    BookingStatus = "Completed",
                    NumberOfGuests = 4
                }
            );
        }
    }
}