using Core.Entities;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.Repositories
{
    internal class PlayerRepository : BaseRepository<PlayerEntity>, IPlayerRepository
    {
        private readonly AppDbContext _db;

        public PlayerRepository(AppDbContext db) : base(db)
        {
            this._db = db;
        }
    }
}
