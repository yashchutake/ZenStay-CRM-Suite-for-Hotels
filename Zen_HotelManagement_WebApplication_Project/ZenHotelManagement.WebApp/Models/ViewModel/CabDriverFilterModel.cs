namespace ZenHotelManagement.WebApp.Models.ViewModel
{
    public class CabDriverFilterModel
    {
        public string? SearchTerm { get; set; }
        public string? CarVendor { get; set; }
        public string? CarType { get; set; }        public string? Gender { get; set; }
        public bool? IsCurrent { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}