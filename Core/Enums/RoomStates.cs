using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Enums
{
    public enum RoomStates
    {
        Created,
        PlayerJoined,
        WaitingForWord,
        InProgress,
        Finished
        
        
    }
    public static class RoomStatesExtensions
    {
        public static string ToFriendlyString(this RoomStates state)
        {
            return state switch
            {
                RoomStates.Created => "created",
                RoomStates.PlayerJoined => "playerJoined",
                RoomStates.WaitingForWord => "waitingForWord",
                RoomStates.InProgress => "inProgress",
                RoomStates.Finished => "finished",
                _ => "unknown"
            };
        }
    }
}
