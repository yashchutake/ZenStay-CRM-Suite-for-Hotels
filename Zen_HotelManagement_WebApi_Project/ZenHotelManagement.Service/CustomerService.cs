using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Exceptions;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service
{
    public class CustomerService : ICustomerService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;
        private readonly IRoomBookingService _roomBookingService;
        private readonly ICabBookingService _cabBookingService;

        public CustomerService(IRepositoryManager repository, IMapper mapper,
            IRoomBookingService roomBookingService, ICabBookingService cabBookingService)
        {
            _repository = repository;
            _mapper = mapper;
            _roomBookingService = roomBookingService;
            _cabBookingService = cabBookingService;
        }
        public CustomerDTOWithBookings AddCustomer(CustomerCreationDTO customer)
        {
            if (customer.RoomBookings == null || !customer.RoomBookings.Any())
                throw new ArgumentException("At least one room booking is required when creating a customer");

            // Create customer first, without room bookings
            var customerEntity = new Customer
            {
                CustomerId = customer.CustomerId,
                IdType = customer.IdType,
                Name = customer.Name,
                Gender = customer.Gender,
                Address = customer.Address,
                Country = customer.Country,
                MobileNo = customer.MobileNo,
                RoomBookings = new List<RoomBooking>(),
                CabBookings = new List<CabBooking>()
            };

            _repository.Customer.AddCustomer(customerEntity);
            _repository.Save(); // Save to get the customer ID

            // Now create room bookings
            foreach (var roomBooking in customer.RoomBookings)
            {
                var bookingToCreate = roomBooking with { CustomerId = customerEntity.Id };
                var createdBooking = _roomBookingService.CreateBooking(bookingToCreate);
            }

            // Create cab bookings if any
            if (customer.CabBookings != null && customer.CabBookings.Any())
            {
                foreach (var cabBooking in customer.CabBookings)
                {
                    var cabBookingToCreate = cabBooking with { CustomerId = customerEntity.Id };
                    _cabBookingService.CreateCabBooking(cabBookingToCreate);
                }
                _repository.Save();
            }

            // Get fresh data after creating all bookings to ensure we have all the relationships loaded
            var customerWithBookings = _repository.Customer
                .ViewCustomerById(customerEntity.CustomerId, false);

            var customerToReturn = _mapper.Map<CustomerDTOWithBookings>(customerWithBookings);
            return customerToReturn;
        }
        public void UpdateCustomer(string customerId, CustomerUpdationDTO customerForUpdate, bool trackChanges)
        {
            // Always track changes when updating to ensure EF can detect changes
            var customerEntity = _repository.Customer.ViewCustomerById(customerId, true);
            if (customerEntity == null)
                throw new CustomerNotFoundException(customerId);

            // Update customer properties manually to ensure they are set
            customerEntity.IdType = customerForUpdate.IdType;
            customerEntity.Name = customerForUpdate.Name;
            customerEntity.Gender = customerForUpdate.Gender;
            customerEntity.Address = customerForUpdate.Address;
            customerEntity.Country = customerForUpdate.Country;
            customerEntity.MobileNo = customerForUpdate.MobileNo;

            // Explicitly call UpdateCustomer to ensure entity is marked as modified
            _repository.Customer.UpdateCustomer(customerEntity);
            _repository.Save();
        }

        public IEnumerable<CustomerDTOWithBookings> ViewAllCustomers(bool trackChanges)
        {
            var customers = _repository.Customer.ViewAllCustomers(trackChanges);
            var customerDTOs = _mapper.Map<IEnumerable<CustomerDTOWithBookings>>(customers);
            return customerDTOs;
        }

        public CustomerDTOWithBookings ViewCustomerById(string customerId, bool trackChanges)
        {
            var customer = _repository.Customer.ViewCustomerById(customerId, trackChanges);
            if (customer == null)
                throw new CustomerNotFoundException(customerId);
            var customerDTO = _mapper.Map<CustomerDTOWithBookings>(customer);
            return customerDTO;
        }

        public IEnumerable<CustomerDTOWithBookings> ViewCustomersByRoomType(string roomType, bool trackChanges)
        {
            var customers = _repository.Customer.ViewCustomersByRoomType(roomType, trackChanges);
            var customerDTOs = _mapper.Map<IEnumerable<CustomerDTOWithBookings>>(customers);
            return customerDTOs;
        }
    }
}
