using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Repositories
{
    public interface IRepositoryManager
    {
        IPlayerRepository Player { get; }
        IRoomRepository Room { get; }

        IWordRepository Word { get; }
    }
}
