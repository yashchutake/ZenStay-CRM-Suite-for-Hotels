using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenHotelManagement.Entities.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        public string? CustomerId { get; set; }

        [Required(ErrorMessage = "Customer ID Type is required")]
        public string? IdType { get; set; }

        [Required(ErrorMessage = "Customer Name is required")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Customer Gender is required")]
        public string? Gender { get; set; }

        public string? Address { get; set; }

        public string? Country { get; set; }

        [Required(ErrorMessage = "Customer MobileNo is required")]
        public string? MobileNo { get; set; }

        public ICollection<CabBooking>? CabBookings { get; set; }
        public virtual ICollection<RoomBooking>? RoomBookings { get; set; }
    }
}