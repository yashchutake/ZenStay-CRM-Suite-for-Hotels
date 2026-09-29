using AutoMapper;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Exceptions;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service
{
    public class CabBookingService : ICabBookingService
    {
        private readonly IRepositoryManager _repositoryManager;
        private readonly IMapper _mapper;
        public CabBookingService(IRepositoryManager repository,  IMapper mapper)
        {
            _repositoryManager = repository;
            _mapper = mapper;
        }        public CabBookingDto CreateCabBooking(CabBookingDtoForOperation cabBooking)
        {
            var cabBookingEntity = _mapper.Map<CabBooking>(cabBooking);
            cabBookingEntity.IsRideCompleted = false;
            
            // Driver availability will be managed by background service based on pickup/dropoff times
            // No need to change driver status here - booking is just scheduled for future
            
            _repositoryManager.CabBooking.CreateCabBooking(cabBookingEntity);
            _repositoryManager.Save();

            var cabBookingToReturn = _mapper.Map<CabBookingDto>(cabBookingEntity);
            return cabBookingToReturn;
        }

       
        public IEnumerable<CabBookingDto> GetAllCabBookings(bool trackChanges)
        {
            var cabBookings = _repositoryManager.CabBooking.GetAllCabBookings(trackChanges);
            var cabBookingDtos = _mapper.Map<IEnumerable<CabBookingDto>>(cabBookings);
            return cabBookingDtos;
        }

        public IEnumerable<CabBookingDto> GetCabBookingByCustomerId(int customerId, bool trackChanges)
        {

            var cabBookings = _repositoryManager.CabBooking.GetCabBookingByCustomerId(customerId, trackChanges);
            if (cabBookings == null )
            {
                throw new CabBookingNotFoundException(customerId);
            }

            var cabBookingDtos = _mapper.Map<IEnumerable<CabBookingDto>>(cabBookings);
            return cabBookingDtos;

        }

        public CabBookingDto GetCabBookingById(int CabBookingId, bool trackChanges)
        {
            var cabBooking = _repositoryManager.CabBooking.GetCabBookingById(CabBookingId, trackChanges);
            if (cabBooking == null)
            {
                throw new CabBookingNotFoundException(CabBookingId);
            }
            var cabBookingToReturn = _mapper.Map<CabBookingDto>(cabBooking);
            return cabBookingToReturn;
        }        public void UpdateCabBooking(int CabBookingId, CabBookingDtoForOperation cabBookingForUpdate, bool trackChanges)
        {
            var cabBooking = _repositoryManager.CabBooking.GetCabBookingById(CabBookingId, trackChanges);
            if (cabBooking == null)
            {
                throw new CabBookingNotFoundException(CabBookingId);
            }
            
            _mapper.Map(cabBookingForUpdate, cabBooking);
            _repositoryManager.Save();
            
            // After update, recalculate all driver availabilities based on current time
            UpdateDriverAvailabilityBasedOnCurrentTime();
        }
          public void CompleteExpiredRides()
        {
            var currentTime = DateTime.Now;
              // Get all active (non-completed) cab bookings that have passed their drop-off time
            var expiredBookings = _repositoryManager.CabBooking.GetAllCabBookings(trackChanges: true)
                .Where(booking => booking.IsRideCompleted != true && booking.DropOffDateTime <= currentTime);
            
            foreach (var booking in expiredBookings)
            {
                // Mark ride as completed
                booking.IsRideCompleted = true;
            }
            
            if (expiredBookings.Any())
            {
                _repositoryManager.Save();
            }
        }

        public void UpdateDriverAvailabilityBasedOnCurrentTime()
        {
            var currentTime = DateTime.Now;
              // Get all bookings that should be active right now (pickup time has passed, drop-off time hasn't, and not completed)
            var currentlyActiveBookings = _repositoryManager.CabBooking.GetAllCabBookings(trackChanges: false)
                .Where(booking => booking.IsRideCompleted != true && 
                                booking.PickUpDateTime <= currentTime && 
                                booking.DropOffDateTime > currentTime)
                .Select(b => b.CabDriverId)
                .ToHashSet();
            
            // Get all drivers and update their status based on real-time data
            var allDrivers = _repositoryManager.CabDriver.GetAllCabDrivers(trackChanges: true);
            bool hasChanges = false;
            
            foreach (var driver in allDrivers)
            {
                bool shouldBeBusy = currentlyActiveBookings.Contains(driver.CabDriverId);
                
                // Update driver availability if it doesn't match current reality
                if (driver.IsAvailable == shouldBeBusy) // If available but should be busy OR busy but should be available
                {
                    driver.IsAvailable = !shouldBeBusy;
                    hasChanges = true;
                }
            }
            
            if (hasChanges)
            {
                _repositoryManager.Save();
            }
        }

        /// <summary>
        /// Cancels a specific cab booking by ID
        /// </summary>
        /// <param name="cabBookingId">The ID of the cab booking to cancel</param>
        /// <param name="trackChanges">Whether to track changes</param>
        /// <exception cref="CabBookingNotFoundException">If booking not found</exception>
        public void CancelCabBooking(int cabBookingId, bool trackChanges)
        {
            var cabBooking = _repositoryManager.CabBooking.GetCabBookingById(cabBookingId, trackChanges);
            if (cabBooking == null)
            {
                throw new CabBookingNotFoundException(cabBookingId);
            }

            // Only cancel if the ride is not already completed
            if (cabBooking.IsRideCompleted != true)
            {
                cabBooking.IsRideCompleted = true;
                
                // Update driver availability if necessary
                var driver = _repositoryManager.CabDriver.GetCabDriverById(cabBooking.CabDriverId, true);
                if (driver != null && !driver.IsAvailable)
                {
                    driver.IsAvailable = true;
                }
                
                _repositoryManager.Save();
            }
        }

        /// <summary>
        /// Cancels all cab bookings for a specific customer within a date range
        /// Used when a room booking is cancelled to clean up associated cab bookings
        /// </summary>
        /// <param name="customerId">The customer ID</param>
        /// <param name="fromDate">The start date of the range</param>
        /// <param name="toDate">The end date of the range</param>
        /// <param name="trackChanges">Whether to track changes</param>
        public void CancelCabBookingsByCustomerId(int customerId, DateTime fromDate, DateTime toDate, bool trackChanges)
        {            // Get all incomplete cab bookings for the customer within the date range
            var cabBookings = _repositoryManager.CabBooking.GetCabBookingByCustomerId(customerId, trackChanges)
                .Where(booking => booking.IsRideCompleted != true && 
                       ((booking.PickUpDateTime >= fromDate && booking.PickUpDateTime <= toDate) ||
                        (booking.DropOffDateTime >= fromDate && booking.DropOffDateTime <= toDate)));
            
            foreach (var booking in cabBookings)
            {
                booking.IsRideCompleted = true;
                
                // Update driver availability
                var driver = _repositoryManager.CabDriver.GetCabDriverById(booking.CabDriverId, true);
                if (driver != null && !driver.IsAvailable)
                {
                    driver.IsAvailable = true;
                }
            }
            
            if (cabBookings.Any())
            {
                _repositoryManager.Save();
            }
        }
    }
}
