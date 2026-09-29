using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface IRoomBookingService
    {
        IEnumerable<Room> GetAllBookings();
        Room GetBookingById(int bookingId);
        Room CreateBooking(Room booking);
        IEnumerable<Room> GetBookingsByCustomerId(int customerId);
        IEnumerable<Room> GetActiveBookings();
        void UpdateBooking(int bookingId, Room booking);
        Task<bool> CancelBookingAsync(int bookingId);
    }
}
