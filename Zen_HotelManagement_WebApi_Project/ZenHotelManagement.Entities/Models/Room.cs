using System.ComponentModel.DataAnnotations;

namespace ZenHotelManagement.Entities.Models
{
    public class Room
    {
        [Required(ErrorMessage = "Room Id is required")]
        [Key]
        public int RoomId { get; set; }

        [Required(ErrorMessage = "Room no is required")]
        public string? RoomNo { get; set; }

        [Required(ErrorMessage = "Room Type is required")]
        public string? RoomType { get; set; }

        [Required(ErrorMessage = "Room AC status is required")]
        public bool IsAc { get; set; }

        [Required(ErrorMessage = "Cost per night is required")]
        public decimal CostPerNight { get; set; } // Cost per night for the room

        [Required(ErrorMessage = "Room status is required")]
        public bool RoomStatus { get; set; } = true;

        [Required(ErrorMessage = "Room clean status is required")]
        public bool IsClean { get; set; } = true;

        [Required]
        public int RoomCapacity { get; set; } // Number of guests the room can accommodate

        public bool IsDeleted { get; set; } = false;

        public virtual ICollection<RoomBooking>? Bookings { get; set; }
    }
}
