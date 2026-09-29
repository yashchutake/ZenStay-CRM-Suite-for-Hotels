using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZenHotelManagement.Entities.Exceptions
{
   public class CustomerNotFoundException : Exception
    {
        public CustomerNotFoundException(string customerId)
    : base($"Customer with ID '{customerId}' was not found.")
        {
        }
    }
}
