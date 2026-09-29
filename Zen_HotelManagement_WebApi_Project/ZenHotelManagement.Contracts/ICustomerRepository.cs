using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Contracts
{    public interface ICustomerRepository
    {       
        IEnumerable<Customer> ViewAllCustomers(bool trackChanges);
        Customer ViewCustomerById(string customerId, bool trackChanges);
        void AddCustomer(Customer customer);
        void UpdateCustomer(Customer customer);
        IEnumerable<Customer> ViewCustomersByRoomType(string roomType, bool trackChanges);
    }
}
