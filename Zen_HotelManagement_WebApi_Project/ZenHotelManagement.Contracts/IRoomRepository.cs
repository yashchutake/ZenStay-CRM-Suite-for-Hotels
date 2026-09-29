using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Contracts
{
    public interface IRoomRepository
    {
        IEnumerable<Room> GetAllRooms(bool trackChanges);
        Room GetRoomByNo(string RoomNo, bool trackChanges);
        Room GetRoomById(int roomId, bool trackChanges);
        IEnumerable<Room> GetRoomsByType(string roomType, bool trackChanges);
        void CreateRoom(Room room);

    }
}
