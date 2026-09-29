using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Models.ViewModel
{
    public class CustomerDetailsViewModel
    {
        public Customer Customer { get; set; }
        public List<IGrouping<DateTime, RoomBooking>> GroupedRoomBookings { get; set; } = new List<IGrouping<DateTime, RoomBooking>>();
        public List<IGrouping<DateTime, CabBooking>> GroupedCabBookings { get; set; } = new List<IGrouping<DateTime, CabBooking>>();
    }
}
