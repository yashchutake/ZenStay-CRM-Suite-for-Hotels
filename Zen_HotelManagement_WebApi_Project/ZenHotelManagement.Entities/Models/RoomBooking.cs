using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ZenHotelManagement.Entities.Models
{
    public class RoomBooking
    {
        [Key]
        public int BookingId { get; set; }

        [Required]
        public int CustomerId { get; set; }


        [Required]
        public int RoomId { get; set; }


        [Required]
        public DateTime CheckInDate { get; set; }        //Can checkout at any time
        public DateTime? CheckOutDate { get; set; }

         public decimal? TotalAmount { get; set; }

        public decimal? PendingAmount { get; set; }

        [Required]
        public decimal AmountPaid { get; set; }


        [Required]
        public DateTime BookingDate { get; set; }[Required]
        public string BookingStatus { get; set; } = string.Empty; // e.g., "Confirmed", "Cancelled", "Checked-in", "Completed"

        [Required]
        public int NumberOfGuests { get; set; } // Number of guests for the booking


        // Navigation properties
        [ForeignKey(nameof(CustomerId))]
        public Customer Customer { get; set; } = null!;

        [ForeignKey(nameof(RoomId))]
        public Room Room { get; set; } = null!;
    }
}
