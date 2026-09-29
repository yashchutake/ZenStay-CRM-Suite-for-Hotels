using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service.Contracts
{
   public interface ICustomerService
    {
        IEnumerable<CustomerDTOWithBookings> ViewAllCustomers(bool trackChanges);

        CustomerDTOWithBookings ViewCustomerById(string customerId, bool trackChanges);

        CustomerDTOWithBookings AddCustomer(CustomerCreationDTO customer);

        IEnumerable<CustomerDTOWithBookings> ViewCustomersByRoomType(string roomType, bool trackChanges);

        void UpdateCustomer(string customerId, CustomerUpdationDTO customerForUpdate, bool trackChanges);

       
    }
}
