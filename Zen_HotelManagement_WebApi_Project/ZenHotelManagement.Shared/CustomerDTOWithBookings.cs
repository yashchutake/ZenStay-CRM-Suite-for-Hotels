using System;
using System.Collections.Generic;

namespace ZenHotelManagement.Shared
{
    public record CustomerDTOWithBookings(
        int Id,
        string CustomerId,
        string IdType,
        string Name,
        string Gender,
        string? Address,
        string? Country,
        string MobileNo,
        ICollection<RoomBookingDto> RoomBookings,
        ICollection<CabBookingDto> CabBookings
    );
}
