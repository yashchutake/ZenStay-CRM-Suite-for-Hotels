using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Presentation.Controllers
{
    [Route("api/roombookings")]
    [ApiController]
    public class RoomBookingController : ControllerBase
    {
        private readonly IServiceManager _service;

        public RoomBookingController(IServiceManager service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }        /// <summary>
        /// Gets all room bookings
        /// </summary>
        [HttpGet]
        public IActionResult GetAllBookings()
        {
            try
            {
                var bookings = _service.RoomBookingService.GetAllBookings(trackChanges: false);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Gets a room booking by its ID
        /// </summary>
        [HttpGet("{id:int}", Name = "GetBookingById")]
        public IActionResult GetBookingById(int id)
        {
            try
            {
                var booking = _service.RoomBookingService.GetBookingById(id, trackChanges: false);
                return Ok(booking);
            }
            catch (Exception ex) when (ex.GetType().Name == "RoomBookingNotFoundException")
            {
                return NotFound($"Booking with id: {id} was not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Gets all bookings for a specific customer
        /// </summary>
        [HttpGet("customer/{customerId:int}")]
        public IActionResult GetBookingsByCustomerId(int customerId)
        {
            try
            {
                var bookings = _service.RoomBookingService.GetBookingsByCustomerId(customerId, trackChanges: false);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Gets all active room bookings
        /// </summary>
        [HttpGet("active")]
        public IActionResult GetActiveBookings()
        {
            try
            {
                var bookings = _service.RoomBookingService.GetActiveBookings(trackChanges: false);
                return Ok(bookings);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Creates a new room booking
        /// </summary>
        [HttpPost]
        public IActionResult CreateBooking([FromBody] RoomBookingDtoForCreation booking)
        {
            try
            {
                if (booking == null)
                    return BadRequest("RoomBookingDtoForCreation object is null");

                var createdBooking = _service.RoomBookingService.CreateBooking(booking);
                
                return CreatedAtRoute("GetBookingById", 
                    new { id = createdBooking.BookingId }, 
                    createdBooking);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Updates an existing room booking
        /// </summary>
        [HttpPut("{id:int}")]
        public IActionResult UpdateBooking(int id, [FromBody] RoomBookingDtoForUpdation booking)
        {
            try
            {
                if (booking == null)
                    return BadRequest("RoomBookingDtoForUpdation object is null");

                _service.RoomBookingService.UpdateBooking(id, booking, trackChanges: true);
                return NoContent();
            }
            catch (Exception ex) when (ex.GetType().Name == "RoomBookingNotFoundException")
            {
                return NotFound($"Booking with id: {id} was not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        /// <summary>
        /// Completes expired room bookings (checkout time has passed)
        /// </summary>
        [HttpPost("complete-expired")]
        public IActionResult CompleteExpiredBookings()
        {
            try
            {
                _service.RoomBookingService.CompleteExpiredRoomBookings();
                return Ok(new { Message = "Completed expired room bookings successfully" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates room availability based on current time
        /// </summary>
        [HttpPost("update-room-availability")]
        public IActionResult UpdateRoomAvailability()
        {
            try
            {
                _service.RoomBookingService.UpdateRoomAvailabilityBasedOnCurrentTime();
                return Ok(new { Message = "Room availability updated successfully based on current time" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        /// <summary>
        /// Cancels a confirmed booking
        /// </summary>
        [HttpPost("{id:int}/cancel")]
        public IActionResult CancelBooking(int id)
        {
            try
            {
                _service.RoomBookingService.CancelBooking(id, trackChanges: true);
                return Ok(new { Message = $"Booking {id} has been cancelled successfully" });
            }
            catch (Exception ex) when (ex.GetType().Name == "RoomBookingNotFoundException")
            {
                return NotFound($"Booking with id: {id} was not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
