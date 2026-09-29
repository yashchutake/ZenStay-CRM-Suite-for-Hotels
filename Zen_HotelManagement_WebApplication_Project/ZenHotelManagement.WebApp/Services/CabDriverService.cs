using System.Net.Http;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{
    public class CabDriverService : ICabDriverService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public CabDriverService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<CabDriver> CreateAsync(CabDriver cabDriver)
        {
            var http = _httpClientFactory.CreateClient();
            cabDriver.IsAvailable = true;
            var response = await http.PostAsJsonAsync("https://localhost:5001/api/cabdrivers", cabDriver);
            if (response.IsSuccessStatusCode)
            {
                var createdCabDriver = await response.Content.ReadFromJsonAsync<CabDriver>();
                return createdCabDriver!;
            }
            throw new Exception("Failed to create cab driver");
        }        public async Task<List<CabDriver>> GetAllAsync()
        {
            var http = _httpClientFactory.CreateClient();
            var response = await http.GetFromJsonAsync<List<CabDriver>>("https://localhost:5001/api/cabdrivers");
            return response ?? new List<CabDriver>();
        }

        public async Task<List<CabDriver>> GetAvailableCabDriversAsync()
        {
            var allDrivers = await GetAllAsync();
            return allDrivers.Where(d => d.IsAvailable && !d.IsDeleted).ToList();
        }

        public async Task<CabDriver?> GetByIdAsync(int cabDriverId)
        {
            var http = _httpClientFactory.CreateClient();
            var response = await http.GetFromJsonAsync<CabDriver>($"https://localhost:5001/api/cabdrivers/{cabDriverId}");
            return response ?? new();
        }

        public async Task UpdateAsync(int cabDriverId, CabDriver cabDriverForUpdate)
        {
            var http = _httpClientFactory.CreateClient();
            var response = await http.PutAsJsonAsync($"https://localhost:5001/api/cabdrivers/{cabDriverId}", cabDriverForUpdate);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update cab driver");
            }
        }

    }
}
