using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Contracts
{
    public interface ICabBookingRepository
    {
        IEnumerable<CabBooking> GetAllCabBookings(bool trackChanges);
        CabBooking GetCabBookingById(int CabBookingId, bool trackChanges);
        void CreateCabBooking(CabBooking cabBooking);
        void UpdateCabBooking(CabBooking cabBooking); 
        IEnumerable<CabBooking> GetCabBookingByCustomerId(int customerId, bool trackChanges);
    }
}
