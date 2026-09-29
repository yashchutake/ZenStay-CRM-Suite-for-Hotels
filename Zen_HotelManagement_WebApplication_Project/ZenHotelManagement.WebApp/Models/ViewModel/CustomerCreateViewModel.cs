using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace ZenHotelManagement.WebApp.Models.ViewModel
{    public class RoomBookingCreateViewModel : IValidatableObject
    {
        [Required(ErrorMessage = "Room selection is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid room")]
        [Display(Name = "Room")]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Check-in date is required")]
        [DataType(DataType.DateTime)]
        [Display(Name = "Check-in Date & Time")]
        public DateTime CheckInDate { get; set; } = DateTime.Now.AddDays(1);

        [DataType(DataType.DateTime)]
        [Display(Name = "Check-out Date & Time")]
        public DateTime? CheckOutDate { get; set; }

        [Display(Name = "Total Amount")]
        [Range(0, double.MaxValue, ErrorMessage = "Amount must be a positive value")]
        public decimal Amount { get; set; } = 0; // Will be set from room cost per night

        [Required(ErrorMessage = "Advance payment amount is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Advance payment must be a positive value")]
        [Display(Name = "Advance Payment")]
        public decimal AmountPaid { get; set; } = 0;

        [Required(ErrorMessage = "Booking date is required")]
        [DataType(DataType.DateTime)]
        public DateTime BookingDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Booking status is required")]
        [StringLength(50, ErrorMessage = "Booking status cannot exceed 50 characters")]
        public string BookingStatus { get; set; } = "Confirmed";

        [Required(ErrorMessage = "Number of guests is required")]
        [Range(1, 10, ErrorMessage = "Number of guests must be between 1 and 10")]
        [Display(Name = "Number of Guests")]
        public int NumberOfGuests { get; set; } = 1;

        // Helper properties for display
        public string? RoomNo { get; set; }
        public string? RoomType { get; set; }
        public decimal CostPerNight { get; set; }
        public bool IsAc { get; set; }
        public int RoomCapacity { get; set; }        // Method to calculate total amount based on room cost and nights
        public void CalculateTotalAmount()
        {
            // Calculate total amount based on CostPerNight and number of nights
            var numberOfNights = 1; // Default for tentative calculations
            if (CheckOutDate.HasValue)
            {
                var timeDiff = CheckOutDate.Value - CheckInDate;
                numberOfNights = Math.Max(1, (int)Math.Ceiling(timeDiff.TotalHours / 24.0)); // Minimum 1 night
            }

            Amount = CostPerNight * numberOfNights;
        }

        // Method to calculate cost with explicit room cost parameter
        public void CalculateTotalAmount(decimal roomCostPerNight)
        {
            CostPerNight = roomCostPerNight;
            CalculateTotalAmount();
        }        // Custom validation to ensure AmountPaid is reasonable
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Validate check-in date is not in the past
            if (CheckInDate < DateTime.Now.AddMinutes(-30)) // Allow 30 minutes buffer
            {
                yield return new ValidationResult(
                    "Check-in date cannot be in the past",
                    new[] { nameof(CheckInDate) });
            }

            // Validate check-out date is after check-in date
            if (CheckOutDate.HasValue && CheckOutDate.Value <= CheckInDate)
            {
                yield return new ValidationResult(
                    "Check-out date must be after check-in date",
                    new[] { nameof(CheckOutDate) });
            }

            // Validate that advance payment doesn't exceed one night's charge
            if (CostPerNight > 0 && AmountPaid > CostPerNight)
            {
                yield return new ValidationResult(
                    $"Advance payment cannot exceed one night's charge (₹{CostPerNight:N2})",
                    new[] { nameof(AmountPaid) });
            }

            // Ensure advance payment is not negative
            if (AmountPaid < 0)
            {
                yield return new ValidationResult(
                    "Advance payment cannot be negative",
                    new[] { nameof(AmountPaid) });
            }

            // Validate room selection
            if (RoomId <= 0)
            {
                yield return new ValidationResult(
                    "Please select a valid room",
                    new[] { nameof(RoomId) });
            }

            // Validate number of guests
            if (NumberOfGuests <= 0)
            {
                yield return new ValidationResult(
                    "Number of guests must be at least 1",
                    new[] { nameof(NumberOfGuests) });
            }
        }
    }    public class CustomerCreateViewModel
    {
        [Required(ErrorMessage = "Customer ID is required")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Customer ID must be between 5 and 50 characters")]
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "ID Type is required")]
        [Display(Name = "ID Type")]
        public string IdType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [RegularExpression(@"^[A-Za-z\s]+$", ErrorMessage = "Name should only contain letters and spaces")]
        [Display(Name = "Full Name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters")]
        [Display(Name = "Address")]
        public string? Address { get; set; }

        [StringLength(100, ErrorMessage = "Country name cannot exceed 100 characters")]
        [RegularExpression(@"^[A-Za-z\s]*$", ErrorMessage = "Country name should only contain letters and spaces")]
        [Display(Name = "Country")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [RegularExpression(@"^(\+91[\-\s]?)?[0]?(91)?[6789]\d{9}$", ErrorMessage = "Please enter a valid Indian mobile number")]
        [Display(Name = "Mobile Number")]
        public string MobileNo { get; set; } = string.Empty;

        // Room Bookings Collection
        public List<RoomBookingCreateViewModel> RoomBookings { get; set; } = new List<RoomBookingCreateViewModel>();

        // Option to use custom dates for additional rooms
        [Display(Name = "Use custom dates for additional rooms")]
        public bool UseCustomDatesForAdditionalRooms { get; set; } = false;// Available rooms for dropdown
        public List<Room> AvailableRooms { get; set; } = new List<Room>();

        // Available cab drivers for dropdown
        public List<CabDriver> AvailableCabDrivers { get; set; } = new List<CabDriver>();

        // Cab Booking Details (Optional)
        public bool IncludeCabBooking { get; set; } = false;

        public int? CabDriverId { get; set; }

        public DateTime? CabPickUpDateTime { get; set; }

        public DateTime? CabDropOffDateTime { get; set; }

        public string? CabPickUpLocation { get; set; }

        public string? CabDropOffLocation { get; set; }

        public decimal? CabFare { get; set; }

        // Helper methods to convert to domain models
        public Customer ToCustomer()
        {
            var customer = new Customer
            {
                CustomerId = this.CustomerId,
                IdType = this.IdType,
                Name = this.Name,
                Gender = this.Gender,
                Address = this.Address,
                Country = this.Country,
                MobileNo = this.MobileNo,
                RoomBookings = new List<RoomBooking>(),
                CabBookings = new List<CabBooking>()
            };            // Add room bookings
            foreach (var roomBooking in this.RoomBookings)
            {
                customer.RoomBookings.Add(new RoomBooking
                {
                    RoomId = roomBooking.RoomId,
                    CheckInDate = roomBooking.CheckInDate,
                    CheckOutDate = roomBooking.CheckOutDate,
                    TotalAmount = roomBooking.Amount,
                    AmountPaid = roomBooking.AmountPaid,
                    BookingDate = roomBooking.BookingDate,
                    BookingStatus = roomBooking.BookingStatus,
                    NumberOfGuests = roomBooking.NumberOfGuests
                });
            }            // Add cab booking if requested
            if (this.IncludeCabBooking && this.CabDriverId.HasValue)
            {
                customer.CabBookings.Add(new CabBooking
                {
                    CabDriverId = this.CabDriverId.Value,
                    PickUpDateTime = this.CabPickUpDateTime ?? DateTime.Now.AddHours(2),
                    DropOffDateTime = this.CabDropOffDateTime ?? DateTime.Now.AddHours(3),
                    PickUpLocation = this.CabPickUpLocation ?? "",
                    DropOffLocation = this.CabDropOffLocation ?? "",
                    Fare = this.CabFare ?? 0
                });
            }

            return customer;
        }
    }
    public class CustomerListViewModel
    {
        public IEnumerable<Customer> Customers { get; set; } = new List<Customer>();
    }
    public class BookingSummaryViewModel
    {
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string MobileNo { get; set; } = string.Empty; public List<RoomBookingSummary> RoomBookings { get; set; } = new List<RoomBookingSummary>();
        public decimal TotalRoomAmount { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public bool IncludeCabBooking { get; set; }
        public CabBookingSummary? CabBooking { get; set; }

        // Helper property to check if any room booking has TBD checkout
        public bool HasTentativeBookings => RoomBookings.Any(rb => !rb.CheckOutDate.HasValue);

        // Helper property to get grand total including cab fare
        public decimal GrandTotal => TotalRoomAmount + (CabBooking?.Fare ?? 0);

        // Helper property to get total amount at checkout including cab fare
        public decimal GrandTotalAtCheckout => (TotalRoomAmount - TotalAmountPaid) + (IncludeCabBooking && CabBooking != null ? CabBooking.Fare : 0);
    }
    public class RoomBookingSummary
    {
        public string RoomNo { get; set; } = string.Empty;
        public string RoomType { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime? CheckOutDate { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal Amount { get; set; }
        public decimal AmountPaid { get; set; }
        public string BookingStatus { get; set; } = string.Empty;
        // Property for number of nights with both calculated and manual override capability
        public int NumberOfNights { get; set; } = 1;
    }

    public class CabBookingSummary
    {
        public int CabDriverId { get; set; }
        public DateTime PickUpDateTime { get; set; }
        public DateTime DropOffDateTime { get; set; }
        public string PickUpLocation { get; set; } = string.Empty;
        public string DropOffLocation { get; set; } = string.Empty;
        public decimal Fare { get; set; }
    }
}
