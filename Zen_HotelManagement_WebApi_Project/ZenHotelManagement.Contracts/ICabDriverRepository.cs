using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Contracts
{
    public interface ICabDriverRepository
    {
        IEnumerable<CabDriver> GetAllCabDrivers(bool trackChanges);
        CabDriver GetCabDriverById(int cabDriverId, bool trackChanges);
        void CreateCabDriver(CabDriver cabDriver);
    }
}
