using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Models;

namespace ZenHotelManagement.Repository
{
    public class RoomRepository : RepositoryBase<Room>, IRoomRepository
    {
        public RoomRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {

        }
        public void CreateRoom(Room room) => Create(room);

        public IEnumerable<Room> GetAllRooms(bool trackChanges) =>
            FindAll(trackChanges)
            .OrderBy(r => r.RoomNo)
            .ToList();

        public Room GetRoomByNo(string RoomNo, bool trackChanges) =>
            FindByCondition(r => r.RoomNo != null && r.RoomNo.Equals(RoomNo), trackChanges).SingleOrDefault()!;

        public Room GetRoomById(int roomId, bool trackChanges) =>
            FindByCondition(r => r.RoomId == roomId, trackChanges).SingleOrDefault()!;

        public IEnumerable<Room> GetRoomsByType(string roomType, bool trackChanges) =>
            FindByCondition(r => r.RoomType != null && r.RoomType.Equals(roomType), trackChanges)
            .OrderBy(r => r.RoomNo)
            .ToList();
    }
}
