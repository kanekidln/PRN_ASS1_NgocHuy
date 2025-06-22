using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class RoomTypeDAO
    {
        public static List<RoomType> GetAll()
        {
            using var context = new HotelDbContext();
            return context.RoomTypes.ToList();
        }
        
        public static RoomType? GetByID(int id)
        {
            using var context = new HotelDbContext();
            return context.RoomTypes.FirstOrDefault(rt => rt.RoomTypeID == id);
        }
        
        public static void Add(RoomType roomType)
        {
            using var context = new HotelDbContext();
            context.RoomTypes.Add(roomType);
            context.SaveChanges();
        }
        
        public static void Update(RoomType roomType)
        {
            using var context = new HotelDbContext();
            context.Entry(roomType).State = EntityState.Modified;
            context.SaveChanges();
        }
        
        public static void Delete(int id)
        {
            using var context = new HotelDbContext();
            var roomType = context.RoomTypes.Find(id);
            if (roomType != null)
            {
                context.RoomTypes.Remove(roomType);
                context.SaveChanges();
            }
        }
        
        public static List<RoomType> Search(string searchString)
        {
            using var context = new HotelDbContext();
            return context.RoomTypes.Where(rt => 
                EF.Functions.Like(rt.RoomTypeName, "%" + searchString + "%") ||
                EF.Functions.Like(rt.TypeDescription, "%" + searchString + "%") ||
                EF.Functions.Like(rt.TypeNote, "%" + searchString + "%")
            ).ToList();
        }
    }
} 