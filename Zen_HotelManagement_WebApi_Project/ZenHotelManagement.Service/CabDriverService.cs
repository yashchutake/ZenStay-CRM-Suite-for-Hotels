using AutoMapper;
using EmployeePortalWebApi.Entities.Exceptions;
using System.ComponentModel;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service
{
    public class CabDriverService : ICabDriverService
    {

        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public CabDriverService(IRepositoryManager repository, IMapper mapper)
        {
            _repositoryManager = repository;
            _mapper = mapper;
        }

        public CabDriverDto CreateCabDriver(CabDriverDtoForOperation cabDriver)
        {
            var cabDriverEntity = _mapper.Map<CabDriver>(cabDriver);
         
            _repositoryManager.CabDriver.CreateCabDriver(cabDriverEntity);
            _repositoryManager.Save();

            var cabDriverToReturn = _mapper.Map<CabDriverDto>(cabDriverEntity);
            return cabDriverToReturn;
        }

        public IEnumerable<CabDriverDto> GetAllCabDrivers(bool trackChanges)
        {
            var cabDrivers = _repositoryManager.CabDriver.GetAllCabDrivers(trackChanges);
            var cabDriverDtos = _mapper.Map<IEnumerable<CabDriverDto>>(cabDrivers);
            return cabDriverDtos;
        }

        public CabDriverDto GetCabDriverById(int cabDriverId, bool trackChanges)
        {
            var cabDriver = _repositoryManager.CabDriver.GetCabDriverById(cabDriverId, trackChanges);
            if (cabDriver == null)
            {
                throw new CabDriverNotFoundExcpetion(cabDriverId);
            }
            var cabDriverToReturn = _mapper.Map<CabDriverDto>(cabDriver);
            return cabDriverToReturn;
        }

        public void UpdateCabDriver(int cabDriverId, CabDriverDtoForOperation cabDriverForUpdate, bool trackChanges)
        {
            var cabDriverEntity = _repositoryManager.CabDriver.GetCabDriverById(cabDriverId, trackChanges);

            if (cabDriverEntity is null)
                throw new CabDriverNotFoundExcpetion(cabDriverId);

            _mapper.Map(cabDriverForUpdate, cabDriverEntity);

            _repositoryManager.Save();
        }
        
        public IEnumerable<CabDriverDto> GetAvailableCabDrivers(bool trackChanges)
        {
            var availableDrivers = _repositoryManager.CabDriver.GetAllCabDrivers(trackChanges)
                .Where(d => d.IsAvailable == true);
            var availableDriverDtos = _mapper.Map<IEnumerable<CabDriverDto>>(availableDrivers);
            return availableDriverDtos;
        }
    }
}
