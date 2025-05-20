using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class GateRepository : IGateRepository
    {
        public readonly ParkingManagementContext _context;

        public GateRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Gate>> GetAllAsync()
        {
            return await _context.Gates
                .Where(g => !g.Deleted) 
                .ToListAsync();
                //.AsNoTracking().ToListAsync();
        }

        public async Task<Gate> GetByIdAsync(int id)
        {
            return await _context.Gates
                .Include(g => g.Computers)
                .FirstOrDefaultAsync(g => g.Id == id && !g.Deleted);
        }

        public async Task CreateAsync(Gate gate)
        {
            await _context.Gates.AddAsync(gate);
        }

        public void Update(Gate gate)
        {
            _context.Gates.Update(gate);
        }

        public void Delete(Gate gate)
        {
            _context.Gates.Remove(gate);
        }
    }
}
