using BusinessObjects;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DataAccessObjects
{
    public class CustomerDAO
    {
        // Singleton pattern
        private static CustomerDAO? instance;
        private static readonly object instanceLock = new object();

        private CustomerDAO() { }

        public static CustomerDAO Instance
        {
            get
            {
                lock (instanceLock)
                {
                    if (instance == null)
                    {
                        instance = new CustomerDAO();
                    }
                    return instance;
                }
            }
        }

        // Get all customers
        public List<Customer> GetCustomers()
        {
            using var context = new HotelDbContext();
            return context.Customers.Where(c => c.CustomerStatus == 1).ToList();
        }

        // Get customer by ID
        public Customer? GetCustomerByID(int id)
        {
            using var context = new HotelDbContext();
            return context.Customers.FirstOrDefault(c => c.CustomerID == id);
        }

        // Get customer by email
        public Customer? GetCustomerByEmail(string email)
        {
            using var context = new HotelDbContext();
            return context.Customers.Where(c => c.EmailAddress == email).FirstOrDefault();
        }

        // Add a new customer
        public void Add(Customer customer)
        {
            using var context = new HotelDbContext();
            context.Customers.Add(customer);
            context.SaveChanges();
        }

        // Update a customer
        public void Update(Customer customer)
        {
            using var context = new HotelDbContext();
            context.Entry(customer).State = EntityState.Modified;
            context.SaveChanges();
        }

        // Delete a customer
        public void Delete(int id)
        {
            using var context = new HotelDbContext();
            var customer = context.Customers.Find(id);
            if (customer != null)
            {
                customer.CustomerStatus = 2; // Mark as deleted
                context.SaveChanges();
            }
        }

        public static List<Customer> GetAll()
        {
            using var context = new HotelDbContext();
            return context.Customers.Where(c => c.CustomerStatus == 1).ToList();
        }
        
        public static Customer? Find(string email, string pass)
        {
            using var context = new HotelDbContext();
            return context.Customers.Where(c => 
                c.EmailAddress == email && 
                c.Password == pass && 
                c.CustomerStatus == 1).FirstOrDefault();
        }
        
        public static List<Customer> Search(string searchString)
        {
            using var context = new HotelDbContext();
            return context.Customers.Where(c => 
                c.CustomerStatus == 1 && 
                (EF.Functions.Like(c.CustomerFullName, "%" + searchString + "%") ||
                 EF.Functions.Like(c.EmailAddress, "%" + searchString + "%") ||
                 EF.Functions.Like(c.Telephone, "%" + searchString + "%"))
            ).ToList();
        }
    }
}