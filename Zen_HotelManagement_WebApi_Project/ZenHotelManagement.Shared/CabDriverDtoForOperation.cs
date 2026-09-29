namespace ZenHotelManagement.Shared
{
    public record CabDriverDtoForOperation(string Name, int Age, string Gender, string CarVendor, string CarType, bool IsAvailable, bool IsDeleted, IEnumerable<CabDriverDto>? Customers);
    
}
