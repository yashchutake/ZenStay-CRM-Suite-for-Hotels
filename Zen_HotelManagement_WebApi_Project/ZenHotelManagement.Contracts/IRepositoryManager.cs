namespace ZenHotelManagement.Contracts
{     public interface IRepositoryManager
    {      
        ICabDriverRepository CabDriver { get; }
        IEmployeeRepository Employee { get; }
        IRoomRepository Room { get; }
        ICustomerRepository Customer { get; }
        ICabBookingRepository CabBooking { get; }
        IRoomBookingRepository RoomBooking { get; }
       

        void Save();
    }
}
