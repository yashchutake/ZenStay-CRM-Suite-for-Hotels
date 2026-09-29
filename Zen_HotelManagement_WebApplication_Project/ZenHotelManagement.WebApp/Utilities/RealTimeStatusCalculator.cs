using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Utilities;

namespace ZenHotelManagement.WebApp.Utilities
{
    /// <summary>
    /// Provides real-time status calculation with minute-level precision for room bookings,
    /// cab bookings, and cab drivers without requiring database polling.
    /// All status calculations are performed dynamically when needed.
    /// </summary>
    public static class RealTimeStatusCalculator
    {
        #region Room Booking Status Calculation

        /// <summary>
        /// Calculates the real-time status of a room booking based on current time with minute precision
        /// </summary>
        /// <param name="booking">The room booking to calculate status for</param>
        /// <param name="currentTime">Current time (optional, defaults to DateTime.Now)</param>
        /// <returns>Real-time booking status</returns>
        public static string CalculateRoomBookingStatus(RoomBooking booking, DateTime? currentTime = null)
        {
            if (booking == null)
                return BookingStatusBadge.RoomBookingStatus.Cancelled;

            var now = currentTime ?? DateTime.Now;

            // Don't override manually cancelled bookings
            if (string.Equals(booking.BookingStatus, BookingStatusBadge.RoomBookingStatus.Cancelled, 
                              StringComparison.OrdinalIgnoreCase))
            {
                return BookingStatusBadge.RoomBookingStatus.Cancelled;
            }

            // Don't override manually set No Show status
            if (string.Equals(booking.BookingStatus, BookingStatusBadge.RoomBookingStatus.NoShow, 
                              StringComparison.OrdinalIgnoreCase))
            {
                return BookingStatusBadge.RoomBookingStatus.NoShow;
            }

            // Check-out time has passed (minute precision) - mark as checked out
            if (booking.CheckOutDate.HasValue && booking.CheckOutDate.Value <= now)
            {
                return BookingStatusBadge.RoomBookingStatus.CheckedOut;
            }

            // Check-in time has arrived/passed (minute precision) and booking is confirmed - mark as checked in
            if (booking.CheckInDate <= now && 
                string.Equals(booking.BookingStatus, BookingStatusBadge.RoomBookingStatus.Confirmed, 
                              StringComparison.OrdinalIgnoreCase))
            {
                return BookingStatusBadge.RoomBookingStatus.CheckedIn;
            }

            // If already checked in and check-out time hasn't passed, keep checked in status
            if (IsCheckedInStatus(booking.BookingStatus) && 
                (!booking.CheckOutDate.HasValue || booking.CheckOutDate.Value > now))
            {
                return BookingStatusBadge.RoomBookingStatus.CheckedIn;
            }

            // Default to the current booking status if none of the time-based conditions apply
            return booking.BookingStatus ?? BookingStatusBadge.RoomBookingStatus.Confirmed;
        }

        /// <summary>
        /// Applies real-time status calculation to a room booking object
        /// </summary>
        /// <param name="booking">The booking to update</param>
        /// <param name="currentTime">Current time (optional)</param>
        public static void ApplyRealTimeStatusToRoomBooking(RoomBooking booking, DateTime? currentTime = null)
        {
            if (booking != null)
            {
                booking.BookingStatus = CalculateRoomBookingStatus(booking, currentTime);
            }
        }

        /// <summary>
        /// Applies real-time status calculation to a collection of room bookings
        /// </summary>
        /// <param name="bookings">The bookings to update</param>
        /// <param name="currentTime">Current time (optional)</param>
        public static void ApplyRealTimeStatusToRoomBookings(IEnumerable<RoomBooking> bookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;
            foreach (var booking in bookings ?? Enumerable.Empty<RoomBooking>())
            {
                ApplyRealTimeStatusToRoomBooking(booking, now);
            }
        }

        #endregion

        #region Cab Booking Status Calculation

        /// <summary>
        /// Calculates the real-time status of a cab booking based on current time with minute precision
        /// </summary>
        /// <param name="booking">The cab booking to calculate status for</param>
        /// <param name="currentTime">Current time (optional, defaults to DateTime.Now)</param>
        /// <returns>Real-time cab booking status</returns>
        public static bool CalculateCabBookingCompletionStatus(CabBooking booking, DateTime? currentTime = null)
        {
            if (booking == null)
                return true; // Consider null bookings as completed

            var now = currentTime ?? DateTime.Now;

            // If manually marked as completed, respect that
            if (booking.IsRideCompleted == true)
                return true;

            // Auto-complete if drop-off time has passed (minute precision)
            if (booking.DropOffDateTime <= now)
                return true;

            // Otherwise, not completed yet
            return false;
        }

