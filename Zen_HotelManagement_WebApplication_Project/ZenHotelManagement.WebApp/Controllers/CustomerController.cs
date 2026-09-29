using Microsoft.AspNetCore.Mvc;
using ZenHotelManagement.WebApp.Services.Contracts;
using ZenHotelManagement.WebApp.Models.ViewModel;
using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Utilities;
using Microsoft.AspNetCore.Authorization;

namespace ZenHotelManagement.WebApp.Controllers
{
    [Authorize(Roles = "Receptionist")]
    public class CustomerController: Controller
    {
        private readonly IServiceManager _serviceManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ICheckoutCalculationService _checkoutCalculationService;

        public CustomerController(IServiceManager serviceManager, IHttpClientFactory httpClientFactory, ICheckoutCalculationService checkoutCalculationService)
        {
            _serviceManager = serviceManager;
            _httpClientFactory = httpClientFactory;
            _checkoutCalculationService = checkoutCalculationService;
        }        public async Task<IActionResult> Index(CustomerFilterModel? filter)
        {
            try
            {
                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                
                // Get distinct customers based on CustomerId
                var distinctCustomers = allCustomers
                    .GroupBy(c => c.CustomerId)
                    .Select(g => g.First())
                    .ToList();                // Populate ViewBag with ALL available values before filtering (not from filtered customers)
                ViewBag.CustomerTypes = new List<string>(); // Customer model doesn't have CustomerType property

                // Get all possible genders from all customers (not filtered)
                ViewBag.Genders = new List<string> { "Male", "Female", "Other" };

                // Get all possible customer statuses
                ViewBag.CustomerStatuses = new List<string> 
                { 
                    BookingStatusBadge.CustomerStatus.CheckedIn,
                    BookingStatusBadge.CustomerStatus.ActiveBooking,
                    BookingStatusBadge.CustomerStatus.RecentGuest,
                    BookingStatusBadge.CustomerStatus.PendingPayment,
                    BookingStatusBadge.CustomerStatus.NoBookings
                };

                filter ??= new CustomerFilterModel();
                var customers = distinctCustomers;                // Apply filters
                if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
                {
                    customers = customers.Where(c =>
                        (c.Name?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false) ||
                        (c.CustomerId.ToString().Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase)) ||
                        (c.MobileNo?.Contains(filter.SearchTerm, StringComparison.OrdinalIgnoreCase) ?? false)
                    ).ToList();
                }                // CustomerType filter removed - Customer model doesn't have CustomerType property

                if (!string.IsNullOrWhiteSpace(filter.Gender))
                {
                    customers = customers.Where(c =>
                        c.Gender?.Equals(filter.Gender, StringComparison.OrdinalIgnoreCase) ?? false
                    ).ToList();
                }                if (filter.IsCurrent.HasValue)
                {
                    // Filter based on whether customer has active bookings
                    var currentTime = DateTime.Now;
                    customers = customers.Where(c =>
                    {
                        var hasActiveRoomBookings = c.RoomBookings?.Any(rb => rb.CheckOutDate > currentTime) ?? false;
                        var hasActiveCabBookings = c.CabBookings?.Any(cb => cb.DropOffDateTime > currentTime && (cb.IsRideCompleted == false || cb.IsRideCompleted == null)) ?? false;
                        var hasActiveBookings = hasActiveRoomBookings || hasActiveCabBookings;
                        return hasActiveBookings == filter.IsCurrent.Value;
                    }).ToList();
                }

                // Filter by customer status
                if (!string.IsNullOrWhiteSpace(filter.Status))
                {
                    customers = customers.Where(c => GetCustomerStatus(c) == filter.Status).ToList();
                }// Sort customers by priority: Checked In first, then Confirmed, then remaining
                customers = customers.OrderBy(c => GetCustomerPriority(c)).ThenBy(c => c.Name).ToList();

                int pageNumber = filter.PageNumber < 1 ? 1 : filter.PageNumber;
                int pageSize = filter.PageSize < 1 ? 10 : filter.PageSize;

                var pagedList = new PagedList<Customer>(
                    customers.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList(),
                    customers.Count,
                    pageNumber,
                    pageSize
                );// Calculate stats for the new requirements
                var allCustomersForStats = customers; // Use filtered customers for stats
                var totalRegisteredCustomers = allCustomersForStats.Count;
                var totalCheckedInCustomers = allCustomersForStats.Count(c => GetCustomerStatus(c) == "Checked In");
                var totalConfirmedCustomers = allCustomersForStats.Count(c => GetCustomerStatus(c) == "Active Booking");

                ViewBag.TotalRegisteredCustomers = totalRegisteredCustomers;
                ViewBag.TotalCheckedInCustomers = totalCheckedInCustomers;
                ViewBag.TotalConfirmedCustomers = totalConfirmedCustomers;

                // Add status calculation function to ViewBag
                ViewBag.GetCustomerStatus = new Func<Customer, string>(GetCustomerStatus);
                ViewBag.CurrentFilter = filter;
                
                return View(pagedList);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading customers: {ex.Message}";
                return View(new PagedList<Customer>(new List<Customer>(), 0, 1, 10));
            }
        }[HttpGet]
        public async Task<IActionResult> Create(bool fromSummary = false)
        {
            CustomerCreateViewModel viewModel;
            
            // Get current session ID for tracking
            var currentSessionId = HttpContext.Session.Id;
            var sessionKeyForData = $"PendingCustomerModel_{currentSessionId}";
            
            // Check if there's pending data from summary page (user clicked "Back to Edit")
            var pendingModelJson = TempData.Peek("PendingCustomerModel") as string;
            var sessionData = HttpContext.Session.GetString(sessionKeyForData);
            
            if (!string.IsNullOrEmpty(pendingModelJson) && fromSummary && !string.IsNullOrEmpty(sessionData))
            {
                try
                {
                    viewModel = System.Text.Json.JsonSerializer.Deserialize<CustomerCreateViewModel>(pendingModelJson);
                    // Keep the data in TempData for potential future use
                    TempData.Keep("PendingCustomerModel");
                    TempData["Info"] = "You are editing a booking. Make changes and submit again to see the updated summary.";
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deserializing pending model: {ex.Message}");
                    viewModel = new CustomerCreateViewModel();
                    // Clear invalid data
                    TempData.Remove("PendingCustomerModel");
                    HttpContext.Session.Remove(sessionKeyForData);
                }
            }
            else
            {
                // Clear any stale data if not coming from summary or session mismatch
                if (!string.IsNullOrEmpty(pendingModelJson) && !fromSummary)
                {
                    TempData.Remove("PendingCustomerModel");
                    HttpContext.Session.Remove(sessionKeyForData);
                    TempData["Info"] = "Previous form data has been cleared.";
                }
                
                viewModel = new CustomerCreateViewModel();
                // Initialize with one room booking for new forms
                viewModel.RoomBookings.Add(new RoomBookingCreateViewModel());
            }
            
            // Load available rooms
            try
            {
                viewModel.AvailableRooms = (await _serviceManager.RoomService.GetAvailableRoomsAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["Warning"] = $"Could not load available rooms: {ex.Message}";
                viewModel.AvailableRooms = new List<Room>();
            }

            // Load available cab drivers
            try
            {
                viewModel.AvailableCabDrivers = (await _serviceManager.CabDriverService.GetAvailableCabDriversAsync()).ToList();
            }
            catch (Exception ex)
            {
                TempData["Warning"] = $"Could not load available cab drivers: {ex.Message}";
                viewModel.AvailableCabDrivers = new List<CabDriver>();
            }
            
            return View(viewModel);
        }[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateViewModel model)
        {            // Validate that at least one room booking exists
            if (model.RoomBookings == null || !model.RoomBookings.Any())
            {
                ModelState.AddModelError("", "At least one room booking is required.");
            }            // Load available rooms for validation
            try
            {
                model.AvailableRooms = (await _serviceManager.RoomService.GetAvailableRoomsAsync()).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading available rooms: {ex.Message}");
                ModelState.AddModelError("", "Unable to load available rooms. Please try again.");
                model.AvailableRooms = new List<Room>();
            }

            // Validate room bookings using the available rooms list
            for (int i = 0; i < model.RoomBookings.Count; i++)
            {
                var booking = model.RoomBookings[i];
                
                if (booking.RoomId <= 0)
                {
                    ModelState.AddModelError($"RoomBookings[{i}].RoomId", "Please select a room");
                    continue;
                }

                // Find room in available rooms list
                var room = model.AvailableRooms?.FirstOrDefault(r => r.RoomId == booking.RoomId);
                
                if (room == null)
                {
                    ModelState.AddModelError($"RoomBookings[{i}].RoomId", 
                        "Selected room is not available. Please choose a different room.");
                    continue;
                }                // Update booking with room details
                booking.CostPerNight = room.CostPerNight;
                
                // Validate advance payment - only check that it doesn't exceed one night's charge
                if (booking.AmountPaid < 0)
                {
                    ModelState.AddModelError($"RoomBookings[{i}].AmountPaid", 
                        "Advance payment cannot be negative");
                }
                else if (booking.AmountPaid > room.CostPerNight)
                {
                    ModelState.AddModelError($"RoomBookings[{i}].AmountPaid", 
                        $"Advance payment for {room.RoomNo} cannot exceed one night's charge (₹{room.CostPerNight:N2})");
                }
            }

            // No advance payment validation needed - full payment at checkout
            if (!ModelState.IsValid)
            {
                // Reload available rooms for the dropdowns
                try
                {
                    model.AvailableRooms = (await _serviceManager.RoomService.GetAvailableRoomsAsync()).ToList();
                }
                catch (Exception ex)
                {
                    TempData["Warning"] = $"Could not load available rooms: {ex.Message}";
                    model.AvailableRooms = new List<Room>();
                }

                // Reload available cab drivers
                try
                {
                    model.AvailableCabDrivers = (await _serviceManager.CabDriverService.GetAvailableCabDriversAsync()).ToList();
                }
                catch (Exception ex)
                {
                    TempData["Warning"] = $"Could not load available cab drivers: {ex.Message}";
                    model.AvailableCabDrivers = new List<CabDriver>();
                }

                return View(model);
            }            try
            {
                // Automate booking fields before processing
                var random = new Random();
                var currentDate = DateTime.Now;                // Process each room booking and calculate costs
                for (int i = 0; i < model.RoomBookings.Count; i++)
                {
                    var booking = model.RoomBookings[i];
                    
                    // Set booking date to current date/time
                    booking.BookingDate = currentDate;
                    
                    // Handle custom dates for additional rooms
                    if (i > 0 && !model.UseCustomDatesForAdditionalRooms)
                    {
                        // For additional rooms when not using custom dates, 
                        // sync dates from the primary booking (index 0)
                        var primaryBooking = model.RoomBookings[0];
                        booking.CheckInDate = primaryBooking.CheckInDate;
                        booking.CheckOutDate = primaryBooking.CheckOutDate;
                        Console.WriteLine($"Synced dates for additional room {i}: CheckIn={booking.CheckInDate}, CheckOut={booking.CheckOutDate}");
                    }
                    
                    // Determine booking status based on check-in date with minute precision
                    if (booking.CheckInDate > currentDate)
                    {
                        booking.BookingStatus = "Confirmed";
                    }
                    else if (booking.CheckInDate <= currentDate)
                    {
                        booking.BookingStatus = "Checked-In";
                    }
                    else
                    {
                        booking.BookingStatus = "Confirmed"; // Default fallback
                    }
                      // Calculate cost based on room's CostPerNight
                    try
                    {
                        var room = model.AvailableRooms?.FirstOrDefault(r => r.RoomId == booking.RoomId);
                        if (room != null)
                        {
                            // Update booking with calculated total amount
                            booking.CalculateTotalAmount(room.CostPerNight);
                            Console.WriteLine($"Calculated costs for Room {room.RoomNo}: Amount={booking.Amount}, Paid={booking.AmountPaid}");
                        }
                        else
                        {
                            // Fallback: Try to fetch room details from API
                            try
                            {
                                var roomFromApi = await _serviceManager.RoomService.GetRoomByIdAsync(booking.RoomId);
                                if (roomFromApi != null)
                                {
                                    booking.CalculateTotalAmount(roomFromApi.CostPerNight);
                                    Console.WriteLine($"Calculated costs for Room {roomFromApi.RoomNo} (from API): Amount={booking.Amount}, Paid={booking.AmountPaid}");
                                }
                                else
                                {
                                    // Ultimate fallback: Use existing amount or default calculation
                                    if (booking.Amount <= 0)
                                    {
                                        booking.Amount = 1000m; // Default room rate
                                    }
                                    Console.WriteLine($"Used fallback calculation for Room {booking.RoomId}: Amount={booking.Amount}, Paid={booking.AmountPaid}");
                                }
                            }
                            catch (Exception apiEx)
                            {
                                Console.WriteLine($"Error fetching room from API: {apiEx.Message}");
                                // Ultimate fallback
                                if (booking.Amount <= 0)
                                {
                                    booking.Amount = 1000m; // Default room rate
                                }
                            }
                        }
                    }
                    catch (Exception calcEx)
                    {
                        Console.WriteLine($"Error calculating costs for booking: {calcEx.Message}");
                        // Ensure we have valid amounts even if calculation fails
                        if (booking.Amount <= 0)
                        {
                            booking.Amount = 1000m; // Default room rate
                        }
                    }
                }                // Handle cab booking automation if included
                if (model.IncludeCabBooking)
                {
                    // Auto-select an available cab driver if not already selected
                    if (!model.CabDriverId.HasValue)
                    {
                        var availableDrivers = await _serviceManager.CabDriverService.GetAvailableCabDriversAsync();
                        if (availableDrivers.Any())
                        {
                            var randomDriver = availableDrivers[random.Next(availableDrivers.Count)];
                            model.CabDriverId = randomDriver.CabDriverId;
                        }
                    }

                    // Generate random cab fare between 200-600 if not set
                    if (!model.CabFare.HasValue || model.CabFare <= 0)
                    {
                        model.CabFare = random.Next(200, 601);
                    }
                    
                    // Set default pickup time if not provided (2 hours from now)
                    if (!model.CabPickUpDateTime.HasValue || model.CabPickUpDateTime == DateTime.MinValue)
                    {
                        model.CabPickUpDateTime = currentDate.AddHours(2);
                    }
                    
                    // Automatically set drop-off time 20-50 minutes after pick-up time
                    if (!model.CabDropOffDateTime.HasValue || model.CabDropOffDateTime == DateTime.MinValue)
                    {
                        var minutesToAdd = random.Next(20, 51); // 20-50 minutes
                        model.CabDropOffDateTime = model.CabPickUpDateTime.Value.AddMinutes(minutesToAdd);
                    }
                    
                    Console.WriteLine($"Cab booking automation: PickUp={model.CabPickUpDateTime}, DropOff={model.CabDropOffDateTime}, Driver={model.CabDriverId}, Fare={model.CabFare}");
                }

                // NOTE: Database save is deferred until summary confirmation
                // The customer and bookings will be created only after user confirms on summary page
                
                // Ensure we have fresh room data before creating summary
                try
                {
                    if (model.AvailableRooms == null || !model.AvailableRooms.Any())
                    {
                        model.AvailableRooms = (await _serviceManager.RoomService.GetAvailableRoomsAsync()).ToList();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Warning: Could not reload available rooms for summary: {ex.Message}");
                    // Continue with existing rooms or empty list
                    model.AvailableRooms ??= new List<Room>();
                }
                
                // Create booking summary with calculated costs
                var summary = await CreateBookingSummary(model);
                  // Store the model in TempData for later confirmation
                TempData["PendingCustomerModel"] = System.Text.Json.JsonSerializer.Serialize(model);
                
                // Store session tracking data
                var currentSessionId = HttpContext.Session.Id;
                var sessionKeyForData = $"PendingCustomerModel_{currentSessionId}";
                HttpContext.Session.SetString(sessionKeyForData, "active");
                
                TempData["Info"] = "Review your booking details below. Click 'Confirm Booking' to finalize the reservation.";
                
                return View("BookingSummary", summary);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error creating customer: {ex.Message}";
                // Reload available rooms for the dropdowns
                try
                {
                    model.AvailableRooms = (await _serviceManager.RoomService.GetAvailableRoomsAsync()).ToList();
                }                catch
                {
                    model.AvailableRooms = new List<Room>();
                }
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmBooking()
        {
            try
            {
                // Retrieve the pending customer model from TempData
                var pendingModelJson = TempData["PendingCustomerModel"] as string;
                if (string.IsNullOrEmpty(pendingModelJson))
                {
                    TempData["Error"] = "Booking session expired. Please create the booking again.";
                    return RedirectToAction("Create");
                }

                var model = System.Text.Json.JsonSerializer.Deserialize<CustomerCreateViewModel>(pendingModelJson);
                if (model == null)
                {
                    TempData["Error"] = "Invalid booking data. Please create the booking again.";
                    return RedirectToAction("Create");
                }                // Now save the customer and bookings to database
                var customer = model.ToCustomer();                await _serviceManager.CustomerService.AddCustomer(customer);
                
                // Clear the pending data since booking is confirmed
                TempData.Remove("PendingCustomerModel");
                
                // Clear session tracking data
                var currentSessionId = HttpContext.Session.Id;
                var sessionKeyForData = $"PendingCustomerModel_{currentSessionId}";
                HttpContext.Session.Remove(sessionKeyForData);
                
                TempData["Success"] = "Customer and booking(s) created successfully!";
                
                // Redirect to customer details page
                return RedirectToAction("Details", new { customerId = model.CustomerId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error confirming booking: {ex.Message}";
                return RedirectToAction("Create");
            }
        }

        private async Task<BookingSummaryViewModel> CreateBookingSummary(CustomerCreateViewModel model)
        {
            // Debug logging
            Console.WriteLine($"CreateBookingSummary called with {model.RoomBookings?.Count ?? 0} room bookings");
            Console.WriteLine($"Available rooms count: {model.AvailableRooms?.Count ?? 0}");
              var summary = new BookingSummaryViewModel
            {
                CustomerName = model.Name,
                CustomerId = model.CustomerId,
                MobileNo = model.MobileNo,
                RoomBookings = new List<RoomBookingSummary>(),
                TotalRoomAmount = 0,
                TotalAmountPaid = 0,
                IncludeCabBooking = model.IncludeCabBooking
            };

            // Get room details for each booking and calculate costs
            foreach (var booking in model.RoomBookings)
            {
                try
                {
                    // First try to find room in available rooms, then fetch from API if not found
                    var room = model.AvailableRooms?.FirstOrDefault(r => r.RoomId == booking.RoomId);
                      if (room == null)
                    {
                        // Fetch room details from API
                        try
                        {
                            room = await _serviceManager.RoomService.GetRoomByIdAsync(booking.RoomId);
                        }
                        catch (Exception apiEx)
                        {
                            Console.WriteLine($"Error fetching room details from API for RoomId {booking.RoomId}: {apiEx.Message}");
                        }
                    }
                      if (room != null)
                    {                        // Calculate total amount based on room's CostPerNight and number of nights
                        var calculatedAmount = CalculateRoomBookingCost(room.CostPerNight, booking.CheckInDate, booking.CheckOutDate);
                          // Calculate number of nights for display using full DateTime precision
                        int numberOfNights = 1; // Default for tentative calculations
                        if (booking.CheckOutDate.HasValue)
                        {
                            var timeDiff = booking.CheckOutDate.Value - booking.CheckInDate;
                            numberOfNights = Math.Max(1, (int)Math.Ceiling(timeDiff.TotalDays));
                        }
                        
                        Console.WriteLine($"Room {room.RoomNo}: CostPerNight={room.CostPerNight}, CalculatedAmount={calculatedAmount}, AmountPaid={booking.AmountPaid}");
                        
                        summary.RoomBookings.Add(new RoomBookingSummary
                        {
                            RoomNo = room.RoomNo ?? "",
                            RoomType = room.RoomType ?? "",
                            CheckInDate = booking.CheckInDate,
                            CheckOutDate = booking.CheckOutDate,
                            NumberOfGuests = booking.NumberOfGuests,
                            NumberOfNights = numberOfNights,
                            Amount = calculatedAmount,
                            AmountPaid = booking.AmountPaid,
                            BookingStatus = booking.BookingStatus
                        });
                        
                        summary.TotalRoomAmount += calculatedAmount;
                        summary.TotalAmountPaid += booking.AmountPaid;
                    }
                    else
                    {
                        Console.WriteLine($"Room with ID {booking.RoomId} not found in available rooms or API");                        // For unknown rooms, use fallback calculation with default rate
                        var fallbackAmount = booking.Amount > 0 ? booking.Amount : 1000m; // Use existing amount or default
                          // Calculate number of nights for display using full DateTime precision
                        int fallbackNights = 1; // Default for tentative calculations
                        if (booking.CheckOutDate.HasValue)
                        {
                            var timeDiff = booking.CheckOutDate.Value - booking.CheckInDate;
                            fallbackNights = Math.Max(1, (int)Math.Ceiling(timeDiff.TotalDays));
                        }
                        
                        // Add booking with placeholder room details
                        summary.RoomBookings.Add(new RoomBookingSummary
                        {
                            RoomNo = $"Room {booking.RoomId}",
                            RoomType = "Unknown",
                            CheckInDate = booking.CheckInDate,
                            CheckOutDate = booking.CheckOutDate,
                            NumberOfGuests = booking.NumberOfGuests,
                            NumberOfNights = fallbackNights,
                            Amount = fallbackAmount,
                            AmountPaid = booking.AmountPaid,
                            BookingStatus = booking.BookingStatus
                        });
                        
                        summary.TotalRoomAmount += fallbackAmount;
                        summary.TotalAmountPaid += booking.AmountPaid;
                    }
                }
                catch (Exception ex)
                {
                    // Log error but continue with other bookings
                    Console.WriteLine($"Error getting room details for RoomId {booking.RoomId}: {ex.Message}");                    // For error cases, use fallback calculation
                    var fallbackAmount = booking.Amount > 0 ? booking.Amount : 1000m; // Use existing amount or default
                    
                    // Calculate number of nights for display
                    int errorFallbackNights = 1; // Default for tentative calculations                    if (booking.CheckOutDate.HasValue)
                    {
                        var timeDiff = booking.CheckOutDate.Value - booking.CheckInDate;
                        errorFallbackNights = Math.Max(1, (int)Math.Ceiling(timeDiff.TotalDays));
                    }
                    
                    // Add booking with placeholder room details to ensure it's not lost
                    summary.RoomBookings.Add(new RoomBookingSummary
                    {
                        RoomNo = $"Room {booking.RoomId}",
                        RoomType = "Error Loading",
                        CheckInDate = booking.CheckInDate,
                        CheckOutDate = booking.CheckOutDate,
                        NumberOfGuests = booking.NumberOfGuests,
                        NumberOfNights = errorFallbackNights,
                        Amount = fallbackAmount,
                        AmountPaid = booking.AmountPaid,
                        BookingStatus = booking.BookingStatus
                    });
                    
                    summary.TotalRoomAmount += fallbackAmount;
                    summary.TotalAmountPaid += booking.AmountPaid;
                }
            }            // Add cab booking details if included
            if (model.IncludeCabBooking)
            {                summary.CabBooking = new CabBookingSummary
                {
                    CabDriverId = model.CabDriverId ?? 0,
                    PickUpDateTime = model.CabPickUpDateTime ?? DateTime.Now.AddHours(2),
                    DropOffDateTime = model.CabDropOffDateTime ?? DateTime.Now.AddHours(3),
                    PickUpLocation = model.CabPickUpLocation ?? "",
                    DropOffLocation = model.CabDropOffLocation ?? "",
                    Fare = model.CabFare ?? 0
                };
                
                // Note: Cab fare will be added to total checkout amount
            }return summary;
        }

        /// <summary>
        /// Calculate room booking cost based on room's cost per night and number of nights
        /// </summary>
        /// <param name="costPerNight">Room's cost per night</param>
        /// <param name="checkInDate">Check-in date</param>
        /// <param name="checkOutDate">Check-out date (nullable)</param>
        /// <returns>Total calculated amount</returns>
        private decimal CalculateRoomBookingCost(decimal costPerNight, DateTime checkInDate, DateTime? checkOutDate)
        {
            // Calculate number of nights
            int numberOfNights = 1; // Default for tentative calculations
              if (checkOutDate.HasValue)
            {
                // Calculate actual number of nights using full DateTime precision
                var timeDiff = checkOutDate.Value - checkInDate;
                numberOfNights = Math.Max(1, (int)Math.Ceiling(timeDiff.TotalDays)); // Minimum 1 night
            }
            // If checkOutDate is null (TBD), use 1 night as tentative calculation
            
            var totalAmount = costPerNight * numberOfNights;
            
            Console.WriteLine($"Cost calculation: CostPerNight={costPerNight}, NumberOfNights={numberOfNights}, TotalAmount={totalAmount}");
            
            return totalAmount;
        }

        public async Task<IActionResult> Details(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    TempData["Error"] = "Customer ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                var customer = allCustomers.FirstOrDefault(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction(nameof(Index));
                }                // Get all customers with the same CustomerId to collect all bookings
                var customersWithSameId = allCustomers.Where(c => c.CustomerId == customerId).ToList();

                // Debug logging
                Console.WriteLine($"Found {customersWithSameId.Count} customers with CustomerId: {customerId}");

                // Collect all room bookings
                var allRoomBookings = new List<RoomBooking>();
                var allCabBookings = new List<CabBooking>();                foreach (var c in customersWithSameId)
                {
                    Console.WriteLine($"Customer {c.Id}: RoomBookings={c.RoomBookings?.Count ?? 0}, CabBookings={c.CabBookings?.Count ?? 0}");                  if (c.RoomBookings != null)
                    {
                        // Apply real-time status calculation with minute precision
                        RealTimeStatusCalculator.ApplyRealTimeStatusToRoomBookings(c.RoomBookings);
                        allRoomBookings.AddRange(c.RoomBookings);
                    }                    
                    if (c.CabBookings != null)
                    {
                        // Apply real-time status calculation with minute precision
                        RealTimeStatusCalculator.ApplyRealTimeStatusToCabBookings(c.CabBookings);
                        allCabBookings.AddRange(c.CabBookings);
                    }
                }

                Console.WriteLine($"Total collected: RoomBookings={allRoomBookings.Count}, CabBookings={allCabBookings.Count}");

                // Group room bookings by check-in date
                var groupedRoomBookings = allRoomBookings
                    .GroupBy(rb => rb.CheckInDate.Date)
                    .OrderBy(g => g.Key)
                    .ToList();

                // Group cab bookings by pickup date
                var groupedCabBookings = allCabBookings
                    .GroupBy(cb => cb.PickUpDateTime.Date)
                    .OrderBy(g => g.Key)
                    .ToList();

                var viewModel = new CustomerDetailsViewModel
                {
                    Customer = customer,
                    GroupedRoomBookings = groupedRoomBookings,
                    GroupedCabBookings = groupedCabBookings
                };                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading customer details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(string customerId)
        {
            try
            {
                if (string.IsNullOrEmpty(customerId))
                {
                    TempData["Error"] = "Customer ID is required.";
                    return RedirectToAction(nameof(Index));
                }

                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                var customer = allCustomers.FirstOrDefault(c => c.CustomerId == customerId);

                if (customer == null)
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction(nameof(Index));
                }

                var viewModel = CustomerUpdateViewModel.FromCustomer(customer);
                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading customer for editing: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var httpClient = _httpClientFactory.CreateClient("API");

                // Prepare the customer data for update (only customer details, no booking data)
                var updateData = new
                {
                    idType = model.IdType,
                    name = model.Name,
                    gender = model.Gender,
                    address = model.Address ?? string.Empty,
                    country = model.Country ?? string.Empty,
                    mobileNo = model.MobileNo
                };

                var json = System.Text.Json.JsonSerializer.Serialize(updateData);
                var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                var response = await httpClient.PutAsync($"api/customers/{model.CustomerId}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Customer details updated successfully!";
                    return RedirectToAction(nameof(Details), new { customerId = model.CustomerId });
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    TempData["Error"] = $"Failed to update customer: {response.StatusCode} - {errorContent}";
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error updating customer: {ex.Message}";
                return View(model);
            }
        }        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckoutBookings([FromBody] CheckoutRequest request)
        {
            try
            {
                if (request?.BookingIds == null || !request.BookingIds.Any())
                {
                    return Json(new { success = false, message = "No bookings selected for checkout." });
                }

                // Get customer details to find the bookings
                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                var customersWithSameId = allCustomers.Where(c => c.CustomerId == request.CustomerId).ToList();

                var allRoomBookings = new List<RoomBooking>();
                foreach (var c in customersWithSameId)
                {
                    if (c.RoomBookings != null)
                        allRoomBookings.AddRange(c.RoomBookings);
                }                var bookingsToCheckout = allRoomBookings.Where(rb => request.BookingIds.Contains(rb.BookingId)).ToList();
                
                if (!bookingsToCheckout.Any())
                {
                    return Json(new { success = false, message = "Selected bookings not found." });
                }

                // Calculate dynamic checkout amounts for accurate payment processing
                var customer = customersWithSameId.First();
                var currentCheckoutDate = DateTime.Now;
                var dynamicCheckout = await _checkoutCalculationService.CalculateDynamicCheckoutAsync(bookingsToCheckout, customer, currentCheckoutDate);

                var httpClient = _httpClientFactory.CreateClient("API");
                var successfulCheckouts = 0;
                var errors = new List<string>();                foreach (var booking in bookingsToCheckout)
                {
                    try
                    {                        // Find the dynamic calculation for this booking to get accurate amounts
                        var calculation = dynamicCheckout.BookingCalculations.FirstOrDefault(bc => bc.BookingId == booking.BookingId);
                        var recalculatedAmount = calculation?.RecalculatedAmount ?? booking.TotalAmount ?? booking.AmountPaid;
                        var bookingCheckoutDate = calculation?.ActualCheckOutDate ?? DateTime.Now;
                          // Customer pays for nights actually stayed (minimum 1 night)
                        // The total amount is recalculated based on actual nights stayed
                        // The amount paid should be updated to reflect full payment (settlement)
                        // The pending amount should be 0 for checked out bookings
                        var updateData = new
                        {
                            RoomId = booking.RoomId,
                            CheckInDate = booking.CheckInDate,
                            CheckOutDate = bookingCheckoutDate, // Use actual calculated checkout date
                            TotalAmount = recalculatedAmount, // Total amount based on nights stayed
                            AmountPaid = recalculatedAmount, // Full payment - customer has settled everything
                            PendingAmount = 0m, // No pending amount for checked out bookings
                            BookingStatus = "Checked Out", // Update status to checked out
                            NumberOfGuests = booking.NumberOfGuests
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(updateData);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PutAsync($"api/roombookings/{booking.BookingId}", content);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            successfulCheckouts++;
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            errors.Add($"Booking {booking.BookingId}: {response.StatusCode} - {errorContent}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Booking {booking.BookingId}: {ex.Message}");
                    }
                }

                if (successfulCheckouts > 0)
                {
                    var message = $"Successfully checked out {successfulCheckouts} booking(s)";
                    if (errors.Any())
                    {
                        message += $". {errors.Count} failed: {string.Join(", ", errors)}";
                    }
                    return Json(new { success = true, message = message });
                }
                else
                {
                    return Json(new { success = false, message = $"Failed to checkout bookings: {string.Join(", ", errors)}" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error during checkout: {ex.Message}" });
            }
        }        public class CheckoutRequest
        {
            public string CustomerId { get; set; }
            public List<int> BookingIds { get; set; }
        }        /// <summary>
        /// Checkout page for processing payments and finalizing checkout with dynamic calculations
        /// </summary>
        public async Task<IActionResult> Checkout(string customerId, int? bookingId = null)
        {
            try
            {
                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                var customersWithSameId = allCustomers.Where(c => c.CustomerId == customerId).ToList();

                if (!customersWithSameId.Any())
                {
                    TempData["Error"] = "Customer not found.";
                    return RedirectToAction("Index");
                }

                var customer = customersWithSameId.First();
                var allRoomBookings = new List<RoomBooking>();
                
                foreach (var c in customersWithSameId)
                {
                    if (c.RoomBookings != null)
                        allRoomBookings.AddRange(c.RoomBookings);
                }

                // Get bookings available for checkout (only checked-in bookings should be eligible for checkout)
                var checkoutableBookings = allRoomBookings.Where(rb => 
                    string.Equals(rb.BookingStatus, "Checked In", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(rb.BookingStatus, "Checked-In", StringComparison.OrdinalIgnoreCase)).ToList();

                // If specific booking ID provided, filter to that booking
                if (bookingId.HasValue)
                {
                    checkoutableBookings = checkoutableBookings.Where(rb => rb.BookingId == bookingId.Value).ToList();
                }

                if (!checkoutableBookings.Any())
                {
                    TempData["Warning"] = bookingId.HasValue ? 
                        "The selected booking is not available for checkout." : 
                        "No bookings available for checkout.";
                    return RedirectToAction("Details", new { customerId = customerId });
                }                // Calculate dynamic checkout amounts using the calculation service
                var actualCheckoutDate = DateTime.Now;
                var dynamicCheckout = await _checkoutCalculationService.CalculateDynamicCheckoutAsync(checkoutableBookings, customer, actualCheckoutDate);
                
                var checkoutViewModel = new CheckoutViewModel
                {
                    Customer = customer,
                    BookingsForCheckout = checkoutableBookings,
                    TotalAmountPaid = checkoutableBookings.Sum(b => b.AmountPaid),
                    TotalAmount = checkoutableBookings.Sum(b => b.TotalAmount ?? 0),
                    
                    // Dynamic checkout calculation properties
                    ActualCheckoutDate = dynamicCheckout.ActualCheckoutDate,
                    BookingCalculations = dynamicCheckout.BookingCalculations,
                    RecalculatedTotalAmount = dynamicCheckout.RecalculatedTotalAmount,
                    FinalAmountDue = dynamicCheckout.FinalAmountDue,
                    HasEarlyCheckouts = dynamicCheckout.HasEarlyCheckouts,
                    HasTBDBookings = dynamicCheckout.HasTBDBookings
                };

                return View(checkoutViewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading checkout page: {ex.Message}";
                return RedirectToAction("Details", new { customerId = customerId });
            }
        }        /// <summary>
        /// Process checkout with full payment settlement using dynamic calculations
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessCheckout(CheckoutPaymentRequest request)
        {
            try
            {
                var bookingIdsList = request?.GetBookingIdsList() ?? new List<int>();
                if (!bookingIdsList.Any())
                {
                    TempData["Error"] = "No bookings selected for checkout.";
                    return RedirectToAction("Details", new { customerId = request?.CustomerId });
                }

                // Get customer details to find the bookings
                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                var customersWithSameId = allCustomers.Where(c => c.CustomerId == request.CustomerId).ToList();

                var allRoomBookings = new List<RoomBooking>();
                foreach (var c in customersWithSameId)
                {
                    if (c.RoomBookings != null)
                        allRoomBookings.AddRange(c.RoomBookings);
                }

                var bookingsToCheckout = allRoomBookings.Where(rb => bookingIdsList.Contains(rb.BookingId)).ToList();
                
                if (!bookingsToCheckout.Any())
                {
                    TempData["Error"] = "Selected bookings not found.";
                    return RedirectToAction("Details", new { customerId = request.CustomerId });
                }                // Calculate dynamic checkout amounts for accurate payment processing
                var customer = customersWithSameId.First();
                var currentCheckoutDate = DateTime.Now;
                var dynamicCheckout = await _checkoutCalculationService.CalculateDynamicCheckoutAsync(bookingsToCheckout, customer, currentCheckoutDate);
                var checkoutSummary = _checkoutCalculationService.GetCheckoutSummary(dynamicCheckout);

                var httpClient = _httpClientFactory.CreateClient("API");
                var successfulCheckouts = 0;
                var errors = new List<string>();

                foreach (var booking in bookingsToCheckout)
                {                    try
                    {
                        // Find the dynamic calculation for this booking to get accurate amounts
                        var calculation = dynamicCheckout.BookingCalculations.FirstOrDefault(bc => bc.BookingId == booking.BookingId);
                        var recalculatedAmount = calculation?.RecalculatedAmount ?? (booking.TotalAmount ?? booking.AmountPaid);
                        var bookingCheckoutDate = calculation?.ActualCheckOutDate ?? DateTime.Now;                        // For checkout, customer pays the full recalculated amount
                        // Update total amount to reflect actual nights stayed
                        var finalTotalAmount = recalculatedAmount;
                        
                        // Update amount paid to reflect full payment (total settlement)
                        var finalAmountPaid = finalTotalAmount; // Full payment - customer has settled everything// Prepare the booking data for update - checkout with recalculated amounts
                        // Use the correct structure that matches RoomBookingDtoForUpdation
                        var updateData = new
                        {
                            RoomId = booking.RoomId,
                            CheckInDate = booking.CheckInDate,                            CheckOutDate = bookingCheckoutDate, // Use actual calculated checkout date
                            TotalAmount = finalTotalAmount, // Use dynamically calculated amount based on actual nights
                            AmountPaid = finalAmountPaid, // Full payment amount - customer has settled everything
                            PendingAmount = Math.Max(0, finalTotalAmount - finalAmountPaid), // Should be 0 for checked out bookings
                            BookingStatus = "Checked Out",
                            NumberOfGuests = booking.NumberOfGuests
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(updateData);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PutAsync($"api/roombookings/{booking.BookingId}", content);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            successfulCheckouts++;
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            errors.Add($"Booking {booking.BookingId}: {response.StatusCode} - {errorContent}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Booking {booking.BookingId}: {ex.Message}");
                    }
                }

                // Generate enhanced success message with dynamic calculation info
                if (successfulCheckouts > 0)
                {
                    var message = $"Checkout completed successfully! ";                    if (checkoutSummary.HasRefunds)
                    {
                        message += $"Total refund: ₹{checkoutSummary.TotalRefundAmount:N2}. ";
                    }
                    else if (checkoutSummary.HasAdditionalCharges)
                    {
                        message += $"Additional charge: ₹{checkoutSummary.TotalAdditionalCharges:N2}. ";
                    }
                    else
                    {
                        message += $"Final amount: ₹{dynamicCheckout.FinalAmountDue:N2}. ";
                    }
                    
                    message += $"{successfulCheckouts} booking(s) checked out.";
                    
                    if (dynamicCheckout.HasEarlyCheckouts)
                    {
                        message += " Early checkout amounts have been recalculated.";
                    }
                    
                    if (dynamicCheckout.HasTBDBookings)
                    {
                        message += " TBD booking amounts have been finalized.";
                    }
                    
                    if (errors.Any())
                    {
                        message += $" {errors.Count} update(s) failed: {string.Join(", ", errors)}";
                    }
                    
                    TempData["Success"] = message;
                }
                else
                {
                    TempData["Error"] = $"Failed to process checkout: {string.Join(", ", errors)}";
                }

                return RedirectToAction("Details", new { customerId = request.CustomerId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error during checkout processing: {ex.Message}";
                return RedirectToAction("Details", new { customerId = request?.CustomerId });
            }
        }

        /// <summary>
        /// Automatic checkout process for bookings that have reached their checkout date
        /// This can be called as a background service or scheduled task
        /// </summary>
        public async Task<IActionResult> ProcessAutoCheckout()
        {
            try
            {
                var currentDateTime = DateTime.Now;
                var allCustomers = await _serviceManager.CustomerService.ViewAllCustomers();
                
                var autoCheckoutBookings = new List<RoomBooking>();
                
                // Find all bookings that need auto-checkout
                foreach (var customer in allCustomers)
                {
                    if (customer.RoomBookings != null)
                    {                        var eligibleBookings = customer.RoomBookings.Where(rb =>
                            rb.CheckOutDate.HasValue &&
                            rb.CheckOutDate.Value <= currentDateTime &&
                            (string.Equals(rb.BookingStatus, "Checked In", StringComparison.OrdinalIgnoreCase) ||
                             string.Equals(rb.BookingStatus, "Checked-In", StringComparison.OrdinalIgnoreCase))
                        ).ToList();
                        
                        autoCheckoutBookings.AddRange(eligibleBookings);
                    }
                }

                if (!autoCheckoutBookings.Any())
                {
                    return Json(new { success = true, message = "No bookings require auto-checkout at this time." });
                }

                var httpClient = _httpClientFactory.CreateClient("API");
                var successfulAutoCheckouts = 0;
                var errors = new List<string>();

                foreach (var booking in autoCheckoutBookings)
                {
                    try
                    {
                        // Find the customer for this booking
                        var bookingCustomer = allCustomers.FirstOrDefault(c => 
                            c.RoomBookings?.Any(rb => rb.BookingId == booking.BookingId) == true);
                        
                        if (bookingCustomer == null) continue;                        var updateData = new
                        {
                            RoomId = booking.RoomId,
                            CheckInDate = booking.CheckInDate,
                            CheckOutDate = currentDateTime, // Set to current time
                            TotalAmount = booking.TotalAmount,
                            AmountPaid = booking.TotalAmount ?? 0, // Full payment - customer has settled everything
                            PendingAmount = 0, // No pending amount for checked out bookings
                            BookingStatus = "Checked Out",
                            NumberOfGuests = booking.NumberOfGuests
                        };

                        var json = System.Text.Json.JsonSerializer.Serialize(updateData);
                        var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");

                        var response = await httpClient.PutAsync($"api/roombookings/{booking.BookingId}", content);
                        
                        if (response.IsSuccessStatusCode)
                        {
                            successfulAutoCheckouts++;
                        }
                        else
                        {
                            var errorContent = await response.Content.ReadAsStringAsync();
                            errors.Add($"Booking {booking.BookingId}: {response.StatusCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Add($"Booking {booking.BookingId}: {ex.Message}");
                    }
                }

                var message = $"Auto-checkout completed. {successfulAutoCheckouts} booking(s) checked out successfully.";
                if (errors.Any())
                {
                    message += $" {errors.Count} failed.";
                }

                return Json(new { 
                    success = true, 
                    message = message,
                    processedCount = autoCheckoutBookings.Count,
                    successCount = successfulAutoCheckouts,
                    errorCount = errors.Count
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error during auto-checkout: {ex.Message}" });
            }
        }        /// <summary>
        /// Calculate customer status based on comprehensive business rules:
        /// Uses the BookingStatusBadge utility for consistent status determination
        /// </summary>
        /// <param name="customer">Customer to calculate status for</param>
        /// <returns>Customer status string</returns>
        private string GetCustomerStatus(Customer customer)
        {
            if (customer?.RoomBookings == null || !customer.RoomBookings.Any())
            {
                return BookingStatusBadge.CustomerStatus.NoBookings;
            }

            // Convert room bookings to dynamic objects for the utility method
            var roomBookings = customer.RoomBookings.Select(rb => new
            {
                BookingStatus = rb.BookingStatus,
                CheckInDate = rb.CheckInDate,
                CheckOutDate = rb.CheckOutDate
            }).Cast<dynamic>().ToList();        // Use the centralized status calculation logic
            return BookingStatusBadge.CalculateCustomerStatus(roomBookings, customer.CabBookings?.Cast<dynamic>());
        }

        /// <summary>
        /// Cancels the customer creation form and clears any pending data
        /// </summary>
        [HttpGet]
        public IActionResult Cancel()
        {
            // Clear any pending customer data from TempData
            TempData.Remove("PendingCustomerModel");
            
            // Clear session tracking data
            var currentSessionId = HttpContext.Session.Id;
            var sessionKeyForData = $"PendingCustomerModel_{currentSessionId}";
            HttpContext.Session.Remove(sessionKeyForData);
            
            TempData["Info"] = "Customer creation cancelled. Form data has been cleared.";
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Cancels a confirmed booking
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelBooking(int bookingId, string customerId)
        {
            try
            {
                if (bookingId <= 0)
                {
                    TempData["Error"] = "Invalid booking ID.";
                    return RedirectToAction("Details", new { customerId });
                }

                var success = await _serviceManager.RoomBookingService.CancelBookingAsync(bookingId);
                
                if (success)
                {
                    TempData["Success"] = "Booking cancelled successfully.";
                }
                else
                {
                    TempData["Error"] = "Failed to cancel booking. Only confirmed bookings can be cancelled.";
                }
                
                return RedirectToAction("Details", new { customerId });
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error cancelling booking: {ex.Message}";
                return RedirectToAction("Details", new { customerId });
            }
        }

        /// <summary>
        /// Determines the sorting priority for customers based on their status
        /// Lower numbers have higher priority (appear first in the list)
        /// </summary>
        /// <param name="customer">Customer to evaluate</param>
        /// <returns>Priority number for sorting</returns>
        private int GetCustomerPriority(Customer customer)
        {
            var status = GetCustomerStatus(customer);
            
            return status switch
            {
                BookingStatusBadge.CustomerStatus.CheckedIn => 1,      // Highest priority - currently checked in
                BookingStatusBadge.CustomerStatus.ActiveBooking => 2,  // Second priority - confirmed future bookings
                BookingStatusBadge.CustomerStatus.RecentGuest => 3,    // Third priority - recent guests
                BookingStatusBadge.CustomerStatus.PendingPayment => 4, // Fourth priority - payment issues
                _ => 5  // Lowest priority - no bookings or other statuses
            };
        }
    }
}
