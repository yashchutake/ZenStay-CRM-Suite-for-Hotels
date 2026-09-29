using AutoMapper;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;
using ZenHotelManagement.Entities.Exceptions;
using EmployeePortalWebApi.Entities.Exceptions;

namespace ZenHotelManagement.Service
{
    public class RoomBookingService : IRoomBookingService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public RoomBookingService(IRepositoryManager repository,  IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public IEnumerable<RoomBookingDto> GetAllBookings(bool trackChanges)
        {
            var bookings = _repository.RoomBooking.GetAllBookings(trackChanges);
            var bookingDtos = _mapper.Map<IEnumerable<RoomBookingDto>>(bookings);
            return bookingDtos;
        }

        public RoomBookingDto GetBookingById(int bookingId, bool trackChanges)
        {
            var booking = _repository.RoomBooking.GetBookingById(bookingId, trackChanges);
            if (booking == null)
                throw new RoomBookingNotFoundException(bookingId);

            var bookingDto = _mapper.Map<RoomBookingDto>(booking);
            return bookingDto;
        }        public RoomBookingDto CreateBooking(RoomBookingDtoForCreation booking)
        {
            var bookingEntity = _mapper.Map<RoomBooking>(booking);
            
            // Ensure PendingAmount is calculated based on booking status
            if (bookingEntity.TotalAmount.HasValue)
            {
                // If the booking is checked out, completed, or cancelled, pending amount should be 0
                if (string.Equals(bookingEntity.BookingStatus, "Checked Out", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(bookingEntity.BookingStatus, "Checked-Out", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(bookingEntity.BookingStatus, "Completed", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(bookingEntity.BookingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    bookingEntity.PendingAmount = 0;
                }
                else
                {
                    bookingEntity.PendingAmount = Math.Max(0, bookingEntity.TotalAmount.Value - bookingEntity.AmountPaid);
                }
            }
            
            _repository.RoomBooking.CreateBooking(bookingEntity);

            // Update room status ONLY for checked-in bookings, not confirmed - BEFORE saving
            if (string.Equals(bookingEntity.BookingStatus, "Checked In", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(bookingEntity.BookingStatus, "Checked-In", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(bookingEntity.BookingStatus, "Cheked-In", StringComparison.OrdinalIgnoreCase))
            {
                UpdateRoomStatusForBooking(bookingEntity.RoomId, false, true); // Occupied, Clean
            }
            
            _repository.Save();

            var bookingToReturn = _mapper.Map<RoomBookingDto>(bookingEntity);
            return bookingToReturn;
        }

        public IEnumerable<RoomBookingDto> GetBookingsByCustomerId(int customerId, bool trackChanges)
        {
            var bookings = _repository.RoomBooking.GetBookingsByCustomerId(customerId, trackChanges);
            var bookingDtos = _mapper.Map<IEnumerable<RoomBookingDto>>(bookings);
            return bookingDtos;
        }

        public IEnumerable<RoomBookingDto> GetActiveBookings(bool trackChanges)
        {
            var bookings = _repository.RoomBooking.GetActiveBookings(trackChanges);
            var bookingDtos = _mapper.Map<IEnumerable<RoomBookingDto>>(bookings);
            return bookingDtos;
        }        public void UpdateBooking(int bookingId, RoomBookingDtoForUpdation bookingDto, bool trackChanges)
        {
            var bookingEntity = _repository.RoomBooking.GetBookingById(bookingId, trackChanges);
            if (bookingEntity == null)
                throw new RoomBookingNotFoundException(bookingId);            // Get the old status to compare
            string oldStatus = bookingEntity.BookingStatus;
            
            _mapper.Map(bookingDto, bookingEntity);
            
            // Ensure PendingAmount is calculated based on booking status
            if (bookingEntity.TotalAmount.HasValue)
            {
                // If the booking is checked out, completed, or cancelled, pending amount should be 0
                if (string.Equals(bookingEntity.BookingStatus, "Checked Out", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(bookingEntity.BookingStatus, "Checked-Out", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(bookingEntity.BookingStatus, "Completed", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(bookingEntity.BookingStatus, "Cancelled", StringComparison.OrdinalIgnoreCase))
                {
                    bookingEntity.PendingAmount = 0;
                }
                else
                {
                    bookingEntity.PendingAmount = Math.Max(0, bookingEntity.TotalAmount.Value - bookingEntity.AmountPaid);
                }
            }
            
            // Handle room status changes based on booking status transitions BEFORE saving
            HandleRoomStatusForBookingUpdate(bookingEntity, oldStatus, bookingEntity.BookingStatus);
            
            _repository.Save();
        }/// <summary>
        /// Updates room status based on booking status changes
        /// </summary>
        private void HandleRoomStatusForBookingUpdate(RoomBooking booking, string oldStatus, string newStatus)
        {
            // When a customer checks in (Confirmed -> Checked In or any status -> Checked In)
            if (string.Equals(newStatus, "Checked In", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(newStatus, "Checked-In", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(newStatus, "Cheked-In", StringComparison.OrdinalIgnoreCase))
            {
                // Mark room as occupied and clean (initial check-in)
                UpdateRoomStatusForBooking(booking.RoomId, false, true);
            }
            // When a customer checks out (Checked In -> Checked Out)
            else if (string.Equals(newStatus, "Checked Out", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(newStatus, "Checked-Out", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(newStatus, "Completed", StringComparison.OrdinalIgnoreCase))
            {
                // Mark room as occupied and dirty (needs cleaning after checkout)
                UpdateRoomStatusForBooking(booking.RoomId, false, false);
            }
            // Note: For Confirmed bookings, we do NOT change room status
        }        /// <summary>
        /// Updates the room status and cleanliness
        /// </summary>
        private void UpdateRoomStatusForBooking(int roomId, bool roomStatus, bool isClean)
        {
            // Find room by ID with change tracking enabled for proper entity updates
            var room = _repository.Room.GetRoomById(roomId, true);
            if (room != null)
            {
                room.RoomStatus = roomStatus;
                room.IsClean = isClean;
                // Note: Save() will be called by the parent method after this call
            }
        }
          public void UpdateRoomAvailabilityBasedOnCurrentTime()
        {
            var currentTime = DateTime.Now;
            
            // Get all bookings that should be active right now
            // A room should be occupied if:
            // 1. Check-in time has passed AND check-out time hasn't passed
            // 2. Booking is either "Checked In" OR is "Confirmed" but check-in time has passed (auto check-in)
            var currentlyActiveBookings = _repository.RoomBooking.GetAllBookings(trackChanges: false)
                .Where(booking => booking.CheckInDate <= currentTime && 
                                (booking.CheckOutDate == null || booking.CheckOutDate > currentTime) &&
                                (booking.BookingStatus.Equals("Checked In", StringComparison.OrdinalIgnoreCase) ||
                                 booking.BookingStatus.Equals("Checked-In", StringComparison.OrdinalIgnoreCase) ||
                                 booking.BookingStatus.Equals("Cheked-In", StringComparison.OrdinalIgnoreCase) ||
                                 // Auto check-in: Confirmed bookings past their check-in time
                                 (booking.BookingStatus.Equals("Confirmed", StringComparison.OrdinalIgnoreCase) && 
                                  booking.CheckInDate <= currentTime)))
                .Select(b => b.RoomId)
                .ToHashSet();
            
            // Auto check-in: Update booking status from "Confirmed" to "Checked In" for bookings past check-in time
            var bookingsToAutoCheckIn = _repository.RoomBooking.GetAllBookings(trackChanges: true)
                .Where(booking => booking.CheckInDate <= currentTime &&
                                booking.BookingStatus.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
                .ToList();
            
            bool hasBookingChanges = false;
            foreach (var booking in bookingsToAutoCheckIn)
            {
                booking.BookingStatus = "Checked In";
                hasBookingChanges = true;
            }
            
            // Get all rooms and update their status based on real-time data
            var allRooms = _repository.Room.GetAllRooms(trackChanges: true);
            bool hasRoomChanges = false;            foreach (var room in allRooms)
            {
                bool shouldBeOccupied = currentlyActiveBookings.Contains(room.RoomId);
                
                // Update room availability if it doesn't match current reality
                // room.RoomStatus: true = available, false = occupied
                // shouldBeOccupied: true = should be occupied, false = should be available
                bool roomShouldBeAvailable = !shouldBeOccupied;
                
                if (room.RoomStatus != roomShouldBeAvailable) // If current status doesn't match what it should be
                {
                    room.RoomStatus = roomShouldBeAvailable; // Set to correct status
                    hasRoomChanges = true;
                }
            }
            
            if (hasBookingChanges || hasRoomChanges)
            {
                _repository.Save();
            }
        }

        public void CompleteExpiredRoomBookings()
        {
            var currentTime = DateTime.Now;
            
            // Get all active bookings that have passed their check-out time
            var expiredBookings = _repository.RoomBooking.GetAllBookings(trackChanges: true)
                .Where(booking => booking.CheckOutDate.HasValue && 
                                booking.CheckOutDate <= currentTime &&
                                !booking.BookingStatus.Equals("Checked Out", StringComparison.OrdinalIgnoreCase) &&
                                !booking.BookingStatus.Equals("Completed", StringComparison.OrdinalIgnoreCase) &&
                                !booking.BookingStatus.Equals("Cancelled", StringComparison.OrdinalIgnoreCase));
            
            foreach (var booking in expiredBookings)
            {
                // Mark booking as completed
                booking.BookingStatus = "Completed";
                
                // Mark room as dirty (needs cleaning after checkout)
                var room = _repository.Room.GetRoomById(booking.RoomId, trackChanges: true);
                if (room != null)
                {
                    room.RoomStatus = false; // occupied (dirty, needs cleaning)
                    room.IsClean = false;
                }
            }
            
            if (expiredBookings.Any())
            {
                _repository.Save();
            }
        }

        /// <summary>
        /// Cancels a confirmed booking and ensures the room is properly freed
        /// </summary>
        /// <param name="bookingId">The booking ID to cancel</param>
        /// <param name="trackChanges">Whether to track changes</param>
        /// <exception cref="RoomBookingNotFoundException">If booking not found</exception>
        /// <exception cref="InvalidOperationException">If booking cannot be cancelled</exception>
        public void CancelBooking(int bookingId, bool trackChanges)
        {
            var bookingEntity = _repository.RoomBooking.GetBookingById(bookingId, trackChanges);
            if (bookingEntity == null)
                throw new RoomBookingNotFoundException(bookingId);

            // Only allow cancellation of confirmed bookings
            if (!string.Equals(bookingEntity.BookingStatus, "Confirmed", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException($"Cannot cancel booking with status '{bookingEntity.BookingStatus}'. Only 'Confirmed' bookings can be cancelled.");
            }

            // Update booking status to cancelled
            bookingEntity.BookingStatus = "Cancelled";

            // Ensure the room is available when booking is cancelled
            // This is important because cancelled bookings should not affect room occupancy
            var room = _repository.Room.GetRoomById(bookingEntity.RoomId, trackChanges: true);
            if (room != null)
            {
                // Mark room as available and clean (ready for next booking)
                room.RoomStatus = true; // Available
                room.IsClean = true;    // Clean
            }

            // Additionally, cancel any associated cab bookings for the same customer within the room booking period
            var serviceManager = new ServiceManager(_repository, _mapper);
            try
            {
                serviceManager.CabBookingService.CancelCabBookingsByCustomerId(
                    bookingEntity.CustomerId, 
                    bookingEntity.CheckInDate,
                    bookingEntity.CheckOutDate ?? bookingEntity.CheckInDate.AddDays(1),
                    true);
            }
            catch (Exception ex)
            {
                // Log the error but don't stop the room cancellation process
                // This ensures that if something goes wrong with cab cancellation, 
                // the room booking is still properly cancelled
                Console.WriteLine($"Error cancelling associated cab bookings: {ex.Message}");
            }

            _repository.Save();
        }
    }
}
