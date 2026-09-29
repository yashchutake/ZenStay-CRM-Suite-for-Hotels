namespace ZenHotelManagement.WebApp.Models
{
    public class Login
    {
        public required string Name { get; set; }
        public required string Password { get; set; }
        public required string ReturnUrl { get; set; } = "/";
    }
}
