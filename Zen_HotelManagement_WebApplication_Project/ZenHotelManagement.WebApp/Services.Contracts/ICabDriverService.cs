using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{    public interface ICabDriverService
    {
        Task<List<CabDriver>> GetAllAsync();

        Task<List<CabDriver>> GetAvailableCabDriversAsync();

        Task<CabDriver?> GetByIdAsync(int cabDriverId);

        Task<CabDriver> CreateAsync(CabDriver cabDriver);

        Task UpdateAsync(int cabDriverId, CabDriver cabDriver);

    }
}
