using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service
{    public interface ICabDriverService
    {
        IEnumerable<CabDriverDto> GetAllCabDrivers(bool trackChanges);

        CabDriverDto GetCabDriverById(int cabDriverId, bool trackChanges);

        CabDriverDto CreateCabDriver(CabDriverDtoForOperation cabDriver);

        void UpdateCabDriver(int cabDriverId, CabDriverDtoForOperation cabDriverForUpdate, bool trackChanges);
        
        IEnumerable<CabDriverDto> GetAvailableCabDrivers(bool trackChanges);

    }
}
