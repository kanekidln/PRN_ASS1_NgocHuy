using BusinessObjects;
using System.Collections.Generic;

namespace Repositories
{
    public interface ICustomerRepository
    {
        List<Customer> GetAll();
        Customer? GetByID(int id);
        Customer? GetByEmail(string email);
        void Add(Customer customer);
        void Update(Customer customer);
        void Delete(int id);
        Customer? Login(string email, string password);
        List<Customer> Search(string searchString);
    }

    public class CustomerRepository : ICustomerRepository
    {
        private readonly DataAccessObjects.CustomerDAO _customerDAO;

        public CustomerRepository()
        {
            _customerDAO = DataAccessObjects.CustomerDAO.Instance;
        }

        public List<Customer> GetAll() => _customerDAO.GetCustomers();
        public Customer? GetByID(int id) => _customerDAO.GetCustomerByID(id);
        public Customer? GetByEmail(string email) => _customerDAO.GetCustomerByEmail(email);
        public void Add(Customer customer) => _customerDAO.Add(customer);
        public void Update(Customer customer) => _customerDAO.Update(customer);
        public void Delete(int id) => _customerDAO.Delete(id);
        public Customer? Login(string email, string password) => DataAccessObjects.CustomerDAO.Find(email, password);
        public List<Customer> Search(string searchString) => DataAccessObjects.CustomerDAO.Search(searchString);
    }
}