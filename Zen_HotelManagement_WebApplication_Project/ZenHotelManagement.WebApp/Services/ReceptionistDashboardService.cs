using System.Net.Http;
using System.Net.Http.Json;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.WebApp.Services
{    public class ReceptionistDashboardService : IReceptionistDashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "http://localhost:5000/api";

        public ReceptionistDashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetAvailableRoomsCountAsync()
        {
            try
            {
                var rooms = await _httpClient.GetFromJsonAsync<IEnumerable<Room>>($"{_baseUrl}/rooms");
                return rooms?.Count(r => r.RoomStatus == true) ?? 0;
            }
            catch
            {
                return 0;
            }
        }        public async Task<int> GetCheckedInCustomersCountAsync()
        {
            try
            {
                var customers = await _httpClient.GetFromJsonAsync<IEnumerable<CustomerDTOWithBookings>>($"{_baseUrl}/customers");
                if (customers == null) return 0;

                var currentDateTime = DateTime.Now; // Use DateTime for precise comparison
                var checkedInCount = 0;

                foreach (var customer in customers)
                {
                    if (customer.RoomBookings != null)
                    {
                        // Check if customer has any booking where:
                        // 1. Either explicitly checked-in OR auto-checked-in (Confirmed past check-in time)
                        // 2. Check-in date/time is in the past
                        // 3. Check-out date is either in the future or null (TBD)
                        var hasCheckedInBooking = customer.RoomBookings.Any(booking =>
                            // Explicitly checked-in statuses
                            ((string.Equals(booking.BookingStatus, "Checked In", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(booking.BookingStatus, "Checked-In", StringComparison.OrdinalIgnoreCase) ||
                              string.Equals(booking.BookingStatus, "Cheked-In", StringComparison.OrdinalIgnoreCase)) ||
                             // Auto-checked-in: Confirmed bookings past their check-in time (matches auto check-in logic)
                             (string.Equals(booking.BookingStatus, "Confirmed", StringComparison.OrdinalIgnoreCase) &&
                              booking.CheckInDate <= currentDateTime)) &&
                            // Check-in time must be in the past
                            booking.CheckInDate <= currentDateTime && 
                            // Check-out must be in the future or null (TBD)
                            (booking.CheckOutDate == null || booking.CheckOutDate.Value > currentDateTime));
                        
                        if (hasCheckedInBooking)
                        {
                            checkedInCount++;
                        }
                    }
                }

                return checkedInCount;
            }
            catch
            {
                return 0;
            }
        }public async Task<int> GetAvailableCabDriversCountAsync()
        {
            try
            {
                var drivers = await _httpClient.GetFromJsonAsync<IEnumerable<CabDriver>>($"{_baseUrl}/cabdrivers");
                return drivers?.Count(d => !d.IsDeleted && d.IsAvailable == true) ?? 0;
            }
            catch
            {
                return 0;
            }
        }        public async Task<Dictionary<string, int>> GetDailyCheckInsAsync()
        {
            try
            {
                var customers = await _httpClient.GetFromJsonAsync<IEnumerable<CustomerDTOWithBookings>>($"{_baseUrl}/customers");
                if (customers == null) return new Dictionary<string, int>();                var dailyCheckIns = new Dictionary<string, int>();
                var currentTime = DateTime.Now;

                // Generate last 5 days
                for (int i = 4; i >= 0; i--)
                {
                    var date = currentTime.Date.AddDays(-i);
                    var dateKey = date.ToString("MMM dd");
                    dailyCheckIns[dateKey] = 0;
                }

                // Count check-ins for each day
                foreach (var customer in customers)
                {
                    if (customer.RoomBookings != null)
                    {
                        foreach (var booking in customer.RoomBookings)
                        {
                            var checkInDate = booking.CheckInDate.Date;
                            
                            // Check if the check-in date is within the last 5 days
                            if (checkInDate >= currentTime.Date.AddDays(-4) && checkInDate <= currentTime.Date)
                            {
                                var dateKey = checkInDate.ToString("MMM dd");
                                if (dailyCheckIns.ContainsKey(dateKey))
                                {
                                    dailyCheckIns[dateKey]++;
                                }
                            }
                        }
                    }
                }

                return dailyCheckIns;
            }
            catch            {
                // Return default data for last 5 days with 0 check-ins
                var dailyCheckIns = new Dictionary<string, int>();
                var currentTime = DateTime.Now;
                
                for (int i = 4; i >= 0; i--)
                {
                    var date = currentTime.Date.AddDays(-i);
                    var dateKey = date.ToString("MMM dd");
                    dailyCheckIns[dateKey] = 0;
                }
                  return dailyCheckIns;
            }
        }        public async Task<List<TodayArrival>> GetTodayArrivalsAsync()
        {
            try
            {
                var customers = await _httpClient.GetFromJsonAsync<IEnumerable<CustomerDTOWithBookings>>($"{_baseUrl}/customers");
                if (customers == null) return new List<TodayArrival>();

                var todayArrivals = new List<TodayArrival>();
                var today = DateTime.Now.Date;

                foreach (var customer in customers)
                {
                    if (customer.RoomBookings != null)
                    {                        var todaysBookings = customer.RoomBookings.Where(booking =>
                            booking.CheckInDate.Date == today).ToList();

                        foreach (var booking in todaysBookings)
                        {
                            todayArrivals.Add(new TodayArrival
                            {
                                CustomerName = customer.Name ?? "Unknown",
                                RoomNo = booking.Room?.RoomNo ?? "N/A",
                                CheckInTime = booking.CheckInDate.ToString("HH:mm:ss"), // Show seconds for precision
                                BookingStatus = booking.BookingStatus ?? "Confirmed",
                                NumberOfGuests = booking.NumberOfGuests
                            });
                        }
                    }
                }

                // Sort by booking status priority: Confirmed first, then Checked-In, then others
                // Then by check-in time as secondary sort
                return todayArrivals
                    .OrderBy(a => a.BookingStatus == "Confirmed" ? 0 : 
                                a.BookingStatus == "Checked-In" ? 1 : 2)
                    .ThenBy(a => a.CheckInTime)
                    .Take(5)
                    .ToList();
            }
            catch
            {
                return new List<TodayArrival>();
            }
        }
    }    public class TodayArrival
    {
        public string CustomerName { get; set; } = string.Empty;
        public string RoomNo { get; set; } = string.Empty;
        public string CheckInTime { get; set; } = string.Empty;
        public string BookingStatus { get; set; } = string.Empty;
        public int NumberOfGuests { get; set; }
    }
}
