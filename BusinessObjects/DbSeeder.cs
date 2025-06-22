using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace BusinessObjects
{
    public static class DbSeeder
    {
        public static void Seed(HotelDbContext context)
        {
            context.Database.EnsureCreated();

            // Seed RoomTypes if empty
            if (!context.RoomTypes.Any())
            {
                context.RoomTypes.AddRange(
                    new RoomType
                    {
                        RoomTypeName = "Standard",
                        TypeDescription = "Standard room with basic amenities",
                        TypeNote = "Suitable for 1-2 persons"
                    },
                    new RoomType
                    {
                        RoomTypeName = "Deluxe",
                        TypeDescription = "Deluxe room with premium amenities",
                        TypeNote = "Suitable for 2-3 persons"
                    },
                    new RoomType
                    {
                        RoomTypeName = "Suite",
                        TypeDescription = "Luxurious suite with separate living area",
                        TypeNote = "Suitable for 2-4 persons"
                    }
                );
                context.SaveChanges();
            }

            // Seed RoomInformations if empty
            if (!context.RoomInformations.Any())
            {
                // Get room types from database to use their IDs
                var standardRoomType = context.RoomTypes.FirstOrDefault(rt => rt.RoomTypeName == "Standard");
                var deluxeRoomType = context.RoomTypes.FirstOrDefault(rt => rt.RoomTypeName == "Deluxe");
                var suiteRoomType = context.RoomTypes.FirstOrDefault(rt => rt.RoomTypeName == "Suite");

                if (standardRoomType != null && deluxeRoomType != null && suiteRoomType != null)
                {
                    context.RoomInformations.AddRange(
                        new RoomInformation
                        {
                            RoomNumber = "101",
                            RoomDescription = "Standard room with city view",
                            RoomMaxCapacity = 2,
                            RoomStatus = 1, // Active
                            RoomPricePerDate = 100.00M,
                            RoomTypeID = standardRoomType.RoomTypeID
                        },
                        new RoomInformation
                        {
                            RoomNumber = "201",
                            RoomDescription = "Deluxe room with ocean view",
                            RoomMaxCapacity = 3,
                            RoomStatus = 1, // Active
                            RoomPricePerDate = 150.00M,
                            RoomTypeID = deluxeRoomType.RoomTypeID
                        },
                        new RoomInformation
                        {
                            RoomNumber = "301",
                            RoomDescription = "Suite with balcony",
                            RoomMaxCapacity = 4,
                            RoomStatus = 1, // Active
                            RoomPricePerDate = 200.00M,
                            RoomTypeID = suiteRoomType.RoomTypeID
                        }
                    );
                    context.SaveChanges();
                }
            }

            // Seed Customers if empty
            if (!context.Customers.Any())
            {
                context.Customers.AddRange(
                    new Customer
                    {
                        CustomerFullName = "John Doe",
                        Telephone = "1234567890",
                        EmailAddress = "john.doe@example.com",
                        CustomerBirthday = new DateTime(1990, 1, 1),
                        CustomerStatus = 1, // Active
                        Password = "password123"
                    },
                    new Customer
                    {
                        CustomerFullName = "Jane Smith",
                        Telephone = "0987654321",
                        EmailAddress = "jane.smith@example.com",
                        CustomerBirthday = new DateTime(1992, 5, 15),
                        CustomerStatus = 1, // Active
                        Password = "password456"
                    }
                );
                context.SaveChanges();
            }

            // Seed BookingReservations if empty
            if (!context.BookingReservations.Any())
            {
                // Get customers and rooms from database to use their IDs
                var customer1 = context.Customers.FirstOrDefault(c => c.EmailAddress == "john.doe@example.com");
                var customer2 = context.Customers.FirstOrDefault(c => c.EmailAddress == "jane.smith@example.com");
                var room1 = context.RoomInformations.FirstOrDefault(r => r.RoomNumber == "101");
                var room2 = context.RoomInformations.FirstOrDefault(r => r.RoomNumber == "201");

                if (customer1 != null && customer2 != null && room1 != null && room2 != null)
                {
                    DateTime now = DateTime.Now;
                    context.BookingReservations.AddRange(
                        new BookingReservation
                        {
                            CustomerID = customer1.CustomerID,
                            RoomID = room1.RoomID,
                            BookingDate = now.AddDays(-10),
                            StartDate = now.AddDays(-5),
                            EndDate = now.AddDays(-3),
                            BookingDuration = 2,
                            TotalPrice = 200.00M,
                            BookingStatus = 2, // Completed
                            BookingType = 1 // Online
                        },
                        new BookingReservation
                        {
                            CustomerID = customer2.CustomerID,
                            RoomID = room2.RoomID,
                            BookingDate = now.AddDays(-3),
                            StartDate = now.AddDays(2),
                            EndDate = now.AddDays(5),
                            BookingDuration = 3,
                            TotalPrice = 450.00M,
                            BookingStatus = 1, // Active
                            BookingType = 1 // Online
                        }
                    );
                    context.SaveChanges();
                }
            }
        }
    }
} 