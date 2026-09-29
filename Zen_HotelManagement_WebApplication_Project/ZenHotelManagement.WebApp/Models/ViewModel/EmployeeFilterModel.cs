namespace ZenHotelManagement.WebApp.Models.ViewModel
{
    public class EmployeeFilterModel
    {
        public string? SearchTerm { get; set; }        public string? Position { get; set; }
        public string? Gender { get; set; }        public string? Status { get; set; } = "Active"; // Can be "Active", "Inactive" or "All"
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
    }
}
