using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.Service;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Presentation.Controllers
{
    [Route("api/cabdrivers")]
    [ApiController]
    public class CabDriverController :  ControllerBase
    {
        private readonly IServiceManager _service;

        public CabDriverController(IServiceManager service) => _service = service;

        [HttpGet]
        public IActionResult GetCabDrivers()
        {
            var cabDrivers = _service.CabDriverService.GetAllCabDrivers(trackChanges: false);
            return Ok(cabDrivers);
        }

        [HttpGet("{cabDriverId}", Name = "CabDriverById")]
        public IActionResult GetCabDriverById(int cabDriverId)
        {
            var cabDriver = _service.CabDriverService.GetCabDriverById(cabDriverId, trackChanges: false);
            return Ok(cabDriver);
        }

        [HttpPost]
        public IActionResult CreateCabDriver([FromBody] CabDriverDtoForOperation cabDriver)
        {
            if (cabDriver is null)
                return BadRequest("Cab Driver details not found");

            var createdCabDriver = _service.CabDriverService.CreateCabDriver(cabDriver);
            return CreatedAtRoute("CabDriverById", new { cabDriverId = createdCabDriver.CabDriverId }, createdCabDriver);
        }

        [HttpPut("{cabDriverId:int}")]
        public IActionResult UpdateCabDriver(int cabDriverId, [FromBody] CabDriverDtoForOperation cabDriverToUpdate)
        {
            if (cabDriverToUpdate is null)
                return BadRequest("Cab Driver details not found");

             _service.CabDriverService.UpdateCabDriver(cabDriverId, cabDriverToUpdate, trackChanges: true);
            return NoContent();
        }

        [HttpGet("available")]
        public IActionResult GetAvailableCabDrivers()
        {
            var availableDrivers = _service.CabDriverService.GetAvailableCabDrivers(trackChanges: false);
            return Ok(availableDrivers);
        }

    }
}