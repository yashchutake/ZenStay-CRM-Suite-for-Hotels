namespace ZenHotelManagement.Shared
{    public record RoomBookingDto(
        int BookingId,
        int CustomerId,
        int RoomId,
        DateTime CheckInDate,
        DateTime? CheckOutDate,
        decimal? TotalAmount,
        decimal? PendingAmount,
        decimal AmountPaid,
        DateTime BookingDate,
        string BookingStatus,
        int NumberOfGuests,
        RoomDto Room);
   
}
