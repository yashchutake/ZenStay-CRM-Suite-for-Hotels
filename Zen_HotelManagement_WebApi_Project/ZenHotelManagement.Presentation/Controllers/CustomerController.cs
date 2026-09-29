using Microsoft.AspNetCore.Mvc;
using System.Runtime.InteropServices;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;
namespace ZenHotelManagement.Presentation.Controllers
{

    [ApiController]
    [Route("api/customers")]
    public class CustomerController : ControllerBase
    {
        private readonly IServiceManager _customerService;

        public CustomerController(IServiceManager customerService)
        {
            _customerService = customerService;
        }

        [HttpGet]
        public IActionResult GetAllCustomers([FromQuery] bool trackChanges = false)
        {
            var customers =_customerService.CustomerService.ViewAllCustomers(trackChanges);
            return Ok(customers);
        }

        [HttpGet("{customerId}")]
        public IActionResult GetCustomerById(string customerId, [FromQuery] bool trackChanges = false)
        {
            var customer = _customerService.CustomerService.ViewCustomerById(customerId, trackChanges);
            if (customer == null)
                return NotFound();
            return Ok(customer);
        }        [HttpPost]
        public IActionResult AddCustomer([FromBody] CustomerCreationDTO customer)
        {
            var createdCustomer = _customerService.CustomerService.AddCustomer(customer);
            return CreatedAtAction(nameof(GetCustomerById), new { customerId = createdCustomer.CustomerId }, createdCustomer);
        }

        [HttpPut("{customerId}")]
        public IActionResult UpdateCustomer(string customerId, [FromBody] CustomerUpdationDTO customerForUpdate, [FromQuery] bool trackChanges = false)
        {
            _customerService.CustomerService.UpdateCustomer(customerId, customerForUpdate, trackChanges);
            return NoContent();
        }

        [HttpGet("by-room-type/{roomType}")]
        public IActionResult GetCustomersByRoomType(string roomType, [FromQuery] bool trackChanges = false)
        {
            var customers = _customerService.CustomerService.ViewCustomersByRoomType(roomType, trackChanges);
            return Ok(customers);
        }



    }
}
