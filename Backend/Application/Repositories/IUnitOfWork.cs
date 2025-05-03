using Final_year_Project.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Application.Repositories
{
    public interface IUnitOfWork
    {
        ICameraRepository Cameras { get; }

        IComputerRepository Computers { get; }

        IControlUnitRepository ControlUnits { get; }

        IGateRepository Gates { get; }

        ILaneRepository Lanes { get; }

        ILaneCameraRepository LaneCameras { get; }

        ILaneControlUnitRepository LaneControlUnits { get; }

        ILedRepository Leds { get; }

        ICustomerGroupRepository CustomerGroups { get; }

        ICustomerRepository Customers { get; }

        ICardGroupRepository CardGroups { get; }

        ICardRepository Cards { get; }

        ICardGroupLaneRepository CardGroupLanes { get; }

        Task<int> SaveChangesAsync();
    }
}
