using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository.Configuration
{
    public class CabDriverConfiguration : IEntityTypeConfiguration<CabDriver>
    {
        public void Configure(EntityTypeBuilder<CabDriver> builder)
        {
            builder.HasData(
                new CabDriver
                {
                    CabDriverId = 101,
                    Name = "Om Patil",
                    Age = 23,
                    Gender = "Male",
                    CarVendor = "City Cabs",
                    CarType = "Sedan",
                },
                new CabDriver
                {
                    CabDriverId = 102,
                    Name = "Rajesh Kumar",
                    Age = 30,
                    Gender = "Male",
                    CarVendor = "Metro Taxi",
                    CarType = "Sedan",
                },
                new CabDriver
                {
                    CabDriverId = 103,
                    Name = "Priya Sharma",
                    Age = 28,
                    Gender = "Female",
                    CarVendor = "Urban Rides",
                    CarType = "SUV",
                },
                new CabDriver
                {
                    CabDriverId = 104,
                    Name = "Amit Singh",
                    Age = 35,
                    Gender = "Male",
                    CarVendor = "Express Cabs",
                    CarType = "Hatchback",
                },
                new CabDriver
                {
                    CabDriverId = 105,
                    Name = "Sneha Patel",
                    Age = 26,
                    Gender = "Female",
                    CarVendor = "Quick Taxi",
                    CarType = "Sedan",

                },
                new CabDriver
                {
                    CabDriverId = 106,
                    Name = "Vikram Rao",
                    Age = 40,
                    Gender = "Male",
                    CarVendor = "Reliable Rides",
                    CarType = "SUV",
                },

                new CabDriver
                {
                    CabDriverId = 107,
                    Name = "Aarti Sharma",
                    Age = 28,
                    Gender = "Female",
                    CarVendor = "Urban Rides",
                    CarType = "Hatchback"
                },
                new CabDriver
                {
                    CabDriverId = 108,
                    Name = "Ravi Kumar",
                    Age = 35,
                    Gender = "Male",
                    CarVendor = "Metro Taxis",
                    CarType = "SUV"
                },
                new CabDriver
                {
                    CabDriverId = 109,
                    Name = "Sneha Desai",
                    Age = 30,
                    Gender = "Female",
                    CarVendor = "Quick Cabs",
                    CarType = "Sedan"
                },

                new CabDriver
                {
                    CabDriverId = 110,
                    Name = "Priya Joshi",
                    Age = 27,
                    Gender = "Female",
                    CarVendor = "Urban Rides",
                    CarType = "Sedan"
                },
                new CabDriver
                {
                    CabDriverId = 111,
                    Name = "Amit Patel",
                    Age = 32,
                    Gender = "Male",
                    CarVendor = "Metro Taxis",
                    CarType = "Hatchback"
                },
                new CabDriver
                {
                    CabDriverId = 112,
                    Name = "Kavita Rao",
                    Age = 29,
                    Gender = "Female",
                    CarVendor = "Quick Cabs",
                    CarType = "SUV"
                },

              new CabDriver
              {
                  CabDriverId = 113,
                  Name = "Rajesh Gupta",
                  Age = 45,
                  Gender = "Male",
                  CarVendor = "City Cabs",
                  CarType = "Sedan"
              },
            new CabDriver
            {
                CabDriverId = 114,
                Name = "Meera Nair",
                Age = 33,
                Gender = "Female",
                CarVendor = "Urban Rides",
                CarType = "SUV"
            },
            new CabDriver
            {
                CabDriverId = 115,
                Name = "Suresh Reddy",
                Age = 38,
                Gender = "Male",
                CarVendor = "Metro Taxis",
                CarType = "Sedan"
            },
            new CabDriver
            {
                CabDriverId = 116,
                Name = "Anita Verma",
                Age = 29,
                Gender = "Female",
                CarVendor = "Quick Cabs",
                CarType = "Hatchback"
            },
            new CabDriver
            {
                CabDriverId = 117,
                Name = "Manoj Kumar",
                Age = 36,
                Gender = "Male",
                CarVendor = "City Cabs",
                CarType = "SUV"
            },
            new CabDriver
            {
                CabDriverId = 118,
                Name = "Pooja Singh",
                Age = 27,
                Gender = "Female",
                CarVendor = "Urban Rides",
                CarType = "Sedan"
            },
            new CabDriver
            {
                CabDriverId = 119,
                Name = "Karan Mehta",
                Age = 31,
                Gender = "Male",
                CarVendor = "Metro Taxis",
                CarType = "Hatchback"
            },
            new CabDriver
            {
                CabDriverId = 120,
                Name = "Divya Sharma",
                Age = 34,
                Gender = "Female",
                CarVendor = "Quick Cabs",
                CarType = "SUV"
            }



            );
        }
    }
}

