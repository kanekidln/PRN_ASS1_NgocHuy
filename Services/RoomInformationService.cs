using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class RoomInformationService
    {
        private readonly IRoomInformationRepository _repo = new RoomInformationRepository();
        
        public List<RoomInformation> GetRooms() => _repo.GetAll();
        
        public RoomInformation? GetRoomByID(int id) => _repo.GetByID(id);
        
        public void AddRoom(RoomInformation room) => _repo.Add(room);
        
        public void UpdateRoom(RoomInformation room) => _repo.Update(room);
        
        public void DeleteRoom(int id) => _repo.Delete(id);
        
        public List<RoomInformation> GetByRoomType(int roomTypeId) => _repo.GetByRoomType(roomTypeId);
        
        public List<RoomInformation> SearchRooms(string searchString) => _repo.Search(searchString);
        
        public List<RoomInformation> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate) => 
            _repo.GetAvailableRooms(checkInDate, checkOutDate);
    }
} 