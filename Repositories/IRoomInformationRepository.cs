using BusinessObjects;
using System;
using System.Collections.Generic;

namespace Repositories
{
    public interface IRoomInformationRepository
    {
        List<RoomInformation> GetAll();
        RoomInformation? GetByID(int id);
        void Add(RoomInformation room);
        void Update(RoomInformation room);
        void Delete(int id);
        List<RoomInformation> Search(string searchString);
        List<RoomInformation> GetByRoomType(int roomTypeId);
        List<RoomInformation> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate);
    }

    public class RoomInformationRepository : IRoomInformationRepository
    {
        public List<RoomInformation> GetAll() => DataAccessObjects.RoomInformationDAO.GetAll();
        public RoomInformation? GetByID(int id) => DataAccessObjects.RoomInformationDAO.GetByID(id);
        public void Add(RoomInformation room) => DataAccessObjects.RoomInformationDAO.Add(room);
        public void Update(RoomInformation room) => DataAccessObjects.RoomInformationDAO.Update(room);
        public void Delete(int id) => DataAccessObjects.RoomInformationDAO.Delete(id);
        public List<RoomInformation> Search(string searchString) => DataAccessObjects.RoomInformationDAO.Search(searchString);
        public List<RoomInformation> GetByRoomType(int roomTypeId) => DataAccessObjects.RoomInformationDAO.GetByRoomType(roomTypeId);
        public List<RoomInformation> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate) => 
            DataAccessObjects.RoomInformationDAO.GetAvailableRooms(checkInDate, checkOutDate);
    }
} 