using EmployeePortalWebApi.Entities.Exceptions;

namespace ZenHotelManagement.Entities.Exceptions
{
   public class CabBookingNotFoundException : NotFoundException
    {
        public CabBookingNotFoundException(int cabBookingId)
           : base($"This CabBooking with id : {cabBookingId} doesn't exist in the database")
        {

        }
    }
}
