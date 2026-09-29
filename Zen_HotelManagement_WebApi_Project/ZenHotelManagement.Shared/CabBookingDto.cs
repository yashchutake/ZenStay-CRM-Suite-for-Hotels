namespace ZenHotelManagement.Shared
{
    public record CabBookingDto(int CabBookingId, DateTime PickUpDateTime,
        DateTime? DropOffDateTime, string PickUpLocation, string? DropOffLocation,
        int CustomerId, int CabDriverId, decimal? Fare);


}
