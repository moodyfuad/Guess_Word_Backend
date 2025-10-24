using Core.Entities;
using Core.Enums;
using Shared.Dtos.RoomDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.MappingExtensions
{
    public static class RoomMapping
    {
        public static RoomDto To<T>(this RoomEntity entity) where T : RoomDto
        {
            
            return new RoomDto(entity.Key, entity.WordLength, entity.MaxAttempts, entity.CreatorWord, entity.JoinerWord, entity.CreatorId, entity.JoinerId, entity.State.ToFriendlyString(), entity.CreatedAt);
        }
    }
}
