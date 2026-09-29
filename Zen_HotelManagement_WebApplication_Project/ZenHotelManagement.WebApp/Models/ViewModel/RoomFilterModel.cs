namespace ZenHotelManagement.WebApp.Models.ViewModel
{
    public class RoomFilterModel
    {
        public string? SearchTerm { get; set; }
        public string? RoomType { get; set; }
        public bool? IsAc { get; set; }
        public bool? IsClean { get; set; }

        public bool? IsDeleted { get; set; }
        public bool? RoomStatus { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
