using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface IReceptionistDashboardService
    {
        Task<int> GetAvailableRoomsCountAsync();
        Task<int> GetCheckedInCustomersCountAsync();
        Task<int> GetAvailableCabDriversCountAsync();
        Task<Dictionary<string, int>> GetDailyCheckInsAsync();
        Task<List<TodayArrival>> GetTodayArrivalsAsync();
    }
}
