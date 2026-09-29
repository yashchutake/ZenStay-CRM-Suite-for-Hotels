using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Models.ViewModel;
using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{
    /// <summary>
    /// Service for dynamic checkout amount calculations
    /// </summary>
    public class CheckoutCalculationService : ICheckoutCalculationService
    {
        private readonly IServiceManager _serviceManager;

        public CheckoutCalculationService(IServiceManager serviceManager)
        {
            _serviceManager = serviceManager;
        }

        /// <summary>
        /// Calculate dynamic checkout amounts based on actual checkout date
        /// </summary>
        public async Task<CheckoutViewModel> CalculateDynamicCheckoutAsync(List<RoomBooking> bookings, Customer customer, DateTime actualCheckoutDate)
        {
            var checkoutViewModel = new CheckoutViewModel
            {
                Customer = customer,
                BookingsForCheckout = bookings,
                ActualCheckoutDate = actualCheckoutDate,
                BookingCalculations = new List<CheckoutBookingCalculation>()
            };

            decimal totalRecalculatedAmount = 0;
            decimal totalAmountPaid = 0;
            bool hasEarlyCheckouts = false;
            bool hasTBDBookings = false;

            foreach (var booking in bookings)
            {
                var calculation = await CalculateBookingCheckoutAsync(booking, actualCheckoutDate);
                checkoutViewModel.BookingCalculations.Add(calculation);

                totalRecalculatedAmount += calculation.RecalculatedAmount;
                totalAmountPaid += calculation.AmountPaid;

                if (calculation.IsEarlyCheckout) hasEarlyCheckouts = true;
                if (calculation.WasTBD) hasTBDBookings = true;
            }

            // Set summary values
            checkoutViewModel.RecalculatedTotalAmount = totalRecalculatedAmount;
            checkoutViewModel.TotalAmountPaid = totalAmountPaid;
            checkoutViewModel.FinalAmountDue = Math.Max(0, totalRecalculatedAmount - totalAmountPaid);
            checkoutViewModel.HasEarlyCheckouts = hasEarlyCheckouts;
            checkoutViewModel.HasTBDBookings = hasTBDBookings;

            // Keep original amounts for comparison
            checkoutViewModel.TotalAmount = bookings.Sum(b => b.TotalAmount ?? b.AmountPaid);

            return checkoutViewModel;
        }

        /// <summary>
        /// Calculate checkout amounts for a single booking
        /// </summary>
        public async Task<CheckoutBookingCalculation> CalculateBookingCheckoutAsync(RoomBooking booking, DateTime actualCheckoutDate)
        {
            // Get room details to get cost per night
            Room room;
            try
            {
                room = await _serviceManager.RoomService.GetRoomByIdAsync(booking.RoomId);
                if (room == null)
                {
                    throw new InvalidOperationException($"Room with ID {booking.RoomId} not found");
                }
            }            catch (Exception ex)
            {
                // Fallback: estimate cost per night based on total amount and original nights
                decimal estimatedCostPerNight;
                  if (booking.TotalAmount.HasValue && booking.CheckOutDate.HasValue)
                {
                    // Calculate original nights to get cost per night
                    var originalTimeDiff = booking.CheckOutDate.Value - booking.CheckInDate;
                    var originalNights = Math.Max(1, (int)Math.Ceiling(originalTimeDiff.TotalHours / 24.0));
                    estimatedCostPerNight = booking.TotalAmount.Value / originalNights;
                }
                else
                {
                    // Ultimate fallback: use amount paid as estimate for cost per night
                    estimatedCostPerNight = booking.AmountPaid;
                }
                
                room = new Room { RoomId = booking.RoomId, RoomNo = $"Room {booking.RoomId}", CostPerNight = estimatedCostPerNight };
            }

            var calculation = new CheckoutBookingCalculation
            {
                BookingId = booking.BookingId,
                RoomNo = room.RoomNo ?? $"Room {booking.RoomId}",
                CheckInDate = booking.CheckInDate,
                OriginalCheckOutDate = booking.CheckOutDate,
                ActualCheckOutDate = actualCheckoutDate,
                RoomCostPerNight = room.CostPerNight,
                AmountPaid = booking.AmountPaid
            };            // Calculate original nights and amount
            if (booking.CheckOutDate.HasValue)
            {
                var originalTimeDiff = booking.CheckOutDate.Value - booking.CheckInDate;
                calculation.OriginalNights = Math.Max(1, (int)Math.Ceiling(originalTimeDiff.TotalHours / 24.0));
                calculation.WasTBD = false;
                calculation.CheckoutType = "Normal";
            }
            else
            {
                calculation.OriginalNights = 1; // TBD bookings default to 1 night estimate
                calculation.WasTBD = true;
                calculation.CheckoutType = "TBD";
            }            calculation.OriginalAmount = booking.TotalAmount ?? (room.CostPerNight * calculation.OriginalNights);            // Calculate actual time difference with minute precision
            var actualTimeDiff = actualCheckoutDate - booking.CheckInDate;
            
            // Calculate actual nights using precise time calculation
            // Always use minimum 1 night charge, but calculate based on actual time stayed
            double actualHours = actualTimeDiff.TotalHours;
            double actualDays = actualTimeDiff.TotalDays;
              // Always charge minimum 1 night regardless of stay duration
            // Even if customer stays for minutes, they pay for at least 1 full night
            // For longer stays, round up to nearest day (but minimum 1 night always applies)
            calculation.ActualNights = Math.Max(1, (int)Math.Ceiling(actualDays));

            // Determine checkout type with minute precision
            bool isEarlyCheckout = booking.CheckOutDate.HasValue && actualCheckoutDate < booking.CheckOutDate.Value;
            bool isSameDayCheckout = actualHours <= 24; // Same day if 24 hours or less
            
            if (isSameDayCheckout)
            {
                calculation.IsSameDayCheckout = true;
                
                if (isEarlyCheckout)
                {
                    // Same-day checkout that is also early (customer had a later planned checkout date)
                    calculation.IsEarlyCheckout = true;
                    calculation.CheckoutType = "Same-Day Early";
                }
                else
                {
                    calculation.CheckoutType = "Same-Day";
                }
            }
            else if (isEarlyCheckout)
            {
                calculation.IsEarlyCheckout = true;
                calculation.CheckoutType = "Early";
            }
            else
            {
                calculation.CheckoutType = "Normal";
            }
            
            // Calculate recalculated amount based on actual nights stayed (minimum 1 night)
            // This ensures customers only pay for nights actually stayed, with minimum 1 night charge
            calculation.RecalculatedAmount = room.CostPerNight * calculation.ActualNights;

            // Calculate final amount due (can be negative if overpaid due to early checkout)
            calculation.FinalAmountDue = calculation.RecalculatedAmount - calculation.AmountPaid;

            return calculation;
        }

        /// <summary>
        /// Get checkout summary with savings/additional charges
        /// </summary>
        public CheckoutSummary GetCheckoutSummary(CheckoutViewModel checkoutViewModel)
        {
            var originalTotal = checkoutViewModel.BookingCalculations.Sum(c => c.OriginalAmount);
            var recalculatedTotal = checkoutViewModel.RecalculatedTotalAmount;
            var totalPaid = checkoutViewModel.TotalAmountPaid;

            return new CheckoutSummary
            {
                OriginalTotalAmount = originalTotal,
                RecalculatedTotalAmount = recalculatedTotal,
                TotalAmountPaid = totalPaid,
                AmountDifference = recalculatedTotal - originalTotal,
                FinalAmountDue = Math.Max(0, recalculatedTotal - totalPaid),
                IsRefundDue = (recalculatedTotal - totalPaid) < 0,
                RefundAmount = Math.Max(0, totalPaid - recalculatedTotal),
                HasSavings = recalculatedTotal < originalTotal,
                SavingsAmount = Math.Max(0, originalTotal - recalculatedTotal),
                HasAdditionalCharges = recalculatedTotal > originalTotal,
                AdditionalChargesAmount = Math.Max(0, recalculatedTotal - originalTotal)
            };
        }
    }    /// <summary>
    /// Summary of checkout calculations
    /// </summary>
    public class CheckoutSummary
    {
        public decimal OriginalTotalAmount { get; set; }
        public decimal RecalculatedTotalAmount { get; set; }
        public decimal TotalAmountPaid { get; set; }
        public decimal AmountDifference { get; set; }
        public decimal FinalAmountDue { get; set; }
        public bool IsRefundDue { get; set; }
        public decimal RefundAmount { get; set; }
        public bool HasSavings { get; set; }
        public decimal SavingsAmount { get; set; }
        public bool HasAdditionalCharges { get; set; }
        public decimal AdditionalChargesAmount { get; set; }
        
        // Additional properties for refund functionality
        public bool HasRefunds => IsRefundDue;
        public decimal TotalRefundAmount => RefundAmount;
        public decimal TotalAdditionalCharges => AdditionalChargesAmount;
    }
}
