using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository
{
    public class CabBookingRepository : RepositoryBase<CabBooking>, ICabBookingRepository
    {
        public CabBookingRepository(RepositoryContext repository) : base(repository)
        {
        }

        public void CreateCabBooking(CabBooking cabBooking) => Create(cabBooking);
        public void UpdateCabBooking(CabBooking cabBooking) => Update(cabBooking);
        
        public IEnumerable<CabBooking> GetAllCabBookings(bool trackChanges) =>
             FindAll(trackChanges).OrderBy(b => b.PickUpDateTime).ToList();

        public IEnumerable<CabBooking> GetCabBookingByCustomerId(int customerId, bool trackChanges) =>
             FindByCondition(b => b.CustomerId.Equals(customerId), trackChanges).OrderBy(b => b.PickUpDateTime).ToList();

        public CabBooking GetCabBookingById(int CabBookingId, bool trackChanges) =>           
            FindByCondition(b => b.CabBookingId.Equals(CabBookingId), trackChanges).SingleOrDefault();

     
      
    }

}
