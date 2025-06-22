using System;
using System.Collections.Generic;

namespace BusinessObjects
{
    public class RoomType
    {
        public int RoomTypeID { get; set; }
        public string RoomTypeName { get; set; } = string.Empty;
        public string TypeDescription { get; set; } = string.Empty;
        public string TypeNote { get; set; } = string.Empty;
    }

    public class RoomInformation
    {
        public int RoomID { get; set; }
        public string RoomNumber { get; set; } = string.Empty;
        public string RoomDescription { get; set; } = string.Empty;
        public int RoomMaxCapacity { get; set; }
        public int RoomStatus { get; set; } // 1 Active, 2 Deleted
        public decimal RoomPricePerDate { get; set; }
        public int RoomTypeID { get; set; }
        public RoomType? RoomType { get; set; }
    }

    public class Customer
    {
        public int CustomerID { get; set; }
        public string CustomerFullName { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string EmailAddress { get; set; } = string.Empty;
        public DateTime CustomerBirthday { get; set; }
        public int CustomerStatus { get; set; } // 1 Active, 2 Deleted
        public string Password { get; set; } = string.Empty;
    }

    public class BookingReservation
    {
        public int BookingReservationID { get; set; }
        public int CustomerID { get; set; }
        public Customer? Customer { get; set; }
        public int RoomID { get; set; }
        public RoomInformation? Room { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int BookingDuration { get; set; }
        public decimal TotalPrice { get; set; }
        public int BookingStatus { get; set; } = 1; // 1: Active, 2: Completed, 3: Cancelled
        public int BookingType { get; set; } = 1; // 1: Online, 2: Offline
    }
}