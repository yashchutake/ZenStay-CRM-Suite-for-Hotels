namespace ZenHotelManagement.Shared
{
    public record CabBookingDtoForOperation(
        DateTime PickUpDateTime,
        DateTime? DropOffDateTime, 
        string PickUpLocation, 
        string? DropOffLocation,
        int CustomerId, 
        int CabDriverId, 
        decimal Fare);
}
