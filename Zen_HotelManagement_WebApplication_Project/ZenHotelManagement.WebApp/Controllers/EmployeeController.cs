using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Models.ViewModel;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class EmployeeController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public EmployeeController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public async Task<IActionResult> Index(EmployeeFilterModel? filter)
        {
            filter ??= new EmployeeFilterModel();

            // Get all employees initially
            var employees = await _serviceManager.EmployeeService.GetAllAsync(true);
            ViewBag.Positions = await _serviceManager.EmployeeService.GetDistinctPositionsAsync();
            ViewBag.TotalEmployeeCount = employees.Count();
            ViewBag.ActiveEmployeeCount = employees.Count(e => !e.IsDeleted);
            ViewBag.TotalSalary = employees.Where(d => !d.IsDeleted).Sum(d => d.Salary);




            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                employees = employees.Where(e =>
                    (e.Name?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.Email?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (e.AdharNo?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Position))
            {
                employees = employees.Where(e =>
                    e.Position?.Equals(filter.Position, StringComparison.OrdinalIgnoreCase) ?? false
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Gender))
            {
                employees = employees.Where(e =>
                    e.Gender?.Equals(filter.Gender, StringComparison.OrdinalIgnoreCase) ?? false
                ).ToList();
            }

            // Apply Status filter only if it's not "All"
            if (!string.IsNullOrWhiteSpace(filter.Status) && filter.Status.ToLower() != "all")
            {
                bool isActive = filter.Status.Equals("Active", StringComparison.OrdinalIgnoreCase);
                employees = employees.Where(e => e.IsDeleted != isActive).ToList();
            }

            // Recalculate page number if needed
            int totalPages = (int)Math.Ceiling(employees.Count() / (double)filter.PageSize);
            if (filter.PageNumber > totalPages && totalPages > 0)
            {
                filter.PageNumber = totalPages;
            }

            // Create the paged list with filter
            var model = new PagedList<Employee>(
                employees.Skip((filter.PageNumber - 1) * filter.PageSize).Take(filter.PageSize).ToList(),
                employees.Count,
                filter.PageNumber,
                filter.PageSize,
                filter
            );

            return View(model);
        }
        public async Task<IActionResult> Create()
        {
            ViewBag.Positions = await _serviceManager.EmployeeService.GetDistinctPositionsAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _serviceManager.EmployeeService.CreateAsync(employee);
                return RedirectToAction("Index");
            }

            ViewBag.Positions = await _serviceManager.EmployeeService.GetDistinctPositionsAsync();
            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int employeeId)
        {
            var employee = await _serviceManager.EmployeeService.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.Positions = await _serviceManager.EmployeeService.GetDistinctPositionsAsync();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int employeeId, Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _serviceManager.EmployeeService.UpdateAsync(employeeId, employee);
                return RedirectToAction("Index");
            }

            ViewBag.Positions = await _serviceManager.EmployeeService.GetDistinctPositionsAsync();
            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int employeeId)
        {
            var employee = await _serviceManager.EmployeeService.GetByIdAsync(employeeId);

            if (employee == null)
                return NotFound();

            employee.IsDeleted = !employee.IsDeleted;

            await _serviceManager.EmployeeService.UpdateAsync(employeeId, employee);
            return RedirectToAction("Index");
        }

    }
}
