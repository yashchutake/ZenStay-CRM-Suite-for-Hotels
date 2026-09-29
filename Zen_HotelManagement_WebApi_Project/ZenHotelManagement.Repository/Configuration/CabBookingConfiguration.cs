using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository.Configuration
{
    internal class CabBookingConfiguration : IEntityTypeConfiguration<CabBooking>
    {
        public void Configure(EntityTypeBuilder<CabBooking> builder)
        {
            builder.Property(x => x.Fare)
                   .HasPrecision(18, 2);

            builder.HasData(
                new CabBooking
                {
                    CabBookingId = 1,
                    CustomerId = 101, // Cab for Samiksha
                    CabDriverId = 101,
                    PickUpLocation = "Karvenagar, Pune",
                    DropOffLocation = "Shivajinagar, Pune",
                    Fare = 300,
                    PickUpDateTime = new DateTime(2025, 05, 25, 14, 00, 00),
                    DropOffDateTime = new DateTime(2025, 05, 25, 16, 20, 00),
                    
                },
                new CabBooking
                {
                    CabBookingId = 2,
                    CustomerId = 102, // Cab for Rohan
                    CabDriverId = 102,
                    PickUpLocation = "Kothrud, Pune",
                    DropOffLocation = "Pune Station",
                    Fare = 400,
                    PickUpDateTime = new DateTime(2025, 06, 25, 14, 00, 00),
                    DropOffDateTime = new DateTime(2025, 06, 25, 16, 20, 00),
                   
                },
                new CabBooking
                {
                    CabBookingId = 3,
                    CustomerId = 103,
                    CabDriverId = 103,
                    PickUpLocation = "Baner, Pune",
                    DropOffLocation = "Airport, Pune",
                    Fare = 600,
                    PickUpDateTime = new DateTime(2025, 07, 25, 11, 00, 00),
                    DropOffDateTime = new DateTime(2025, 07, 25, 18, 20, 00),
                   
                },
                new CabBooking
                {
                    CabBookingId = 4,
                    CustomerId = 104,
                    CabDriverId = 104,
                    PickUpLocation = "Aundh, Pune",
                    DropOffLocation = "Deccan, Pune",
                    Fare = 350,
                    PickUpDateTime = new DateTime(2025, 08, 25, 12, 00, 00),
                    DropOffDateTime = new DateTime(2025, 08, 25, 17, 20, 00),
                   
                }
            );
        }
    }
}