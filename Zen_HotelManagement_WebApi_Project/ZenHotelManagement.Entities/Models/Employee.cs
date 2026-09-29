using System.ComponentModel.DataAnnotations;

namespace ZenHotelManagement.Entities.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Employee name is required")]
        [MaxLength(30, ErrorMessage = "Employee name length is 30 only")]
        public string? Name { get; set; }

        [Required(ErrorMessage = "Employee Age is required")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Employee Gender is required")]
        public string? Gender { get; set; }

        [Required(ErrorMessage = "Employee Position is required")]
        public string? Position { get; set; }

        [Required(ErrorMessage = "Employee Salary is required")]
        public decimal Salary { get; set; }

        [Required(ErrorMessage = "Employee MobileNo is required")]
        public string? MobileNo { get; set; }

        [Required(ErrorMessage = "Employee AdharNo is required")]
        public string? AdharNo { get; set; }

        [Required(ErrorMessage = "Employee Email is required")]
        public string? Email { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
