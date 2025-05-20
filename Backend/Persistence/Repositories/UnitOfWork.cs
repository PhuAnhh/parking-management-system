using Microsoft.EntityFrameworkCore;
using Final_year_Project.Domain.Entities;
using Final_year_Project.Persistence.DbContexts;
using Final_year_Project.Application.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Final_year_Project.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ParkingManagementContext _context;
        private ICameraRepository _cameraRepository;
        private IComputerRepository _computerRepository;
        private IControlUnitRepository _controlUnitRepository;
        private IGateRepository _gateRepository;
        private ILaneRepository _laneRepository;
        private ILaneCameraRepository _laneCameraRepository;
        private ILaneControlUnitRepository _laneControlUnitRepository;
        private ILedRepository _ledRepository;
        private ICustomerGroupRepository _customerGroupRepository;
        private ICustomerRepository _customerRepository;
        private ICardGroupRepository _cardGroupRepository;
        private ICardRepository _cardRepository;
        private ICardGroupLaneRepository _cardLaneRepository;
        private IEntryLogRepository _entryLogRepository;
        private IExitLogRepository _exitLogRepository;

        public UnitOfWork(ParkingManagementContext context)
        {
            _context = context;
        }

        public ICameraRepository Cameras => _cameraRepository ??= new CameraRepository(_context);
        public IComputerRepository Computers => _computerRepository ??= new ComputerRepository(_context);
        public IControlUnitRepository ControlUnits => _controlUnitRepository ??= new ControlUnitRepository(_context);
        public IGateRepository Gates => _gateRepository ??= new GateRepository(_context);
        public ILaneRepository Lanes => _laneRepository ??= new LaneRepository(_context);
        public ILaneCameraRepository LaneCameras => _laneCameraRepository ??= new LaneCameraRepository(_context);
        public ILaneControlUnitRepository LaneControlUnits => _laneControlUnitRepository ??= new LaneControlUnitRepository(_context);
        public ILedRepository Leds => _ledRepository ??= new LedRepository(_context);
        public ICustomerGroupRepository CustomerGroups => _customerGroupRepository ??= new CustomerGroupRepository(_context);
        public ICustomerRepository Customers => _customerRepository ??= new CustomerRepository(_context);
        public ICardGroupRepository CardGroups => _cardGroupRepository ??= new CardGroupRepository(_context);
        public ICardRepository Cards => _cardRepository ??= new CardRepository(_context);
        public ICardGroupLaneRepository CardGroupLanes => _cardLaneRepository ??= new CardGroupLaneRepository(_context);
        public IEntryLogRepository EntryLogs => _entryLogRepository ??= new EntryLogRepository(_context);
        public IExitLogRepository ExitLogs => _exitLogRepository ??= new ExitLogRepository(_context);
        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
