using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class RoomInformationDAO
    {
        public static List<RoomInformation> GetAll()
        {
            using var context = new HotelDbContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == 1)
                .ToList();
        }
        
        public static RoomInformation? GetByID(int id)
        {
            using var context = new HotelDbContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .FirstOrDefault(r => r.RoomID == id);
        }
        
        public static void Add(RoomInformation room)
        {
            using var context = new HotelDbContext();
            context.RoomInformations.Add(room);
            context.SaveChanges();
        }
        
        public static void Update(RoomInformation room)
        {
            using var context = new HotelDbContext();
            context.Entry(room).State = EntityState.Modified;
            context.SaveChanges();
        }
        
        public static void Delete(int id)
        {
            using var context = new HotelDbContext();
            var room = context.RoomInformations.Find(id);
            if (room != null)
            {
                room.RoomStatus = 2; // Deleted
                context.SaveChanges();
            }
        }
        
        public static List<RoomInformation> Search(string searchString)
        {
            using var context = new HotelDbContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .Where(r => 
                    r.RoomStatus == 1 && 
                    (EF.Functions.Like(r.RoomNumber, "%" + searchString + "%") ||
                     EF.Functions.Like(r.RoomDescription, "%" + searchString + "%"))
                ).ToList();
        }
        
        public static List<RoomInformation> GetByRoomType(int roomTypeId)
        {
            using var context = new HotelDbContext();
            return context.RoomInformations
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == 1 && r.RoomTypeID == roomTypeId)
                .ToList();
        }
        
        public static List<RoomInformation> GetAvailableRooms(DateTime checkInDate, DateTime checkOutDate)
        {
            using var context = new HotelDbContext();
            
            // Get all active rooms
            var allRooms = context.RoomInformations
                .Include(r => r.RoomType)
                .Where(r => r.RoomStatus == 1)
                .ToList();
                
            // Get all rooms that are booked during the specified period
            var bookedRoomIds = context.BookingReservations
                .Where(b => 
                    b.BookingStatus == 1 && // Active bookings only
                    ((checkInDate >= b.StartDate && checkInDate < b.EndDate) || // Check-in date falls within an existing booking
                    (checkOutDate > b.StartDate && checkOutDate <= b.EndDate) || // Check-out date falls within an existing booking
                    (checkInDate <= b.StartDate && checkOutDate >= b.EndDate))) // Booking falls entirely within the requested period
                .Select(b => b.RoomID)
                .Distinct()
                .ToList();
                
            // Return rooms that are not booked during the specified period
            return allRooms.Where(r => !bookedRoomIds.Contains(r.RoomID)).ToList();
        }
    }
} 