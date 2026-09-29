using System.ComponentModel.DataAnnotations;

namespace ZenHotelManagement.Entities.Models
{
    public class CabDriver
    {
        public int CabDriverId { get; set; }

        [Required(ErrorMessage = "CabDriver name is required")]
        [MaxLength(30, ErrorMessage = "CabDriver name length is 30 only")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "CabDriver Age is required")]
        public int Age { get; set; }

        [Required(ErrorMessage = "CabDriver Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "CabDriver CarVendor is required")]
        public string? CarVendor { get; set; }

        [Required(ErrorMessage = "CabDriver CarType is required")]
        public string? CarType { get; set; }

        public bool IsAvailable { get; set; } = true;

        public bool IsDeleted { get; set; } = false;

        public ICollection<CabBooking>? CabBookings { get; set; }
    }
}