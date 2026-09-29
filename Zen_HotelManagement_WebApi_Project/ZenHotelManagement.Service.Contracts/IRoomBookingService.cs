using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service.Contracts
{    public interface IRoomBookingService
    {
        IEnumerable<RoomBookingDto> GetAllBookings(bool trackChanges);
        RoomBookingDto GetBookingById(int bookingId, bool trackChanges);
        RoomBookingDto CreateBooking(RoomBookingDtoForCreation booking);
        IEnumerable<RoomBookingDto> GetBookingsByCustomerId(int customerId, bool trackChanges);
        IEnumerable<RoomBookingDto> GetActiveBookings(bool trackChanges);
        void UpdateBooking(int bookingId, RoomBookingDtoForUpdation bookingDto, bool trackChanges);
        void UpdateRoomAvailabilityBasedOnCurrentTime();
        void CompleteExpiredRoomBookings();
        void CancelBooking(int bookingId, bool trackChanges);
    }
}
