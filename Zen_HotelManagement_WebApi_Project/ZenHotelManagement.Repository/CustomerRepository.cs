using Microsoft.EntityFrameworkCore;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository
{
    public class CustomerRepository : RepositoryBase<Customer>, ICustomerRepository
    {
        public CustomerRepository(RepositoryContext repository) : base(repository)
        {
        }       
        public IEnumerable<Customer> ViewAllCustomers(bool trackChanges) => 
            FindAll(trackChanges)
            .Include(c => c.RoomBookings!)
                .ThenInclude(rb => rb.Room)
            .Include(c => c.CabBookings!)
                .ThenInclude(cb => cb.CabDriver)
            .ToList();       
    
        public Customer ViewCustomerById(string customerId, bool trackChanges)
        {
            var customer = FindByCondition(customer => customer.CustomerId == customerId, trackChanges)
                .Include(c => c.RoomBookings!)
                    .ThenInclude(rb => rb.Room)
                .Include(c => c.CabBookings!)
                    .ThenInclude(cb => cb.CabDriver)
                .FirstOrDefault();
            return customer ?? throw new Exception($"Customer not found with ID: {customerId}");
        }

        public void AddCustomer(Customer customer) => Create(customer);

        public void UpdateCustomer(Customer customer) => Update(customer);

        public IEnumerable<Customer> ViewCustomersByRoomType(string roomType, bool trackChanges)
        {
            return FindAll(trackChanges)
                .Include(c => c.RoomBookings!)
                    .ThenInclude(rb => rb.Room)
                .Include(c => c.CabBookings!)
                    .ThenInclude(cb => cb.CabDriver)
                .Where(c => c.RoomBookings != null && 
                           c.RoomBookings.Any(b => b.Room != null && 
                                                  b.Room.RoomType == roomType))
                .ToList();
        }
    }
}