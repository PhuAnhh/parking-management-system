using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class ControlUnitRepository : IControlUnitRepository
    {
        public readonly DeviceServiceContext _context;

        public ControlUnitRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ControlUnit>> GetAllAsync()
        {
            return await _context.ControlUnits.ToListAsync();
        }

        public async Task<ControlUnit> GetByIdAsync(int id)
        {
            return await _context.ControlUnits.FindAsync(id);
        }

        public async Task CreateAsync(ControlUnit controlUnit)
        {
            await _context.ControlUnits.AddAsync(controlUnit);
        }

        public void Update(ControlUnit controlUnit)
        {
            _context.ControlUnits.Update(controlUnit);
        }

        public void Delete(ControlUnit controlUnit)
        {
            _context.ControlUnits.Remove(controlUnit);
        }
    }
}
