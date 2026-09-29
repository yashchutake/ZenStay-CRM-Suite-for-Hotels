using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EmployeePortalWebApi.Entities.Exceptions;

namespace ZenHotelManagement.Entities.Exceptions
{
    public class RoomBookingNotFoundException : NotFoundException
    {
        public RoomBookingNotFoundException(int bookingId) : base("This room booking with id : " + bookingId + " doesn't exist in the database")
        {
            
        }
    }
}
