using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AdminController : Controller
    {
        private readonly IAdminDashboardService _adminDashboardService;

        public AdminController(IAdminDashboardService adminDashboardService)
        {
            _adminDashboardService = adminDashboardService;
        }

        public async Task<IActionResult> Dashboard()
        {
            ViewData["TotalEmployees"] = await _adminDashboardService.GetTotalEmployeesCountAsync();
            ViewData["TotalRooms"] = await _adminDashboardService.GetActiveRoomsCountAsync();

            ViewData["TotalCabDrivers"] = await _adminDashboardService.GetTotalCabDriversCountAsync();

            // Get department distribution data
            var departmentData = await _adminDashboardService.GetEmployeesByDepartmentAsync();
            ViewData["DepartmentData"] = JsonSerializer.Serialize(departmentData);

            // Get vacant rooms data
            ViewData["VacantRooms"] = await _adminDashboardService.GetVacantRoomsAsync();

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateRoomCleanStatus(string roomNo)
        {
            var success = await _adminDashboardService.UpdateRoomCleanStatusAsync(roomNo, true);
            TempData["Message"] = success ? "Room marked as cleaned successfully!" : "Failed to update room status.";
            return RedirectToAction(nameof(Dashboard));
        }
    }
}
