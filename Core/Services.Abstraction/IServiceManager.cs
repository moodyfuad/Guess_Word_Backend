using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Services.Abstraction
{
    public interface IServiceManager
    {
        IPlayerServices PlayerService { get; }
        IRoomServices RoomService { get; }
    }
}
