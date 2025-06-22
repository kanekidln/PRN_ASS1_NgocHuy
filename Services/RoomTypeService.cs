using BusinessObjects;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class RoomTypeService
    {
        private readonly IRoomTypeRepository _repo = new RoomTypeRepository();
        
        public List<RoomType> GetRoomTypes() => _repo.GetAll();
        
        public RoomType? GetRoomTypeByID(int id) => _repo.GetByID(id);
        
        public void Add(RoomType roomType) => _repo.Add(roomType);
        
        public void Update(RoomType roomType) => _repo.Update(roomType);
        
        public void Delete(int id) => _repo.Delete(id);
        
        public List<RoomType> Search(string searchString) => _repo.Search(searchString);
    }
} 