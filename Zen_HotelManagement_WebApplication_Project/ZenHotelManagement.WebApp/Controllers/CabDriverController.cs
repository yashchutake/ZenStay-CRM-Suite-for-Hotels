using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Models.ViewModel;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Controllers
{
    public class CabDriverController : Controller
    {
        private readonly IServiceManager _serviceManager;

        public CabDriverController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        public IActionResult Create() => View();

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Create(CabDriver cabDriver)
        {
            if (ModelState.IsValid)
            {
                await _serviceManager.CabDriverService.CreateAsync(cabDriver);
                TempData["Success"] = $"Cab Driver {cabDriver.Name} was successfully created!";
                return RedirectToAction("Index");
            }

            return View(cabDriver);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> Edit(int cabDriverId)
        {
            var cabDriver = await _serviceManager.CabDriverService.GetByIdAsync(cabDriverId);
            if (cabDriver == null)
            {
                return NotFound();
            }
            return View(cabDriver);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> Edit(int cabDriverId, CabDriver cabDriver)
        {
            if (ModelState.IsValid)
            {
                await _serviceManager.CabDriverService.UpdateAsync(cabDriverId, cabDriver);
                TempData["Success"] = $"Cab Driver {cabDriver.Name} was successfully updated!";
                return RedirectToAction("Index");
            }

            return View(cabDriver);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        public async Task<IActionResult> ToggleStatus(int cabDriverId)
        {
            var driver = await _serviceManager.CabDriverService.GetByIdAsync(cabDriverId);

            if (driver == null)
                return NotFound();

            // Toggle the IsDeleted status
            driver.IsDeleted = !driver.IsDeleted;

            // When changing to Current (IsDeleted = false), set IsAvailable to true
            if (!driver.IsDeleted)
            {
                driver.IsAvailable = true;
                TempData["Success"] = $"Cab Driver {driver.Name} was successfully restored!";
            }
            else
            {
                TempData["Info"] = $"Cab Driver {driver.Name} was marked as past!";
            }

            await _serviceManager.CabDriverService.UpdateAsync(cabDriverId, driver);
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Receptionist")]
        public async Task<IActionResult> Display(CabDriverFilterModel? filter)
        {
            var allDrivers = await _serviceManager.CabDriverService.GetAllAsync();
            PopulateViewBags(allDrivers);

            filter ??= new CabDriverFilterModel();
            var cabDrivers = ApplyFilters(allDrivers, filter);

            int pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 5 : filter.PageSize;

            var pagedList = new PagedList<CabDriver>(
                cabDrivers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                cabDrivers.Count,
                pageNumber,
                pageSize
            );

            ViewBag.CurrentFilter = filter;
            return View(pagedList);
        }
        [Authorize(Roles = "Administrator,Receptionist")]
        private void PopulateViewBags(IEnumerable<CabDriver> allDrivers)
        {
            ViewBag.ActiveCabDrivers = allDrivers.Count(d => !d.IsDeleted);
            ViewBag.AvailableCabDrivers = allDrivers.Count(d => !d.IsDeleted && d.IsAvailable);

            ViewBag.CarVendors = allDrivers
                .Where(d => !string.IsNullOrEmpty(d.CarVendor))
                .Select(d => d.CarVendor)
                .Distinct()
                .OrderBy(v => v)
                .ToList();

            ViewBag.CarTypes = allDrivers
                .Where(d => !string.IsNullOrEmpty(d.CarType))
                .Select(d => d.CarType)
                .Distinct()
                .OrderBy(t => t)
                .ToList();

            ViewBag.Genders = allDrivers
                .Where(d => !string.IsNullOrEmpty(d.Gender))
                .Select(d => d.Gender)
                .Distinct()
                .OrderBy(g => g)
                .ToList();
        }

        [Authorize(Roles = "Administrator,Receptionist")]
        private List<CabDriver> ApplyFilters(IEnumerable<CabDriver> allDrivers, CabDriverFilterModel filter)
        {
            var cabDrivers = allDrivers;

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                cabDrivers = cabDrivers.Where(d =>
                    (d.Name?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.CarVendor?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (d.CarType?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                ).ToList();
            }
            if (!string.IsNullOrWhiteSpace(filter.CarVendor))
            {
                cabDrivers = cabDrivers.Where(d =>
                    d.CarVendor != null &&
                    d.CarVendor.Trim().Equals(filter.CarVendor.Trim(), StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.CarType))
            {
                cabDrivers = cabDrivers.Where(d =>
                    d.CarType != null &&
                    d.CarType.Trim().Equals(filter.CarType.Trim(), StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filter.Gender))
            {
                cabDrivers = cabDrivers.Where(d =>
                    d.Gender?.Equals(filter.Gender, StringComparison.OrdinalIgnoreCase) ?? false
                ).ToList();
            }
            if (filter.IsCurrent.HasValue)
            {
                cabDrivers = cabDrivers.Where(d => !d.IsDeleted == filter.IsCurrent.Value).ToList();
            }

            return cabDrivers.ToList();
        }
        [Authorize(Roles = "Administrator,Receptionist")]
        public async Task<IActionResult> Index(CabDriverFilterModel? filter)
        {
            var allDrivers = await _serviceManager.CabDriverService.GetAllAsync();
            PopulateViewBags(allDrivers);

            filter ??= new CabDriverFilterModel();
            var cabDrivers = ApplyFilters(allDrivers, filter);

            int pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 5 : filter.PageSize;

            var pagedList = new PagedList<CabDriver>(
                cabDrivers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                cabDrivers.Count,
                pageNumber,
                pageSize
            );

            ViewBag.CurrentFilter = filter;
            return View(pagedList);
        }

        [Authorize(Roles = "Administrator,Receptionist")]
        [HttpGet]
        public async Task<IActionResult> GetAll(int pageNumber = 1, int pageSize = 5)
        {
            var drivers = await _serviceManager.CabDriverService.GetAllAsync();
            var pagedDrivers = drivers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return Ok(new
            {
                Data = pagedDrivers,
                TotalCount = drivers.Count
            });
        }
    }
}
