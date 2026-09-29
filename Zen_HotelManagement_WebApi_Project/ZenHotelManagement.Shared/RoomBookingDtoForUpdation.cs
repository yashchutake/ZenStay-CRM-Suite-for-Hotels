namespace ZenHotelManagement.Shared
{
    public record RoomBookingDtoForUpdation(
        int RoomId,
        DateTime CheckInDate,
        DateTime? CheckOutDate,
        decimal TotalAmount,
        decimal AmountPaid,
        decimal? PendingAmount,
        string BookingStatus,
        int NumberOfGuests
    );
}
