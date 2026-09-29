

using ZenHotelManagement.Contracts;

namespace ZenHotelManagement.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;        
        private readonly Lazy<ICabDriverRepository> _cabDriverRepository;
        private readonly Lazy<IEmployeeRepository> _employeeRepository;
        private readonly Lazy<IRoomRepository> _roomRepository;
        private readonly Lazy<ICustomerRepository> _customerRepository;
        private readonly Lazy<ICabBookingRepository> _cabBookingRepository;
        private readonly Lazy<IRoomBookingRepository> _bookingRepository;


        public RepositoryManager(RepositoryContext repositoryContext)
        {            _repositoryContext = repositoryContext;
            _cabDriverRepository = new Lazy<ICabDriverRepository>(() => new CabDriverRepository(repositoryContext));
            _employeeRepository = new Lazy<IEmployeeRepository>(() => new EmployeeRepository(repositoryContext));
            _roomRepository = new Lazy<IRoomRepository>(() => new RoomRepository(repositoryContext));
            _customerRepository = new Lazy<ICustomerRepository>(() => new CustomerRepository(repositoryContext));
            _cabBookingRepository = new Lazy<ICabBookingRepository>(() => new CabBookingRepository(repositoryContext));
            _bookingRepository = new Lazy<IRoomBookingRepository>(() => new RoomBookingRepository(repositoryContext));

        }

        public ICabDriverRepository CabDriver => _cabDriverRepository.Value;
        public IEmployeeRepository Employee => _employeeRepository.Value;
        public IRoomRepository Room => _roomRepository.Value;
        public ICustomerRepository Customer => _customerRepository.Value;        
        public ICabBookingRepository CabBooking => _cabBookingRepository.Value;
        public IRoomBookingRepository RoomBooking => _bookingRepository.Value;

        public void Save() => _repositoryContext.SaveChanges();
        
    }
}
