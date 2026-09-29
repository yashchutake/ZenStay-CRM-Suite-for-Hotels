using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface ICabBookingService
    {
        IEnumerable<CabBooking> GetAllCabBookings();
        CabBooking GetCabBookingById(int CabBookingId);
        CabBooking CreateCabBooking(CabBooking cabBooking);
        void UpdateCabBooking(int CabBookingId, CabBooking cabBookingForUpdate);
        IEnumerable<CabBooking> GetCabBookingByCustomerId(int customerId);
    }
}
