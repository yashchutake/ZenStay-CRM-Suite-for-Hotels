using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenHotelManagement.Shared
{
   public record CustomerDTO
  (
         int Id,
         string CustomerId,
         string IdType,
         string Name,
         string Gender,
         string? Address,
         string? Country,
         string MobileNo
   );
}
