using Core.Entities;
using Shared.Dtos.PlayerDtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Core.MappingExtensions
{
    public static class PlayerMapping
    {
        public static PlayerDto To<T>(this PlayerEntity entity) where T : PlayerDto
        {
            return new PlayerDto(entity.Id, entity.ConnectionId, entity.Name, entity.PlayedCount, entity.WinCount);
           
        }
        
    }
}
