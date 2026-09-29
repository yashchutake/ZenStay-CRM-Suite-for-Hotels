
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Contracts
{
    public interface IRoomBookingRepository
    {
        IEnumerable<RoomBooking> GetAllBookings(bool trackChanges);
        RoomBooking GetBookingById(int bookingId, bool trackChanges);
        void CreateBooking(RoomBooking booking);
        IEnumerable<RoomBooking> GetBookingsByCustomerId(int customerId, bool trackChanges);
        IEnumerable<RoomBooking> GetActiveBookings(bool trackChanges);
    }
}
