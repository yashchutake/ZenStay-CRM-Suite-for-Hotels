using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Models.ViewModel
{    /// <summary>
    /// View model for the checkout page with dynamic amount calculation
    /// </summary>
    public class CheckoutViewModel
    {
        public Customer Customer { get; set; } = new Customer();
        public List<RoomBooking> BookingsForCheckout { get; set; } = new List<RoomBooking>();
        public decimal TotalAmountPaid { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal RemainingAmount => Math.Max(0, TotalAmount - TotalAmountPaid);
        
        // Dynamic checkout calculation properties
        public DateTime ActualCheckoutDate { get; set; } = DateTime.Now;
        public List<CheckoutBookingCalculation> BookingCalculations { get; set; } = new List<CheckoutBookingCalculation>();
        public decimal RecalculatedTotalAmount { get; set; }
        public decimal FinalAmountDue { get; set; }
        public bool HasEarlyCheckouts { get; set; }
        public bool HasTBDBookings { get; set; }
    }
    
    /// <summary>
    /// Detailed calculation for each booking at checkout
    /// </summary>
    public class CheckoutBookingCalculation
    {
        public int BookingId { get; set; }
        public string RoomNo { get; set; } = string.Empty;
        public DateTime CheckInDate { get; set; }
        public DateTime? OriginalCheckOutDate { get; set; }
        public DateTime ActualCheckOutDate { get; set; }
        public decimal RoomCostPerNight { get; set; }
        public int OriginalNights { get; set; }
        public int ActualNights { get; set; }
        public decimal OriginalAmount { get; set; }
        public decimal RecalculatedAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal FinalAmountDue { get; set; }
        public bool IsEarlyCheckout { get; set; }
        public bool WasTBD { get; set; }
        public bool IsSameDayCheckout { get; set; }
        public string CheckoutType { get; set; } = string.Empty; // "Normal", "Early", "TBD", "Same-Day"
    }/// <summary>
    /// Request model for processing checkout with full payment settlement
    /// </summary>
    public class CheckoutPaymentRequest
    {
        public string CustomerId { get; set; } = string.Empty;
        public string BookingIds { get; set; } = string.Empty; // Changed to string to handle comma-separated values
        
        /// <summary>
        /// Gets the booking IDs as a list of integers
        /// </summary>
        public List<int> GetBookingIdsList()
        {
            if (string.IsNullOrEmpty(BookingIds))
                return new List<int>();
                
            return BookingIds.Split(',', StringSplitOptions.RemoveEmptyEntries)
                            .Select(id => int.TryParse(id.Trim(), out int result) ? result : 0)
                            .Where(id => id > 0)
                            .ToList();
        }
    }}
