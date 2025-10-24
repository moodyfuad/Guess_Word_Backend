using Core.Repositories;
using Core.Services.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class ServiceManager : IServiceManager
    {
        private readonly IRepositoryManager _repositoryManager;

        public ServiceManager(IRepositoryManager repositoryManager)
        {
            _repositoryManager = repositoryManager;
            _playerServices ??= new Lazy<IPlayerServices>(() => new PlayerServices(_repositoryManager));
            _roomServices ??= new Lazy<IRoomServices>(() => new RoomServices(_repositoryManager));
        }

        private readonly Lazy<IPlayerServices> _playerServices;
        private readonly Lazy<IRoomServices> _roomServices;
        public IPlayerServices PlayerService => _playerServices.Value;
        public IRoomServices RoomService => _roomServices.Value;
    }
}
