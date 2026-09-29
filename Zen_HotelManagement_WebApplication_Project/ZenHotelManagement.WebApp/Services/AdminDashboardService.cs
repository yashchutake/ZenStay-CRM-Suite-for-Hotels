using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;
using System.Linq;

namespace ZenHotelManagement.WebApp.Services
{
    public class AdminDashboardService : IAdminDashboardService
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://localhost:5001/api";

        public AdminDashboardService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> GetTotalEmployeesCountAsync()
        {
            var employees = await _httpClient.GetFromJsonAsync<IEnumerable<object>>($"{_baseUrl}/employees");
            return employees?.Count() ?? 0;
        }

       
        public async Task<int> GetActiveRoomsCountAsync()
        {
            var rooms = await _httpClient.GetFromJsonAsync<List<Room>>($"{_baseUrl}/rooms");
            return rooms?.Where(r => !r.IsDeleted).Count() ?? 0;

        }

        public async Task<int> GetTotalCabDriversCountAsync()
        {
            var drivers = await _httpClient.GetFromJsonAsync<IEnumerable<object>>($"{_baseUrl}/cabdrivers");
            return drivers?.Count() ?? 0;
        }

        public async Task<Dictionary<string, int>> GetEmployeesByDepartmentAsync()
        {
            var employees = await _httpClient.GetFromJsonAsync<List<EmployeeViewModel>>($"{_baseUrl}/employees");
            return employees
                .GroupBy(e => e.Position)
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );
        }

        public async Task<List<Room>> GetVacantRoomsAsync()
        {
            var rooms = await _httpClient.GetFromJsonAsync<List<Room>>($"{_baseUrl}/rooms");
            return rooms?
                .Where(r => r.RoomStatus) // Only vacant rooms
                .OrderBy(r => r.IsClean) // Dirty rooms first
                .ToList() ?? new List<Room>();
        }

        public async Task<bool> UpdateRoomCleanStatusAsync(string roomNo, bool isClean)
        {
            var content = JsonContent.Create(new { roomStatus = true, isClean });
            var response = await _httpClient.PutAsync($"{_baseUrl}/rooms/{roomNo}", content);
            return response.IsSuccessStatusCode;
        }
    }
}
