using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class BookingReservationDAO
    {
        // Singleton pattern
        private static BookingReservationDAO? instance;
        private static readonly object instanceLock = new object();

        private BookingReservationDAO() { }

        public static BookingReservationDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new BookingReservationDAO();
                    }
                    return instance;
                }
            }
        }

        // Get all bookings
        public List<BookingReservation> GetBookingReservations()
        {
            using var context = new HotelDbContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .ToList();
        }

        // Get booking by ID
        public BookingReservation? GetBookingReservationByID(int id)
        {
            using var context = new HotelDbContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .FirstOrDefault(b => b.BookingReservationID == id);
        }

        // Get bookings by customer ID
        public List<BookingReservation> GetBookingReservationsByCustomerID(int customerID)
        {
            using var context = new HotelDbContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.CustomerID == customerID)
                .ToList();
        }

        // Get bookings by room ID
        public List<BookingReservation> GetBookingReservationsByRoomID(int roomID)
        {
            using var context = new HotelDbContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.RoomID == roomID)
                .ToList();
        }

        // Get bookings by date range
        public List<BookingReservation> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            using var context = new HotelDbContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.BookingDate >= startDate && b.BookingDate <= endDate)
                .ToList();
        }

        // Get bookings by booking type
        public List<BookingReservation> GetByBookingType(int bookingType)
        {
            using var context = new HotelDbContext();
            return context.BookingReservations
                .Include(b => b.Customer)
                .Include(b => b.Room)
                .Where(b => b.BookingType == bookingType)
                .ToList();
        }

        // Add a new booking
        public void Add(BookingReservation booking)
        {
            using var context = new HotelDbContext();
            
            // Calculate booking duration
            TimeSpan duration = booking.EndDate - booking.StartDate;
            int days = (int)duration.TotalDays;
            booking.BookingDuration = days;
            
            context.BookingReservations.Add(booking);
            context.SaveChanges();
        }

        // Update a booking
        public void Update(BookingReservation booking)
        {
            using var context = new HotelDbContext();
                
                // Calculate booking duration
                TimeSpan duration = booking.EndDate - booking.StartDate;
                int days = (int)duration.TotalDays;
                booking.BookingDuration = days;
                
            context.Entry(booking).State = EntityState.Modified;
            context.SaveChanges();
        }

        // Delete a booking
        public void Delete(int id)
        {
            using var context = new HotelDbContext();
            var booking = context.BookingReservations.Find(id);
            if (booking != null)
            {
                booking.BookingStatus = 3; // Cancelled
                context.SaveChanges();
            }
        }
    }
} 