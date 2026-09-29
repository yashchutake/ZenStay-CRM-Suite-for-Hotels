using ZenHotelManagement.Shared;
namespace ZenHotelManagement.Service.Contracts
{    public interface ICabBookingService
    {
        IEnumerable<CabBookingDto> GetAllCabBookings(bool trackChanges);
        CabBookingDto GetCabBookingById(int CabBookingId, bool trackChanges);
        CabBookingDto CreateCabBooking(CabBookingDtoForOperation cabBooking);
        void UpdateCabBooking(int CabBookingId, CabBookingDtoForOperation cabBookingForUpdate , bool trackChanges);
        IEnumerable<CabBookingDto> GetCabBookingByCustomerId(int customerId, bool trackChanges);
        void CompleteExpiredRides();
        void UpdateDriverAvailabilityBasedOnCurrentTime();
        void CancelCabBooking(int cabBookingId, bool trackChanges);
        void CancelCabBookingsByCustomerId(int customerId, DateTime fromDate, DateTime toDate, bool trackChanges);
    }
}
