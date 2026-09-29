using ZenHotelManagement.WebApp.Models;

namespace ZenHotelManagement.WebApp.Services.Contracts
{
    public interface IRoomService
    {
        // f. View all Rooms
        IEnumerable<Room> GetAllRooms();

        // g. View Rooms by Type (e.g., Single, Double, Deluxe, etc.)
        IEnumerable<Room> GetRoomsByType(string roomType);
        Task<Room?> GetRoomByNoAsync(string roomNo);
        Task<Room?> GetRoomByIdAsync(int roomId);

        // h. Add new Rooms
        Room CreateRoom(Room room);

        // i. Update Room Details - Status (Available, Occupied), Cleanliness (Clean, Dirty)
        void UpdateRoom(string roomNo, Room room);

        // Update room clean status
        Task<bool> UpdateRoomCleanStatusAsync(string roomNo, bool isClean);        // Delete room (soft delete)
        Task<bool> DeleteRoomAsync(string roomNo);

        // Get available rooms for booking
        Task<IEnumerable<Room>> GetAvailableRoomsAsync();
    }
}
