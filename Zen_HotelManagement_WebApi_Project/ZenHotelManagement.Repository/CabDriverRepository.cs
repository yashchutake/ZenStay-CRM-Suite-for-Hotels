using System.ComponentModel.Design;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository
{
    public class CabDriverRepository : RepositoryBase<CabDriver>,ICabDriverRepository
    {
        public CabDriverRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public void CreateCabDriver(CabDriver cabDriver) => Create(cabDriver);

        public IEnumerable<CabDriver> GetAllCabDrivers(bool trackChanges) =>
                  FindAll(trackChanges).OrderBy(c => c.CabDriverId).ToList();     

        public CabDriver GetCabDriverById(int cabDriverId, bool trackChanges) =>
                  FindByCondition(c => c.CabDriverId.Equals(cabDriverId), trackChanges).SingleOrDefault();


    }
}
