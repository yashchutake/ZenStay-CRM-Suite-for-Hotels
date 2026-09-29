using System.ComponentModel.DataAnnotations;

namespace ZenHotelManagement.WebApp.Models.ViewModel
{
    public class CustomerUpdateViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer ID is required")]
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer ID Type is required")]
        [Display(Name = "ID Type")]
        public string IdType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Customer Name is required")]
        [Display(Name = "Full Name")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Gender is required")]
        [Display(Name = "Gender")]
        public string Gender { get; set; } = string.Empty;

        [Display(Name = "Address")]
        [StringLength(500, ErrorMessage = "Address cannot be longer than 500 characters")]
        public string? Address { get; set; }

        [Display(Name = "Country")]
        [StringLength(100, ErrorMessage = "Country name cannot be longer than 100 characters")]
        public string? Country { get; set; }

        [Required(ErrorMessage = "Mobile number is required")]
        [Display(Name = "Mobile Number")]
        [Phone(ErrorMessage = "Please enter a valid mobile number")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Please enter a valid mobile number")]
        public string MobileNo { get; set; } = string.Empty;

        // Helper method to convert from Customer model
        public static CustomerUpdateViewModel FromCustomer(Customer customer)
        {
            return new CustomerUpdateViewModel
            {
                Id = customer.Id,
                CustomerId = customer.CustomerId ?? string.Empty,
                IdType = customer.IdType ?? string.Empty,
                Name = customer.Name ?? string.Empty,
                Gender = customer.Gender ?? string.Empty,
                Address = customer.Address,
                Country = customer.Country,
                MobileNo = customer.MobileNo ?? string.Empty
            };
        }

        // Helper method to convert to Customer model
        public Customer ToCustomer()
        {
            return new Customer
            {
                Id = this.Id,
                CustomerId = this.CustomerId,
                IdType = this.IdType,
                Name = this.Name,
                Gender = this.Gender,
                Address = this.Address,
                Country = this.Country,
                MobileNo = this.MobileNo
            };
        }
    }
}