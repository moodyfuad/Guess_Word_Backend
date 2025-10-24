using Core.Entities;
using Core.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Repositories.Implementation.Repositories
{
    internal class WordRepository : BaseRepository<WordEntity>, IWordRepository
    {
        private readonly AppDbContext _db;

        public WordRepository(AppDbContext db) : base(db)
        {
            this._db = db;
        }
    }
}
