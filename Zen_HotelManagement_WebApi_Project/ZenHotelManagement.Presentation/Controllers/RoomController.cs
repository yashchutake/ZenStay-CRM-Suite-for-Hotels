using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Presentation.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomsController : ControllerBase
    {
        private readonly IServiceManager _service;

        public RoomsController(IServiceManager serviceManager)
        {
            _service = serviceManager;
        }

        // View all rooms
        [HttpGet]
        public IActionResult GetAllRooms()
        {
            var rooms = _service.RoomService.GetAllRooms(trackChanges: false);
            return Ok(rooms);
        }

        // View rooms by type (e.g., Deluxe, Single)
        [HttpGet("type/{type}")]
        public IActionResult GetRoomsByType(string type)
        {
            var rooms = _service.RoomService.GetRoomsByType(type, trackChanges: false);
            return Ok(rooms);
        }

        // Get room by ID

        [HttpGet("number/{roomNo}", Name = "GetRoomById")]

        public IActionResult GetRoomByNo(string roomNo)
        {
            var room = _service.RoomService.GetRoomByNo(roomNo, trackChanges: false);
            return Ok(room);
        }

        // Get room by ID
        [HttpGet("{roomId:int}")]
        public IActionResult GetRoomById(int roomId)
        {
            try
            {
                var room = _service.RoomService.GetRoomById(roomId, trackChanges: false);
                return Ok(room);
            }
            catch (Exception ex) when (ex.GetType().Name == "RoomNotFoundException")
            {
                return NotFound($"Room with id: {roomId} was not found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }        // Add new room
        [HttpPost]
        public IActionResult CreateRoom([FromBody] RoomForCreateDto roomDto)

        {

            if (roomDto == null)
                return BadRequest("Room data is null");

            try
            {
                var createdRoom = _service.RoomService.CreateRoom(roomDto);
                return CreatedAtRoute("GetRoomById", new { roomNo = createdRoom.RoomNo }, createdRoom);
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("already exists"))
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


        // Update room (status, cleanliness)
        [HttpPut("{roomNo}")]
        public IActionResult UpdateRoom(string roomNo, [FromBody] RoomForUpdateDto roomDto)
        {
            if (roomDto == null)
                return BadRequest("Room data is null");

            _service.RoomService.UpdateRoom(roomNo, roomDto, trackChanges: true);

            return NoContent();
        }

        // DELETE: api/rooms/{roomNo}
        [HttpDelete("{roomNo}")]
        public IActionResult DeleteRoom(string roomNo)
        {
            _service.RoomService.DeleteRoom(roomNo);
            return NoContent();
        }


    }
}
