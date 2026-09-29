using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public EmployeeService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Employee> CreateAsync(Employee employee)
        {
            var http = _httpClientFactory.CreateClient();
            var response = await http.PostAsJsonAsync("https://localhost:5001/api/employees", employee);
            if (response.IsSuccessStatusCode)
            {
                var createdEmployee = await response.Content.ReadFromJsonAsync<Employee>();
                return createdEmployee!;
            }
            throw new Exception("Failed to create employee");
        }
        public async Task<List<Employee>> GetAllAsync()
        {
            var http = _httpClientFactory.CreateClient();
            var response = await http.GetFromJsonAsync<List<Employee>>("https://localhost:5001/api/employees");
            return response ?? new List<Employee>();
        }

        public async Task<List<Employee>> GetAllAsync(bool includeInactive)
        {
            var http = _httpClientFactory.CreateClient();
            var url = includeInactive
                ? "https://localhost:5001/api/employees?includeInactive=true"
                : "https://localhost:5001/api/employees";
            var response = await http.GetFromJsonAsync<List<Employee>>(url);
            return response ?? new List<Employee>();
        }

        public async Task<Employee?> GetByIdAsync(int employeeId)
        {
            try
            {
                var http = _httpClientFactory.CreateClient();
                var response = await http.GetFromJsonAsync<Employee>($"https://localhost:5001/api/employees/{employeeId}?includeDeleted=true");
                return response ?? new();
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to get employee. Status code: {ex.Message}");
            }
        }

        public async Task UpdateAsync(int employeeId, Employee employeeForUpdate)
        {
            var http = _httpClientFactory.CreateClient();
            var response = await http.PutAsJsonAsync($"https://localhost:5001/api/employees/{employeeId}", employeeForUpdate);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to update employee");
            }
        }

        public async Task<List<string>> GetDistinctPositionsAsync()
        {
            var employees = await GetAllAsync(true);
            return employees
                .Where(e => !string.IsNullOrEmpty(e.Position))
                .Select(e => e.Position!)
                .Distinct()
                .OrderBy(p => p)
                .ToList();
        }
    }
}
