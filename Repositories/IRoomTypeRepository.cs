using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface IRoomTypeRepository
    {
        List<RoomType> GetAll();
        RoomType? GetByID(int id);
        void Add(RoomType roomType);
        void Update(RoomType roomType);
        void Delete(int id);
        List<RoomType> Search(string searchString);
    }

    public class RoomTypeRepository : IRoomTypeRepository
    {
        public List<RoomType> GetAll() => DataAccessObjects.RoomTypeDAO.GetAll();
        public RoomType? GetByID(int id) => DataAccessObjects.RoomTypeDAO.GetByID(id);
        public void Add(RoomType roomType) => DataAccessObjects.RoomTypeDAO.Add(roomType);
        public void Update(RoomType roomType) => DataAccessObjects.RoomTypeDAO.Update(roomType);
        public void Delete(int id) => DataAccessObjects.RoomTypeDAO.Delete(id);
        public List<RoomType> Search(string searchString) => DataAccessObjects.RoomTypeDAO.Search(searchString);
    }
} 