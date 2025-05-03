using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class CardGroupLaneRepository : ICardGroupLaneRepository
    {
        public readonly DeviceServiceContext _context;

        public CardGroupLaneRepository(DeviceServiceContext context)
        {
            _context = context;
        }

        public void Delete(CardGroupLane cardGroupLane)
        {
            _context.CardGroupLanes.Remove(cardGroupLane);
        }
    }
}
