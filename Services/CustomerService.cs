using BusinessObjects;
using Repositories;
using System.Collections.Generic;

namespace Services
{
    public class CustomerService
    {
        private readonly ICustomerRepository _repo = new CustomerRepository();
        
        public List<Customer> GetCustomers() => _repo.GetAll();
        
        public Customer? GetCustomerByID(int id) => _repo.GetByID(id);
        
        public Customer? GetCustomerByEmail(string email) => _repo.GetByEmail(email);
        
        public void AddCustomer(Customer customer) => _repo.Add(customer);
        
        public void UpdateCustomer(Customer customer) => _repo.Update(customer);
        
        public void DeleteCustomer(int id) => _repo.Delete(id);
        
        public Customer? Login(string email, string password) => _repo.Login(email, password);
        
        public List<Customer> SearchCustomers(string searchString) => _repo.Search(searchString);
    }
}