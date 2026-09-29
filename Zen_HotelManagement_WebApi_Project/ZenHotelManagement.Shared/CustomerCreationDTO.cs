using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Shared
{    public record CustomerCreationDTO
    (
           string CustomerId,
           string IdType,
           string Name,
           string Gender,
           string? Address,
           string? Country,
           string MobileNo,
           ICollection<RoomBookingDtoForCreation> RoomBookings,
           ICollection<CabBookingDtoForOperation>? CabBookings
    );
    
}
