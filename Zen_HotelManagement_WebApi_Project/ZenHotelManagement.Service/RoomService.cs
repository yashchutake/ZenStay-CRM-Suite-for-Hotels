using AutoMapper;
using ZenHotelManagement.Contracts;
using ZenHotelManagement.Entities.Exceptions;
using ZenHotelManagement.Entities.Models;
using ZenHotelManagement.Service.Contracts;
using ZenHotelManagement.Shared;

namespace ZenHotelManagement.Service
{
    public class RoomService : IRoomService
    {
        private readonly IRepositoryManager _repository;
        private readonly IMapper _mapper;

        public RoomService(IRepositoryManager repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }        public RoomDto CreateRoom(RoomForCreateDto room)
        {
            // ✅ Check if RoomNo already exists
            var existingRoom = _repository.Room.GetRoomByNo(room.RoomNo, false);
            if (existingRoom != null)
            {
                throw new InvalidOperationException($"Room number {room.RoomNo} already exists");
            }
            var roomEntity = _mapper.Map<Room>(room);
            _repository.Room.CreateRoom(roomEntity);
            _repository.Save();

            return _mapper.Map<RoomDto>(roomEntity);

        }

        public void DeleteRoom(string roomNo)
        {
            // Find the room by room number (trackChanges: true for update)
            var roomEntity = _repository.Room.GetRoomByNo(roomNo, trackChanges: true);
            if (roomEntity == null)
                throw new RoomNotFoundException(roomNo);

            // Mark as deleted (soft delete)
            roomEntity.IsDeleted = !roomEntity.IsDeleted;
            _repository.Save();
        }


        public IEnumerable<RoomDto> GetAllRooms(bool trackChanges)
        {
            var rooms = _repository.Room.GetAllRooms(trackChanges);
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }
        public RoomDto GetRoomByNo(string roomNo, bool trackChanges)
        {
            var room = _repository.Room.GetRoomByNo(roomNo, trackChanges);
            if (room == null)
                throw new RoomNotFoundException(roomNo);

            return _mapper.Map<RoomDto>(room);
        }

        public RoomDto GetRoomById(int roomId, bool trackChanges)
        {
            var room = _repository.Room.GetRoomById(roomId, trackChanges);
            if (room == null)
                throw new RoomNotFoundException(roomId.ToString());

            return _mapper.Map<RoomDto>(room);
        }

        public IEnumerable<RoomDto> GetRoomsByType(string roomType, bool trackChanges)
        {
            var rooms = _repository.Room.GetRoomsByType(roomType, trackChanges);
            return _mapper.Map<IEnumerable<RoomDto>>(rooms);
        }

        public void UpdateRoom(string roomNo, RoomForUpdateDto room, bool trackChanges)
        {
            var roomEntity = _repository.Room.GetRoomByNo(roomNo, trackChanges);
            if (roomEntity == null)
                throw new RoomNotFoundException(roomNo);

            _mapper.Map(room, roomEntity);
            _repository.Save();
        }
    }
}
