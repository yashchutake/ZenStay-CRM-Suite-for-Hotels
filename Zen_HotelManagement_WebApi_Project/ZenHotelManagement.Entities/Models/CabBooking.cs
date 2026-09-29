using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenHotelManagement.Entities.Models
{
    public class CabBooking
    {
        [Key]
        public int CabBookingId { get; set; }

        [Required(ErrorMessage = "Pick-up date and time is required")]
        public DateTime PickUpDateTime { get; set; }

        [Required(ErrorMessage = "Drop-off date and time is required")]
        public DateTime DropOffDateTime { get; set; }

        [Required(ErrorMessage = "Pick-up location is required")]
        public string? PickUpLocation { get; set; }

        [Required(ErrorMessage = "Drop-off location is required")]
        public string? DropOffLocation { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        [ForeignKey("Customer")]
        public int CustomerId { get; set; }
        public Customer? Customer { get; set; }

        [Required(ErrorMessage = "CabDriver ID is required")]
        [ForeignKey("CabDriver")]
        public int CabDriverId { get; set; }
        public CabDriver? CabDriver { get; set; }

        [Required(ErrorMessage = "Fare is required")]
        public decimal Fare { get; set; }         // Added
        public bool? IsRideCompleted { get; set; } = false;
    }
}