namespace ZenHotelManagement.WebApp.Models.ViewModel
{    public class CustomerFilterModel
    {
        public string? SearchTerm { get; set; }
        public string? Gender { get; set; }
        public bool? IsCurrent { get; set; }
        public string? Status { get; set; } // New property for customer status filtering
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