        /// <summary>
        /// Gets the display status for a cab booking
        /// </summary>
        /// <param name="booking">The cab booking</param>
        /// <param name="currentTime">Current time (optional)</param>
        /// <returns>Status string for display</returns>
        public static string CalculateCabBookingDisplayStatus(CabBooking booking, DateTime? currentTime = null)
        {
            if (booking == null)
                return BookingStatusBadge.CabBookingStatus.Cancelled;

            var now = currentTime ?? DateTime.Now;
            var isCompleted = CalculateCabBookingCompletionStatus(booking, now);

            if (isCompleted)
                return BookingStatusBadge.CabBookingStatus.Completed;

            // Check if ride is currently in progress (pickup time passed, drop-off time not passed)
            if (booking.PickUpDateTime <= now && booking.DropOffDateTime > now)
                return BookingStatusBadge.CabBookingStatus.InProgress;

            // Otherwise, it's confirmed but not started yet
            return BookingStatusBadge.CabBookingStatus.Confirmed;
        }

        /// <summary>
        /// Applies real-time status calculation to a cab booking object
        /// </summary>
        /// <param name="booking">The booking to update</param>
        /// <param name="currentTime">Current time (optional)</param>
        public static void ApplyRealTimeStatusToCabBooking(CabBooking booking, DateTime? currentTime = null)
        {
            if (booking != null)
            {
                booking.IsRideCompleted = CalculateCabBookingCompletionStatus(booking, currentTime);
            }
        }

        /// <summary>
        /// Applies real-time status calculation to a collection of cab bookings
        /// </summary>
        /// <param name="bookings">The bookings to update</param>
        /// <param name="currentTime">Current time (optional)</param>
        public static void ApplyRealTimeStatusToCabBookings(IEnumerable<CabBooking> bookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;
            foreach (var booking in bookings ?? Enumerable.Empty<CabBooking>())
            {
                ApplyRealTimeStatusToCabBooking(booking, now);
            }
        }

        #endregion

        #region Cab Driver Status Calculation

        /// <summary>
        /// Calculates real-time availability status for a cab driver based on their active bookings
        /// </summary>
        /// <param name="driverId">The driver ID</param>
        /// <param name="allCabBookings">All cab bookings to check against</param>
        /// <param name="currentTime">Current time (optional)</param>
        /// <returns>True if available, false if on duty</returns>
        public static bool CalculateCabDriverAvailability(int driverId, IEnumerable<CabBooking> allCabBookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;

            // Check if driver has any active bookings right now
            var hasActiveBooking = allCabBookings?
                .Where(booking => booking.CabDriverId == driverId)
                .Any(booking => 
                    booking.PickUpDateTime <= now && 
                    booking.DropOffDateTime > now && 
                    !CalculateCabBookingCompletionStatus(booking, now)) ?? false;

            // Available if no active bookings, On Duty if has active bookings
            return !hasActiveBooking;
        }

        /// <summary>
        /// Gets the display status for a cab driver
        /// </summary>
        /// <param name="isAvailable">Whether the driver is available</param>
        /// <returns>Status string for display</returns>
        public static string GetCabDriverDisplayStatus(bool isAvailable)
        {
            return isAvailable ? "Available" : "On Duty";
        }

        /// <summary>
        /// Applies real-time availability calculation to a cab driver
        /// </summary>
        /// <param name="driver">The driver to update</param>
        /// <param name="allCabBookings">All cab bookings to check against</param>
        /// <param name="currentTime">Current time (optional)</param>
        public static void ApplyRealTimeStatusToCabDriver(CabDriver driver, IEnumerable<CabBooking> allCabBookings, DateTime? currentTime = null)
        {
            if (driver != null && !driver.IsDeleted)
            {
                driver.IsAvailable = CalculateCabDriverAvailability(driver.CabDriverId, allCabBookings, currentTime);
            }
        }

