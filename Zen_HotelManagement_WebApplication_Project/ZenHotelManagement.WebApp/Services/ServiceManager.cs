using ZenHotelManagement.WebApp.Services.Contracts;

namespace ZenHotelManagement.WebApp.Services
{

    public class ServiceManager : IServiceManager
    {
        private readonly Lazy<IEmployeeService> _employeeService;
        private readonly Lazy<ICabDriverService> _cabDriverService;
        private readonly Lazy<IRoomService> _roomService;
        private readonly Lazy<ICabBookingService> _cabBookingService;
        private readonly Lazy<IRoomBookingService> _bookingService;
        private readonly Lazy<ICustomerService> _customerService;

        public ServiceManager(IHttpClientFactory httpClientFactory)
        {
            _employeeService = new Lazy<IEmployeeService>(() => new EmployeeService(httpClientFactory));
            _cabDriverService = new Lazy<ICabDriverService>(() => new CabDriverService(httpClientFactory));
            _roomService = new Lazy<IRoomService>(() => new RoomService(httpClientFactory));
            _cabBookingService = new Lazy<ICabBookingService>(() => new CabBookingService(httpClientFactory));
            _bookingService = new Lazy<IRoomBookingService>(() => new RoomBookingService(httpClientFactory));
            _customerService = new Lazy<ICustomerService>(() => new CustomerService(httpClientFactory, _bookingService.Value, _cabBookingService.Value));
        }
       

        public IEmployeeService EmployeeService => _employeeService.Value;
        public ICabDriverService CabDriverService => _cabDriverService.Value;
        public IRoomService RoomService => _roomService.Value;
        public ICabBookingService CabBookingService => _cabBookingService.Value;
        public IRoomBookingService RoomBookingService => _bookingService.Value;
        public ICustomerService CustomerService => _customerService.Value;

    }


}
