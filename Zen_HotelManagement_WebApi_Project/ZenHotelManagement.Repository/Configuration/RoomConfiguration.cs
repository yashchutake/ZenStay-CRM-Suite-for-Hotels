using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository.Configuration
{
    public class RoomConfiguration : IEntityTypeConfiguration<Room>
    {
        public void Configure(EntityTypeBuilder<Room> builder)
        {
            builder.HasData(

                //RoomNo,RoomType,IsAc,RoomStatus,IsClean

                new Room
                {
                    RoomId = 1,
                    RoomNo = "A-101",
                    RoomType = "Single Bed",
                    IsAc = true,
                    RoomStatus = true,
                    CostPerNight = 1000.00m,
                    IsClean = true,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 2,
                    RoomNo = "A-102",
                    RoomType = "Double Bed",
                    IsAc = true,
                    CostPerNight = 1500.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 3,
                    RoomNo = "A-103",
                    RoomType = "Semi Deluxe",
                    IsAc = true,
                    CostPerNight = 2000.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 4,
                    RoomNo = "A-104",
                    RoomType = "Deluxe",
                    IsAc = true,
                    CostPerNight = 2500.00m,
                    RoomStatus = false,
                    IsClean = false,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 5,
                    RoomNo = "B-201",
                    RoomType = "Single Bed",
                    IsAc = false,
                    CostPerNight = 800.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 6,
                    RoomNo = "B-202",
                    RoomType = "Double Bed",
                    IsAc = true,
                    CostPerNight = 1200.00m,
                    RoomStatus = true,
                    IsClean = false,
                    RoomCapacity = 3,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 7,
                    RoomNo = "B-203",
                    RoomType = "Semi Deluxe",
                    IsAc = true,
                    CostPerNight = 1800.00m,
                    RoomStatus = false,
                    IsClean = true,
                    RoomCapacity = 4,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 8,
                    RoomNo = "B-204",
                    RoomType = "Deluxe",
                    IsAc = true,
                    CostPerNight = 3000.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 3,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 9,
                    RoomNo = "C-301",
                    RoomType = "Single Bed",
                    IsAc = false,
                    CostPerNight = 900.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 1,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 10,
                    RoomNo = "C-302",
                    RoomType = "Double Bed",
                    IsAc = true,
                    CostPerNight = 1600.00m,
                    RoomStatus = false,
                    IsClean = false,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 11,
                    RoomNo = "C-303",
                    RoomType = "Semi Deluxe",
                    IsAc = true,
                    CostPerNight = 2200.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 3,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 12,
                    RoomNo = "C-304",
                    RoomType = "Deluxe",
                    IsAc = true,
                    CostPerNight = 2800.00m,
                    RoomStatus = true,
                    IsClean = false,
                    RoomCapacity = 4,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 13,
                    RoomNo = "D-401",
                    RoomType = "Single Bed",
                    IsAc = false,
                    CostPerNight = 750.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 1,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 14,
                    RoomNo = "D-402",
                    RoomType = "Double Bed",
                    IsAc = true,
                    CostPerNight = 1400.00m,
                    RoomStatus = false,
                    IsClean = true,
                    RoomCapacity = 2,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 15,
                    RoomNo = "D-403",
                    RoomType = "Semi Deluxe",
                    IsAc = true,
                    CostPerNight = 2100.00m,
                    RoomStatus = true,
                    IsClean = false,
                    RoomCapacity = 3,
                    IsDeleted = false
                },
                new Room
                {
                    RoomId = 16,
                    RoomNo = "D-404",
                    RoomType = "Deluxe",
                    IsAc = true,
                    CostPerNight = 3200.00m,
                    RoomStatus = true,
                    IsClean = true,
                    RoomCapacity = 4,
                    IsDeleted = false
                }

                );
        }
    }
}

