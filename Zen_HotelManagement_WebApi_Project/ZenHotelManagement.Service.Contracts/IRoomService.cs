using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service.Contracts
{
    public interface IRoomService
    {
        // f. View all Rooms
        IEnumerable<RoomDto> GetAllRooms(bool trackChanges);

        // g. View Rooms by Type (e.g., Single, Double, Deluxe, etc.)
        IEnumerable<RoomDto> GetRoomsByType(string roomType, bool trackChanges);
        RoomDto GetRoomByNo(string roomNo, bool trackChanges);
        RoomDto GetRoomById(int roomId, bool trackChanges);

        // h. Add new Rooms
        RoomDto CreateRoom(RoomForCreateDto room);

        // i. Update Room Details - Status (Available, Occupied), Cleanliness (Clean, Dirty)
        void UpdateRoom(string roomNo, RoomForUpdateDto room, bool trackChanges);

        void DeleteRoom(string roomNo);

    }
}
