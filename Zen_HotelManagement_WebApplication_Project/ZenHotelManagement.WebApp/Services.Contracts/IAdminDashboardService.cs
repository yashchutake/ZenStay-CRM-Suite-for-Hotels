using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface IAdminDashboardService
    {
        Task<int> GetTotalEmployeesCountAsync();
        Task<int> GetActiveRoomsCountAsync();
        Task<int> GetTotalCabDriversCountAsync();
        Task<Dictionary<string, int>> GetEmployeesByDepartmentAsync();
        Task<List<Room>> GetVacantRoomsAsync();
        Task<bool> UpdateRoomCleanStatusAsync(string roomNo, bool isClean);
    }
}
