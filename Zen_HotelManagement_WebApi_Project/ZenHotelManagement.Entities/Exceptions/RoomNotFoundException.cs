using EmployeePortalWebApi.Entities.Exceptions;

namespace ZenHotelManagement.Entities.Exceptions
{
    public sealed class RoomNotFoundException : NotFoundException
    {
        public RoomNotFoundException(string RoomNo)
    : base($"This room with no : {RoomNo} doesn't exist in the database")
        {
        }
    }
}
