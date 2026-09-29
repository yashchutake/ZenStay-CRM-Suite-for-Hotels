using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface IEmployeeService
    {
        Task<List<Employee>> GetAllAsync();
        Task<List<Employee>> GetAllAsync(bool includeInactive);
        Task<Employee?> GetByIdAsync(int employeeId);
        Task<Employee> CreateAsync(Employee employee);
        Task UpdateAsync(int employeeId, Employee employee);
        Task<List<string>> GetDistinctPositionsAsync();
    }
}
