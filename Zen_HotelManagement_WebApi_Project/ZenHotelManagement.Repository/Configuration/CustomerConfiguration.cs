using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository.Configuration
{
    public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> builder)
        {
            builder.HasData(
                new Customer
                {
                    Id = 101,
                    CustomerId = "123412341234",
                    IdType = "Aadhar Card",
                    Name = "Samiksha Shelke",
                    Gender = "Female",
                    Address = "Karvenagar, Pune",
                    Country = "India",
                    MobileNo = "+919765453674"
                },
                new Customer
                {
                    Id = 102,
                    CustomerId = "432143214321",
                    IdType = "Passport",
                    Name = "Rohan Deshmukh",
                    Gender = "Male",
                    Address = "Kothrud, Pune",
                    Country = "India",
                    MobileNo = "+919876543210"
                },
                new Customer
                {
                    Id = 103,
                    CustomerId = "567856785678",
                    IdType = "Driving License",
                    Name = "Priya Sharma",
                    Gender = "Female",
                    Address = "Baner, Pune",
                    Country = "India",
                    MobileNo = "+919876543211"
                },
                new Customer
                {
                    Id = 104,
                    CustomerId = "876587588765",
                    IdType = "Aadhar Card",
                    Name = "Amit Patil",
                    Gender = "Male",
                    Address = "Aundh, Pune",
                    Country = "India",
                    MobileNo = "+919876543212"
                }
            );
        }
    }
}