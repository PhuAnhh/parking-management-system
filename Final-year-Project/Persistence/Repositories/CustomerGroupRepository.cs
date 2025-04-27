using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class CustomerGroupRepository : ICustomerGroupRepository
    {
        public readonly DeviceServiceContext _context;

        public CustomerGroupRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerGroup>> GetAllAsync()
        {
            return await _context.CustomerGroups.ToListAsync();
        }

        public async Task<CustomerGroup> GetByIdAsync(int id)
        {
            return await _context.CustomerGroups.FindAsync(id);
        }

        public async Task CreateAsync(CustomerGroup customerGroup)
        {
            await _context.CustomerGroups.AddAsync(customerGroup);
        }

        public void Update(CustomerGroup customerGroup)
        {
            _context.CustomerGroups.Update(customerGroup);
        }

        public void Delete(CustomerGroup customerGroup)
        {
            _context.CustomerGroups.Remove(customerGroup);
        }
    }
}
