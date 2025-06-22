using BusinessObjects;
using Repositories;
using System;
using System.Collections.Generic;

namespace Services
{
    public class BookingReservationService
    {
        private readonly IBookingReservationRepository _repo = new BookingReservationRepository();
        
        public List<BookingReservation> GetBookings() => _repo.GetAll();
        
        public BookingReservation? GetBookingByID(int id) => _repo.GetByID(id);
        
        public void Add(BookingReservation booking) => _repo.Add(booking);
        
        public void Update(BookingReservation booking) => _repo.Update(booking);
        
        public void Delete(int id) => _repo.Delete(id);
        
        public List<BookingReservation> GetByCustomer(int customerId) => _repo.GetByCustomer(customerId);
        
        public List<BookingReservation> GetByRoom(int roomId) => _repo.GetByRoom(roomId);
        
        public List<BookingReservation> GetByDateRange(DateTime startDate, DateTime endDate) => 
            _repo.GetByDateRange(startDate, endDate);
            
        public List<BookingReservation> GetByBookingType(int bookingType) => 
            _repo.GetByBookingType(bookingType);
    }
} 