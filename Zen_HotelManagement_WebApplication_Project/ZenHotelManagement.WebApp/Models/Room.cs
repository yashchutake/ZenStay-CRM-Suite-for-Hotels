using System.ComponentModel.DataAnnotations;

namespace ZenHotelManagement.WebApp.Models
{
    public class Room
    {
        [Key]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room number is required")]
        [StringLength(10, ErrorMessage = "Room number cannot exceed 10 characters")]
        [Display(Name = "Room Number")]
        public string? RoomNo { get; set; }

        [Required(ErrorMessage = "Room type is required")]
        [Display(Name = "Room Type")]
        public string? RoomType { get; set; }

        [Display(Name = "Air Conditioning")]
        public bool IsAc { get; set; }

        [Required(ErrorMessage = "Cost per night is required")]
        [Range(100, 50000, ErrorMessage = "Cost per night must be between ₹100 and ₹50,000")]
        [Display(Name = "Cost Per Night (₹)")]
        public decimal CostPerNight { get; set; }

        [Display(Name = "Room Status")]
        public bool RoomStatus { get; set; } = true; // Default to available

        [Display(Name = "Clean Status")]
        public bool IsClean { get; set; } = true; // Default to clean

        [Required(ErrorMessage = "Room capacity is required")]
        [Range(1, 10, ErrorMessage = "Room capacity must be between 1 and 10 guests")]
        [Display(Name = "Room Capacity")]
        public int RoomCapacity { get; set; }

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<RoomBooking>? Bookings { get; set; }
    }
}
