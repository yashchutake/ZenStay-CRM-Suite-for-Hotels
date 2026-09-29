using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> ViewAllCustomers();

        Customer ViewCustomerById(string customerId);

        Task<Customer> AddCustomer(Customer customer);

        IEnumerable<Customer> ViewCustomersByRoomType(string roomType);

        void UpdateCustomer(string customerId, Customer customerForUpdate);

    }
}
