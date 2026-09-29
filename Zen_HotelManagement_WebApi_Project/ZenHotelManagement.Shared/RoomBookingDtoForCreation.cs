namespace ZenHotelManagement.Shared
{
    public record RoomBookingDtoForCreation(
       int? CustomerId,
       int RoomId,
       DateTime CheckInDate,
       DateTime? CheckOutDate,
       decimal? TotalAmount,
       decimal? PendingAmount,
       decimal AmountPaid,
       DateTime BookingDate,
       string BookingStatus,
       int NumberOfGuests
    );
}
