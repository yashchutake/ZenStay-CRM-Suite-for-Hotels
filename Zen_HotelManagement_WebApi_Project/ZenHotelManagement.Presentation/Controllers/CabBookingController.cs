using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Presentation.Controllers
{

    [Route("api/cabbookings")]
    [ApiController]
    public class CabBookingController : ControllerBase
    {

        private readonly IServiceManager _service;

        public CabBookingController(IServiceManager service) => _service = service;

        [HttpGet]
        public IActionResult GetCabBookings()
        {
            var cabBookings = _service.CabBookingService.GetAllCabBookings(trackChanges: false);
            return Ok(cabBookings);
        }

        [HttpGet("{cabBookingId:int}", Name = "CabBookingById")]
        public IActionResult GetCabBookingById(int cabBookingId)
        {
            var cabBooking = _service.CabBookingService.GetCabBookingById(cabBookingId, trackChanges: false);
            return Ok(cabBooking);
        }

        [HttpGet("customer/{customerId:int}")]
        public IActionResult GetCabBookingByCustomerId(int customerId)
        {
            var cabBookings = _service.CabBookingService.GetCabBookingByCustomerId(customerId, trackChanges: false);
            return Ok(cabBookings);
        }

        [HttpPost]
        public IActionResult CreateCabBooking([FromBody] CabBookingDtoForOperation cabBooking)
        {
            if (cabBooking is null)
                return BadRequest("Cab Booking details not found");

            var createdCabBooking = _service.CabBookingService.CreateCabBooking(cabBooking);
            return CreatedAtRoute("CabBookingById", new { cabBookingId = createdCabBooking.CabBookingId }, createdCabBooking);
        }

        [HttpPut("{cabBookingId:int}")]
        public IActionResult UpdateCabBooking(int cabBookingId, [FromBody] CabBookingDtoForOperation cabBookingToUpdate)
        {
            if (cabBookingToUpdate is null)
                return BadRequest("Cab Booking details not found");

           _service.CabBookingService.UpdateCabBooking(cabBookingId, cabBookingToUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpPost("complete-expired")]
        public IActionResult CompleteExpiredRides()
        {
            _service.CabBookingService.CompleteExpiredRides();
            return Ok(new { Message = "Expired rides completed successfully" });
        }

        [HttpPost("update-driver-availability")]
        public IActionResult UpdateDriverAvailability()
        {
            _service.CabBookingService.UpdateDriverAvailabilityBasedOnCurrentTime();
            return Ok(new { Message = "Driver availability updated based on current time" });
        }

        /// <summary>
        /// Cancels a cab booking
        /// </summary>
        [HttpPost("{cabBookingId:int}/cancel")]
        public IActionResult CancelCabBooking(int cabBookingId)
        {
            try
            {
                _service.CabBookingService.CancelCabBooking(cabBookingId, trackChanges: true);
                return Ok(new { Message = $"Cab booking {cabBookingId} has been cancelled successfully" });
            }
            catch (Exception ex) when (ex.GetType().Name == "CabBookingNotFoundException")
            {
                return NotFound($"Cab booking with id: {cabBookingId} was not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}

