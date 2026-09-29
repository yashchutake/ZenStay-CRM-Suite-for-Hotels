using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;


namespace ZenHotelManagement.Presentation.Controllers
{
    [Route("api/employees")]
    [ApiController]
    public class EmployeesController : ControllerBase
    {
        private readonly IServiceManager _service;

        public EmployeesController(IServiceManager service) => _service = service; [HttpGet]
        public IActionResult GetAllEmployees([FromQuery] bool includeInactive = false)
        {
            var employees = includeInactive
                ? _service.EmployeeService.GetAllEmployeesIncludingDeleted(trackChanges: true)
                : _service.EmployeeService.GetAllEmployees(trackChanges: false);
            return Ok(employees);
            //var emmployee = _service.EmployeeService.GetAllEmployees(trackChanges: false);
            //return Ok(emmployee);
        }        [HttpGet("{EmployeeId:int}", Name = "EmployeeById")]
        public IActionResult GetEmployeeById(int EmployeeId, [FromQuery] bool includeDeleted = false)
        {
            var employee = includeDeleted 
                ? _service.EmployeeService.GetEmployeeByIdIncludingDeleted(EmployeeId, trackChanges: false)
                : _service.EmployeeService.GetEmployeeById(EmployeeId, trackChanges: false);
            return Ok(employee);
        }

        [HttpGet("adhar/{AdharNo}", Name = "EmployeeByAdharNo")]
        public IActionResult GetEmployeeByAdharNo(string AdharNo)
        {
            var employee = _service.EmployeeService.GetEmployeeByAdharNo(AdharNo, trackChanges: false);
            return Ok(employee);
        }

        [HttpPost]
        public IActionResult CreateEmployee(int EmployeeId, [FromBody] EmployeeDtoForOperation employee)
        {
            if (employee is null)
                return BadRequest("No Employee Details Provided");
            var createdEmployee = _service.EmployeeService.CreateEmployee(employee);
            return CreatedAtRoute("EmployeeById", new { EmployeeId = createdEmployee.EmployeeId }, createdEmployee);
        }

        [HttpPut("{EmployeeId:int}")]
        public IActionResult UpdateEmployee(int EmployeeId, [FromBody] EmployeeDtoForOperation employee)
        {
            if (employee is null)
                return BadRequest("No employee details provided");

            _service.EmployeeService.UpdateEmployee(EmployeeId, employee, trackChanges: true);
            return NoContent();
        }

        [HttpDelete("{EmployeeId:int}")]
        public IActionResult DeleteEmployee(int EmployeeId)
        {
            _service.EmployeeService.DeleteEmployee(EmployeeId, trackChanges: true);
            return NoContent();
        }
    }
}
