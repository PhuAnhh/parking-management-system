using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class ComputerRepository : IComputerRepository
    {
        public readonly DeviceServiceContext _context;

        public ComputerRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Computer>> GetAllAsync()
        {
            return await _context.Computers
                .Where(c => !c.Deleted)
                .ToListAsync();
        }

        public async Task<Computer> GetByIdAsync(int id)
        {
            return await _context.Computers
                .Include(c => c.Cameras)
                .FirstOrDefaultAsync(c => c.Id == id);
                //.FindAsync(id);
        }

        public async Task CreateAsync(Computer computer)
        {
            await _context.Computers.AddAsync(computer);
        }

        public void Update(Computer computer)
        {
            _context.Computers.Update(computer);
        }

        public void Delete(Computer computer)
        {
            _context.Computers.Remove(computer);
        }
    }
}
