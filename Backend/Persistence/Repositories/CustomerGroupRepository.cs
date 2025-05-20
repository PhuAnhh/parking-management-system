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
        public readonly ParkingManagementContext _context;

        public CustomerGroupRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CustomerGroup>> GetAllAsync()
        {
            return await _context.CustomerGroups
                .Where(cg => !cg.Deleted)
                .ToListAsync();
        }

        public async Task<CustomerGroup> GetByIdAsync(int id)
        {
            return await _context.CustomerGroups
                .Include(c => c.Customers)
                .FirstOrDefaultAsync(c => c.Id == id);
                //.FindAsync(id);
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
