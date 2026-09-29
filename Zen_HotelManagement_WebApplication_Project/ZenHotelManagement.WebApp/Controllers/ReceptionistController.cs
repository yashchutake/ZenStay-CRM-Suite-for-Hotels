using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ZenHotelManagement.WebApp.Services.Contracts;
using ZenHotelManagement.WebApp.Services;

namespace ZenHotelManagement.WebApp.Controllers
{
    [Authorize(Roles = "Receptionist")] 
    public class ReceptionistController : Controller
    {
        private readonly IReceptionistDashboardService _receptionistDashboardService;

        public ReceptionistController(IReceptionistDashboardService receptionistDashboardService)
        {
            _receptionistDashboardService = receptionistDashboardService;
        }        public async Task<IActionResult> Dashboard()
        {
            ViewData["AvailableRooms"] = await _receptionistDashboardService.GetAvailableRoomsCountAsync();
            ViewData["CheckedInCustomers"] = await _receptionistDashboardService.GetCheckedInCustomersCountAsync();
            ViewData["AvailableCabDrivers"] = await _receptionistDashboardService.GetAvailableCabDriversCountAsync();
            
            // Get daily check-ins data
            var dailyCheckInsData = await _receptionistDashboardService.GetDailyCheckInsAsync();
            ViewData["DailyCheckInsData"] = JsonSerializer.Serialize(dailyCheckInsData);
            
            // Get today's arrivals data
            ViewData["TodayArrivals"] = await _receptionistDashboardService.GetTodayArrivalsAsync();
            
            return View();
        }
    }
}
