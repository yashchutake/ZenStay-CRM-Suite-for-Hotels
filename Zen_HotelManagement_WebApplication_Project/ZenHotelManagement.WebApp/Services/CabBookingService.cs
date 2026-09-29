using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{
    public class CabBookingService : ICabBookingService
    {
        private IHttpClientFactory _httpClientFactory;
        public CabBookingService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public CabBooking CreateCabBooking(CabBooking cabBooking)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CabBooking> GetAllCabBookings()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<CabBooking> GetCabBookingByCustomerId(int customerId)
        {
            throw new NotImplementedException();
        }

        public CabBooking GetCabBookingById(int CabBookingId)
        {
            throw new NotImplementedException();
        }

        public void UpdateCabBooking(int CabBookingId, CabBooking cabBookingForUpdate)
        {
            throw new NotImplementedException();
        }
    }
}
