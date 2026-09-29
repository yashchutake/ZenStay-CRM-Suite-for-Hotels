using System.ComponentModel.DataAnnotations;

namespace ZenHotelManagement.WebApp.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee name is required")]
        [MaxLength(30, ErrorMessage = "Employee name length is 30 only")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Name can only contain letters and spaces")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Employee Age is required")]
        [Range(18, 65, ErrorMessage = "Age must be between 18 and 65")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Employee Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Employee Position is required")]
        [MaxLength(50, ErrorMessage = "Position length cannot exceed 50 characters")]
        public string? Position { get; set; }

        [Required(ErrorMessage = "Employee Salary is required")]
        [Range(0, double.MaxValue, ErrorMessage = "Salary must be greater than or equal to 0")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Employee Mobile Number is required")]
        [RegularExpression(@"^\d{10}$", ErrorMessage = "Please enter a valid 10-digit mobile number")]
        [MaxLength(10)]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Employee Aadhar Number is required")]
        [RegularExpression(@"^\d{12}$", ErrorMessage = "Please enter a valid 12-digit Aadhar number")]
        [MaxLength(12)]
        public string? AdharNo { get; set; }

        [Required(ErrorMessage = "Employee Email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        public string? Email { get; set; }

        public bool IsDeleted { get; set; }
    }
}
