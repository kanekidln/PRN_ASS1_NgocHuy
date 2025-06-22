using BusinessObjects;
using System;
using System.Collections.Generic;

namespace Repositories
{
    public interface IBookingReservationRepository
    {
        List<BookingReservation> GetAll();
        BookingReservation? GetByID(int id);
        void Add(BookingReservation booking);
        void Update(BookingReservation booking);
        void Delete(int id);
        List<BookingReservation> GetByCustomer(int customerId);
        List<BookingReservation> GetByRoom(int roomId);
        List<BookingReservation> GetByDateRange(DateTime startDate, DateTime endDate);
        List<BookingReservation> GetByBookingType(int bookingType);
    }

    public class BookingReservationRepository : IBookingReservationRepository
    {
        private readonly DataAccessObjects.BookingReservationDAO _bookingDAO;

        public BookingReservationRepository()
        {
            _bookingDAO = DataAccessObjects.BookingReservationDAO.Instance;
        }

        public List<BookingReservation> GetAll() => _bookingDAO.GetBookingReservations();
        public BookingReservation? GetByID(int id) => _bookingDAO.GetBookingReservationByID(id);
        public void Add(BookingReservation booking) => _bookingDAO.Add(booking);
        public void Update(BookingReservation booking) => _bookingDAO.Update(booking);
        public void Delete(int id) => _bookingDAO.Delete(id);
        public List<BookingReservation> GetByCustomer(int customerId) => _bookingDAO.GetBookingReservationsByCustomerID(customerId);
        public List<BookingReservation> GetByRoom(int roomId) => _bookingDAO.GetBookingReservationsByRoomID(roomId);
        public List<BookingReservation> GetByDateRange(DateTime startDate, DateTime endDate) => 
            _bookingDAO.GetByDateRange(startDate, endDate);
        public List<BookingReservation> GetByBookingType(int bookingType) => 
            _bookingDAO.GetByBookingType(bookingType);
    }
} 