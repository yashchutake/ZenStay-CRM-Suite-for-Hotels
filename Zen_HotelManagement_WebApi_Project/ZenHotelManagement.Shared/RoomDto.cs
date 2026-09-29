namespace ZenHotelManagement.Shared
{
    public record RoomDto(int RoomId, string RoomNo, string RoomType, bool IsAc, decimal costPerNight, bool RoomStatus, bool IsClean, int RoomCapacity)
    {
        public bool IsDeleted { get; set; } = false;
    }

}