        /// <summary>
        /// Applies real-time availability calculation to a collection of cab drivers
        /// </summary>
        /// <param name="drivers">The drivers to update</param>
        /// <param name="allCabBookings">All cab bookings to check against</param>
        /// <param name="currentTime">Current time (optional)</param>
        public static void ApplyRealTimeStatusToCabDrivers(IEnumerable<CabDriver> drivers, IEnumerable<CabBooking> allCabBookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;
            foreach (var driver in drivers ?? Enumerable.Empty<CabDriver>())
            {
                ApplyRealTimeStatusToCabDriver(driver, allCabBookings, now);
            }
        }

        #endregion

        #region Customer Status Calculation

        /// <summary>
        /// Calculates the real-time primary status for a customer based on their bookings
        /// </summary>
        /// <param name="customer">The customer</param>
        /// <param name="currentTime">Current time (optional)</param>
        /// <returns>Primary customer status</returns>
        public static string CalculateCustomerStatus(Customer customer, DateTime? currentTime = null)
        {
            if (customer?.RoomBookings == null || !customer.RoomBookings.Any())
            {
                return BookingStatusBadge.CustomerStatus.NoBookings;
            }

            var now = currentTime ?? DateTime.Now;

            // First apply real-time status to all room bookings
            ApplyRealTimeStatusToRoomBookings(customer.RoomBookings, now);

            // Convert to dynamic objects for the existing utility method
            var roomBookings = customer.RoomBookings.Select(rb => new
            {
                BookingStatus = rb.BookingStatus,
                CheckInDate = rb.CheckInDate,
                CheckOutDate = rb.CheckOutDate
            }).Cast<dynamic>().ToList();

            // Use the existing centralized status calculation logic
            return BookingStatusBadge.CalculateCustomerStatus(roomBookings, customer.CabBookings?.Cast<dynamic>());
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Checks if a booking status represents a checked-in state
        /// </summary>
        /// <param name="status">The status to check</param>
        /// <returns>True if the status represents checked-in</returns>
        private static bool IsCheckedInStatus(string? status)
        {
            if (string.IsNullOrWhiteSpace(status))
                return false;

            return string.Equals(status, BookingStatusBadge.RoomBookingStatus.CheckedIn, StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, "Checked-In", StringComparison.OrdinalIgnoreCase) ||
                   string.Equals(status, "Cheked-In", StringComparison.OrdinalIgnoreCase); // Handle typo
        }

        /// <summary>
        /// Gets room occupancy status for a specific room based on its bookings
        /// </summary>
        /// <param name="roomId">The room ID</param>
        /// <param name="roomBookings">All room bookings</param>
        /// <param name="currentTime">Current time (optional)</param>
        /// <returns>True if room is occupied, false if available</returns>
        public static bool IsRoomOccupied(int roomId, IEnumerable<RoomBooking> roomBookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;
            
            return roomBookings?.Any(booking => 
                booking.RoomId == roomId &&
                booking.CheckInDate <= now &&
                (!booking.CheckOutDate.HasValue || booking.CheckOutDate.Value > now) &&
                IsCheckedInStatus(CalculateRoomBookingStatus(booking, now))) ?? false;
        }

        /// <summary>
        /// Gets all currently occupied room IDs
        /// </summary>
        /// <param name="allRoomBookings">All room bookings</param>
        /// <param name="currentTime">Current time (optional)</param>
        /// <returns>Set of occupied room IDs</returns>
        public static HashSet<int> GetOccupiedRoomIds(IEnumerable<RoomBooking> allRoomBookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;
            
            return allRoomBookings?
                .Where(booking => 
                    booking.CheckInDate <= now &&
                    (!booking.CheckOutDate.HasValue || booking.CheckOutDate.Value > now) &&
                    IsCheckedInStatus(CalculateRoomBookingStatus(booking, now)))
                .Select(booking => booking.RoomId)
                .ToHashSet() ?? new HashSet<int>();
        }

        /// <summary>
        /// Gets all currently busy cab driver IDs
        /// </summary>
        /// <param name="allCabBookings">All cab bookings</param>
        /// <param name="currentTime">Current time (optional)</param>
        /// <returns>Set of busy cab driver IDs</returns>
        public static HashSet<int> GetBusyCabDriverIds(IEnumerable<CabBooking> allCabBookings, DateTime? currentTime = null)
        {
            var now = currentTime ?? DateTime.Now;
            
            return allCabBookings?
                .Where(booking => 
                    booking.PickUpDateTime <= now &&
                    booking.DropOffDateTime > now &&
                    !CalculateCabBookingCompletionStatus(booking, now))
                .Select(booking => booking.CabDriverId)
                .ToHashSet() ?? new HashSet<int>();
        }

        #endregion
    }
}
