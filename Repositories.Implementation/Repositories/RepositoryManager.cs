using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.Repositories
{
    public class RepositoryManager : IRepositoryManager
    {

        public RepositoryManager(AppDbContext db)
        {
            _lazyPlayer ??= new Lazy<IPlayerRepository>(() => new PlayerRepository(db));
            _lazyRoom ??= new Lazy<IRoomRepository>(() => new RoomRepository(db));
            _lazyWord ??= new Lazy<IWordRepository>(() => new WordRepository(db));
        }
        private readonly Lazy<IPlayerRepository> _lazyPlayer;
        public IPlayerRepository Player { get { return _lazyPlayer.Value; } }
        
        private readonly Lazy<IRoomRepository> _lazyRoom;
        public IRoomRepository Room { get { return _lazyRoom.Value; } }

        private readonly Lazy<IWordRepository> _lazyWord;
        public IWordRepository Word { get { return _lazyWord.Value; } }

    }
}
