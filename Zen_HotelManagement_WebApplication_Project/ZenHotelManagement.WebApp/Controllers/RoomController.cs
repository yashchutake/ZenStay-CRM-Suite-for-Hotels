using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Models.ViewModel;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Controllers
{

   
    public class RoomController : Controller
    {
        private readonly IServiceManager
            _serviceManager;

        public RoomController(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Index(RoomFilterModel? filter)
        {
            var allRooms = _serviceManager.RoomService.GetAllRooms();
            var rooms = allRooms.AsEnumerable();

            // Calculate total available rooms before applying any filters
            ViewBag.AvailableRoomsCount = allRooms.Count(r => !r.IsDeleted && r.RoomStatus);
            ViewBag.AvailableCleanRooms = allRooms.Count(r => !r.IsDeleted && r.IsClean);

            filter ??= new RoomFilterModel();

            if (!filter.IsDeleted.HasValue)
            {
                filter.IsDeleted = false; // Default to not showing deleted rooms
            }

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                rooms = rooms.Where(r =>
                    (r.RoomNo?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.RoomType?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.RoomType))
            {
                rooms = rooms.Where(r =>
                    r.RoomType?.Equals(filter.RoomType, StringComparison.OrdinalIgnoreCase) ?? false
                );
            }

            if (filter.IsAc.HasValue)
            {
                rooms = rooms.Where(r => r.IsAc == filter.IsAc.Value);
            }

            if (filter.IsClean.HasValue)
            {
                rooms = rooms.Where(r => r.IsClean == filter.IsClean.Value);
            }

            if (filter.RoomStatus.HasValue)
            {
                rooms = rooms.Where(r => r.RoomStatus == filter.RoomStatus.Value);
            }
            if (filter.IsDeleted.HasValue)
            {
                rooms = rooms.Where(r => r.IsDeleted == filter.IsDeleted.Value);
            }


            // Get distinct room types for dropdown
            var roomTypes = allRooms
                .Where(r => !string.IsNullOrWhiteSpace(r.RoomType))
                .Select(r => r.RoomType)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            ViewBag.RoomTypes = roomTypes;

            int pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 5 : filter.PageSize;

            var roomsList = rooms.ToList();
            var pagedList = new PagedList<Room>(
                roomsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                roomsList.Count,
                pageNumber,
                pageSize
            );

            ViewBag.CurrentFilter = filter;
            return View(pagedList);
        }

        [Authorize(Roles = "Receptionist")]
        public IActionResult Display(RoomFilterModel? filter)
        {
            var allRooms = _serviceManager.RoomService.GetAllRooms();
            var rooms = allRooms.AsEnumerable();
            ViewBag.AvailableRoomsCount = allRooms.Count(r => r.RoomStatus);

            // Only show available and not deleted rooms for public display
            rooms = rooms.Where(r => !r.IsDeleted);

            // Populate dropdowns for filtering
            ViewBag.RoomTypes = allRooms
                .Where(r => !string.IsNullOrWhiteSpace(r.RoomType))
                .Select(r => r.RoomType)
                .Distinct()
                .OrderBy(r => r)
                .ToList();

            ViewBag.Capacities = allRooms
                .Select(r => r.RoomCapacity)
                .Distinct()
                .OrderBy(c => c)
                .ToList();

            filter ??= new RoomFilterModel();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                rooms = rooms.Where(r =>
                    (r.RoomNo?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (r.RoomType?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                );
            }

            if (!string.IsNullOrWhiteSpace(filter.RoomType))
            {
                rooms = rooms.Where(r =>
                    r.RoomType?.Equals(filter.RoomType, StringComparison.OrdinalIgnoreCase) ?? false
                );
            }

            if (filter.IsAc.HasValue)
            {
                rooms = rooms.Where(r => r.IsAc == filter.IsAc.Value);
            }

            if (filter.IsClean.HasValue)
            {
                rooms = rooms.Where(r => r.IsClean == filter.IsClean.Value);
            }

            if (filter.RoomStatus.HasValue)
            {
                rooms = rooms.Where(r => r.RoomStatus == filter.RoomStatus.Value);
            }

            // Pagination
            int pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
            int pageSize = filter.PageSize < 1 ? 5 : filter.PageSize;

            var roomsList = rooms.ToList();
            var pagedList = new PagedList<Room>(
                roomsList.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                roomsList.Count,
                pageNumber,
                pageSize
            );

            ViewBag.CurrentFilter = filter;
            return View(pagedList);
        }




        // Get room by number
        [Authorize(Roles = "Administrator")]
        public IActionResult Details(string roomNo)
        {
            if (string.IsNullOrEmpty(roomNo))
                return NotFound();

            var room = _serviceManager.RoomService.GetRoomByNoAsync(roomNo);
            if (room == null)
                return NotFound();

            return View(room);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult ByType(string roomType)
        {
            if (string.IsNullOrEmpty(roomType))
                return View("Index", Enumerable.Empty<Room>());

            var rooms = _serviceManager.RoomService.GetRoomsByType(roomType);
            return View("Index", rooms);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Filter(string? roomType, string? isAc, string? isClean, string? roomStatus, string? searchTerm, int pageNumber = 1)
        {
            var filter = new RoomFilterModel
            {
                RoomType = roomType,
                IsAc = !string.IsNullOrEmpty(isAc) ? (isAc == "true") : null,
                IsClean = !string.IsNullOrEmpty(isClean) ? (isClean == "true") : null,
                RoomStatus = !string.IsNullOrEmpty(roomStatus) ? (roomStatus == "true") : null,
                SearchTerm = searchTerm,
                PageNumber = pageNumber
            };

            return RedirectToAction(nameof(Index), filter);
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Create()
        {
            var room = new Room
            {
                RoomStatus = true, // Default to available
                IsClean = true,    // Default to clean
                IsDeleted = false  // Default to not deleted
            };
            return View(room);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("RoomNo,RoomType,IsAc,CostPerNight,RoomCapacity")] Room room)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Set default values for fields not in the form
                    room.RoomStatus = true;  // New rooms are available by default
                    room.IsClean = true;     // New rooms are clean by default
                    room.IsDeleted = false;  // New rooms are not deleted

                    _serviceManager.RoomService.CreateRoom(room);
                    TempData["SuccessMessage"] = $"Room {room.RoomNo} has been created successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    // Handle duplicate room number error
                    if (ex.Message.Contains("duplicate") || ex.Message.Contains("unique") ||
                        ex.Message.Contains("already exists") || ex.Message.Contains("constraint"))
                    {
                        TempData["RoomExistsError"] = $"Room number '{room.RoomNo}' already exists. Please choose a different room number.";
                    }
                    else
                    {
                        TempData["RoomExistsError"] = "Room Already Exists";
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            // Ensure default values are set for display
            room.RoomStatus = true;
            room.IsClean = true;
            room.IsDeleted = false;

            return View(room);
        }

        // GET: Room/Edit?roomNo=...
        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public IActionResult Edit(string roomNo)
        {
            var room = _serviceManager.RoomService.GetRoomByNoAsync(roomNo); // Or however you fetch the room
            if (room == null)
            {
                return NotFound();
            }
            return View(room);
        }


        // POST: Room/Edit
        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit([Bind("RoomNo,RoomStatus,IsClean")] Room room)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _serviceManager.RoomService.UpdateRoom(room.RoomNo!, room);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, $"Error updating room: {ex.Message}");
                }
            }
            return View(room);
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRoomCleanStatus(string roomNo)
        {
            try
            {
                if (string.IsNullOrEmpty(roomNo))
                {
                    TempData["ErrorMessage"] = "Room number is required.";
                    return RedirectToAction(nameof(Index));
                }

                // Use the new method that sends the correct data structure
                var success = await _serviceManager.RoomService.UpdateRoomCleanStatusAsync(roomNo, true);

                if (success)
                {
                    TempData["SuccessMessage"] = $"Room {roomNo} marked as cleaned successfully!";
                }
                else
                {
                    TempData["ErrorMessage"] = $"Failed to update room {roomNo} status.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating room status: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [Authorize(Roles = "Administrator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(string roomNo)
        {
            try
            {
                if (string.IsNullOrEmpty(roomNo))
                {
                    TempData["ErrorMessage"] = "Room number is required.";
                    return RedirectToAction(nameof(Index));
                }

                // Get the current state of the room to determine if we're deleting or restoring
                var room = await _serviceManager.RoomService.GetRoomByNoAsync(roomNo);
                var isRestoring = room?.IsDeleted ?? false;

                var success = await _serviceManager.RoomService.DeleteRoomAsync(roomNo);

                if (success)
                {
                    TempData["SuccessMessage"] = isRestoring
                        ? $"Room {roomNo} has been restored and marked as available!"
                        : $"Room {roomNo} has been marked as inactive and unavailable!";
                }
                else
                {
                    TempData["ErrorMessage"] = isRestoring
                        ? $"Failed to restore room {roomNo}."
                        : $"Failed to mark room {roomNo} as inactive.";
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating room status: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }



    }
}
