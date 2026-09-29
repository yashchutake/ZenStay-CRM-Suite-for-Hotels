using AutoMapper;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Service.Contracts;

namespace ZenHotelManagement.Service
{
    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<ICabDriverService> _cabDriverService;
        private readonly Lazy<IRoomService> _roomService;
        private readonly Lazy<ICabBookingService> _cabBookingService;
        private readonly Lazy<IRoomBookingService> _bookingService;
        private readonly Lazy<ICustomerService> _customerService;

        public ServiceManager(IRepositoryManager repositoryManager, IMapper mapper)
        {
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(repositoryManager, mapper));
            _cabDriverService = new Lazy<ICabDriverService>(() => new CabDriverService(repositoryManager, mapper));
            _roomService = new Lazy<IRoomService>(() => new RoomService(repositoryManager, mapper));
            _cabBookingService = new Lazy<ICabBookingService>(() => new CabBookingService(repositoryManager, mapper));
            _bookingService = new Lazy<IRoomBookingService>(() => new RoomBookingService(repositoryManager, mapper));
            _customerService = new Lazy<ICustomerService>(() => new CustomerService(repositoryManager, mapper, _bookingService.Value, _cabBookingService.Value));
        }
        public IEmployeeService EmployeeService => _employeeService.Value;
        public ICabDriverService CabDriverService => _cabDriverService.Value;
        public IRoomService RoomService => _roomService.Value;

        public ICabBookingService CabBookingService => _cabBookingService.Value;
        public IRoomBookingService RoomBookingService => _bookingService.Value;
        public ICustomerService CustomerService => _customerService.Value;
    }
}