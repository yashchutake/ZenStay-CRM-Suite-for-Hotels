namespace ZenHotelManagement.Shared
{
    public record RoomForCreateDto(string RoomNo, string RoomType, decimal CostPerNight, bool IsAc, bool RoomStatus, bool IsClean, int RoomCapacity);
}
