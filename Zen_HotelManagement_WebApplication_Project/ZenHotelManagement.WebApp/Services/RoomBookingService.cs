using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{
    public class RoomBookingService : IRoomBookingService
    {
        private IHttpClientFactory _httpClientFactory;
        private readonly string _apiBaseUrl = "https://localhost:5001/api/roombookings/";

        public RoomBookingService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public Room CreateBooking(Room booking)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Room> GetActiveBookings()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Room> GetAllBookings()
        {
            throw new NotImplementedException();
        }

        public Room GetBookingById(int bookingId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Room> GetBookingsByCustomerId(int customerId)
        {
            throw new NotImplementedException();
        }

        public void UpdateBooking(int bookingId, Room booking)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cancels a confirmed booking by calling the API
        /// </summary>
        /// <param name="bookingId">The booking ID to cancel</param>
        /// <returns>True if cancellation was successful, false otherwise</returns>
        public async Task<bool> CancelBookingAsync(int bookingId)
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                var response = await client.PostAsync($"{_apiBaseUrl}{bookingId}/cancel", null);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error cancelling booking {bookingId}: {ex.Message}");
                return false;
            }
        }
    }
}
