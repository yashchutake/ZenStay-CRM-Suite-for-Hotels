namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface IServiceManager
    {
        IEmployeeService EmployeeService { get; }
        ICabDriverService CabDriverService { get; }
        IRoomService RoomService { get; }
        ICabBookingService CabBookingService { get; }
        ICustomerService CustomerService { get; }
        IRoomBookingService RoomBookingService { get; }
    }
}
