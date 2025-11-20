using Core.Entities;
using Core.Enums;
using Core.MappingExtensions;
using Core.Repositories;
using Core.Services.Abstraction;
using Microsoft.AspNetCore.SignalR;
using Shared.Dtos.PlayerDtos;
using Shared.Dtos.RoomDtos;
using Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Services
{
    public class PlayerServices : IPlayerServices
    {
        private readonly IRepositoryManager _repos;

        public PlayerServices(IRepositoryManager repos)
        {
            this._repos = repos;
        }

        

        public async Task<PlayerDto> OnPlayerConnected(string connectionId, string playerId, string playerName, CancellationToken ct = default)
        {
            playerId = playerId.Trim('"', '/', '\\');
           var player = await _repos.Player.GetAsync(p => p.Id == playerId, ct: ct);
            if (player is null)
            {
                player = new()
                {
                    Id = playerId,
                    Name = playerName,
                    ConnectionId = connectionId,
                };
                await _repos.Player.AddAsync(player, ct);
            }
            else
            {
                // if the player was in a game room, handle that reconnection logic here (not implemented)

                player.ConnectionId = connectionId;
                player.Name = playerName;
                player.State = PlayerStates.Available;
                await _repos.Player.UpdateAsync(player, ct);
            }
            return new(player.Id,player.ConnectionId,player.Name,player.PlayedCount,player.WinCount);
        }

        public async Task<PlayerDto?> OnPlayerDisconnected(string connectionId, CancellationToken ct = default)
        {
            var player = await _repos.Player.GetAsync(p => p.ConnectionId == connectionId, ct: ct);
            if (player is null)
            {
                return null;
                return new(player?.Id ?? "", player?.ConnectionId ?? "", player?.Name ?? "", player?.PlayedCount ?? 0, player?.WinCount ?? 0);

            }
            //todo: check this logic
            //player.ConnectionId = null;
            //await _repos.Player.UpdateAsync(player, ct);
            //var room = await _repos.Room.GetAsync(r => r.CreatorId == player.Id && r.JoinerId == null, ct: ct);
            //if (room != null)
            //{
            //    await _repos.Room.DeleteAsync(room, ct);
            //}
            player.State = PlayerStates.Offline;
            await _repos.Player.UpdateAsync(player, ct);
            return player.To<PlayerDto>();

            // if the player is in a game room, handle that logic here (not implemented)
        }
        
        public async Task<GetOnlinePlayersResponseDto> GetOnlinePlayers(PagedListRequestParameters parameters, CancellationToken ct = default)
        {
            var players = await _repos.Player.GetPagedAsync(parameters, p => p.ConnectionId != null && p.State == PlayerStates.Available, ct: ct);
            var mapped = players.Items.Select(p => p.To<PlayerDto>()).ToList();
            return new GetOnlinePlayersResponseDto(pageNumber: players.PageNumber, pageSize: players.PageSize, totalCount: players.TotalCount, items: mapped);
        }

        public async Task<InvitePlayerResultDto> InvitePlayer(SendInvitationRequestDto dto, CancellationToken ct = default)
        {
            // Implementation for inviting a player can be added here
            var creator = await _repos.Player.GetAsync(p => p.Id == dto.FromPlayerId , ct: ct);
            var receiver = await _repos.Player.GetAsync(p=> p.Id == dto.ToPlayerId && p.State == PlayerStates.Available , ct: ct);
            if (receiver is null || creator is null)
            {
                return new InvitePlayerResultDto(false, "Player is Offline", null, null);
            }
            var room = await _repos.Room.GetAsync(r => r.CreatorId == creator.Id);
            if (room is not null)
            {
                await _repos.Room.DeleteAsync(room, ct);
            }
            creator.CreatedRoom = new RoomEntity(){
                Key = GenerateGameKey(4),
                Creator = creator,
                CreatorId = creator.Id,
                JoinerId = dto.ToPlayerId,
                Joiner = receiver,
                MaxAttempts = dto.MaxAttempts,
                WordLength= dto.WordLength
            };
            await _repos.Room.AddAsync(creator.CreatedRoom);
            //await _repos.Player.UpdateAsync(creator, ct);
            //await _repos.Player.UpdateAsync(receiver, ct);

            return new InvitePlayerResultDto(true, creator.CreatedRoom.Key, creator.To<PlayerDto>(), receiver.To<PlayerDto>());

        }
        public async Task<ResponseToInvitationResultDto> ResponseToInvitation(SendInvitationResponseDto dto, CancellationToken ct = default)
        {
            var creator = await _repos.Player.GetAsync(p => p.Id == dto.ToPlayerId && p.ConnectionId != null, ct: ct);
            var joiner = await _repos.Player.GetAsync(p => p.Id == dto.FromPlayerId, ct: ct);
            if (creator is null || joiner is null)
            {
                return new ResponseToInvitationResultDto(false, "Player is Offline", null, null, null);
            }
            var room = await _repos.Room.GetAsync(r => r.CreatorId == creator.Id && r.JoinerId != null, ct: ct);
            if (room is null)
            {
                return new ResponseToInvitationResultDto(false, "Invitation Expired", null, null, null);
            }
            if (dto.State == InvitationStates.Accepted)
            {
                //room.Joiner = joiner;
                //room.JoinerId = joiner.Id;
                room.State = Core.Enums.RoomStates.WaitingForWord;
                await _repos.Room.UpdateAsync(room, ct);
                return new ResponseToInvitationResultDto(true, "Invitation Accepted", creator.To<PlayerDto>(), joiner.To<PlayerDto>(), room.To<RoomDto>());
            }
            else
            {
                await _repos.Room.DeleteAsync(room, ct);
                return new ResponseToInvitationResultDto(true, "Invitation Rejected", creator.To<PlayerDto>(), joiner.To<PlayerDto>(), null);
            }

        }
        private string GenerateGameKey(int len)
        {
            const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            var sb = new StringBuilder();
            using var rng = RandomNumberGenerator.Create();
            var bytes = new byte[len];
            rng.GetBytes(bytes);
            for (int i = 0; i < len; i++)
            {
                sb.Append(chars[bytes[i] % chars.Length]);
            }
            return sb.ToString();
        }
    }
}
