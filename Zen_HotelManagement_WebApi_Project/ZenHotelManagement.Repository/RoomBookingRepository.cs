using Microsoft.EntityFrameworkCore;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository
{
    public class RoomBookingRepository : RepositoryBase<RoomBooking>, IRoomBookingRepository
    {
        public RoomBookingRepository(RepositoryContext repositoryContext)
            : base(repositoryContext)
        {
        }

        public IEnumerable<RoomBooking> GetAllBookings(bool trackChanges) =>
            FindAll(trackChanges)
            .Include(b => b.Room)
            .OrderBy(b => b.CheckInDate)
            .ToList();

        public RoomBooking? GetBookingById(int bookingId, bool trackChanges) =>
            FindByCondition(b => b.BookingId.Equals(bookingId), trackChanges)
            .Include(b => b.Room)
            .SingleOrDefault();

        public void CreateBooking(RoomBooking booking) => Create(booking);

        public IEnumerable<RoomBooking> GetBookingsByCustomerId(int customerId, bool trackChanges) =>
            FindByCondition(b => b.CustomerId.Equals(customerId), trackChanges)
            .Include(b => b.Room)
            .OrderBy(b => b.CheckInDate)
            .ToList();

        public IEnumerable<RoomBooking> GetActiveBookings(bool trackChanges) =>
            FindByCondition(b => b.CheckOutDate >= DateTime.Now, trackChanges)
            .Include(b => b.Room)
            .OrderBy(b => b.CheckInDate)
            .ToList();
    }
}
