using ZenHotelManagement.WebApp.Models;
using ZenHotelManagement.WebApp.Models.ViewModel;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    /// <summary>
    /// Interface for checkout calculation service
    /// </summary>
    public interface ICheckoutCalculationService
    {
        /// <summary>
        /// Calculate dynamic checkout amounts based on actual checkout date
        /// </summary>
        Task<CheckoutViewModel> CalculateDynamicCheckoutAsync(List<RoomBooking> bookings, Customer customer, DateTime actualCheckoutDate);

        /// <summary>
        /// Calculate checkout amounts for a single booking
        /// </summary>
        Task<CheckoutBookingCalculation> CalculateBookingCheckoutAsync(RoomBooking booking, DateTime actualCheckoutDate);

        /// <summary>
        /// Get checkout summary with savings/additional charges
        /// </summary>
        CheckoutSummary GetCheckoutSummary(CheckoutViewModel checkoutViewModel);
    }
}
