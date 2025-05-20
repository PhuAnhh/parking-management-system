using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class LaneControlUnitRepository : ILaneControlUnitRepository
    {
        public readonly ParkingManagementContext _context;

        public LaneControlUnitRepository(ParkingManagementContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<LaneControlUnit>> GetAllAsync()
        {
            return await _context.LaneControlUnits.ToListAsync();
        }

        public async Task<LaneControlUnit> GetByIdAsync(int id)
        {
            return await _context.LaneControlUnits.FindAsync(id);
        }

        public async Task CreateAsync(LaneControlUnit laneControlUnit)
        {
            await _context.LaneControlUnits.AddAsync(laneControlUnit);
        }

        public void Update(LaneControlUnit laneControlUnit)
        {
            _context.LaneControlUnits.Update(laneControlUnit);
        }

        public void Delete(LaneControlUnit laneControlUnit)
        {
            _context.LaneControlUnits.Remove(laneControlUnit);
        }
    }
}
